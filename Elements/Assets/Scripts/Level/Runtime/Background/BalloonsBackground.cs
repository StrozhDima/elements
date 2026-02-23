using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Elements.Level
{
    public sealed class BalloonsBackground : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private GameObject[] _balloonPrefabs;
        [SerializeField]
        private BalloonsBackgroundConfig _config;

        private readonly List<BalloonView> _activeBalloons = new();
        private readonly List<BalloonView> _inactiveBalloons = new();

        private void Start() => SpawnLoopAsync(destroyCancellationToken).Forget();

        private async UniTaskVoid SpawnLoopAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                var interval = Random.Range(_config.MinSpawnInterval, _config.MaxSpawnInterval);
                await UniTask.Delay(TimeSpan.FromSeconds(interval), cancellationToken: cancellationToken);

                if (_activeBalloons.Count < _config.MaxBalloons)
                {
                    SpawnBalloon();
                }
            }
        }

        private void SpawnBalloon()
        {
            if (_balloonPrefabs.Length == 0)
            {
                return;
            }

            var halfWidth = _camera.orthographicSize * _camera.aspect;
            var direction = Random.value > 0.5f ? 1f : -1f;
            var spawnX = direction > 0 ? -halfWidth - 1f : halfWidth + 1f;
            var exitX = direction > 0 ? halfWidth + 2f : -halfWidth - 2f;
            var halfHeight = _camera.orthographicSize;
            var baseY = Random.Range(-halfHeight * 0.33f, halfHeight * 0.33f);

            var view = GetBalloonView();
            view.transform.position = new Vector3(spawnX, baseY, 0f);
            view.transform.localScale = new Vector3(direction > 0 ? 1f : -1f, 1f, 1f);

            view.Initialize(new BalloonViewData(
                speed: Random.Range(_config.MinSpeed, _config.MaxSpeed),
                direction: direction,
                baseY: baseY,
                amplitude: Random.Range(_config.MinAmplitude, _config.MaxAmplitude),
                frequency: Random.Range(_config.MinFrequency, _config.MaxFrequency),
                exitX: exitX));

            view.OnExited += OnBalloonExited;
            _activeBalloons.Add(view);
        }

        private BalloonView GetBalloonView()
        {
            BalloonView view;

            if (_inactiveBalloons.Count > 0)
            {
                view = _inactiveBalloons[^1];
                _inactiveBalloons.RemoveAt(_inactiveBalloons.Count - 1);
                view.gameObject.SetActive(true);
            }
            else
            {
                var prefab = _balloonPrefabs[Random.Range(0, _balloonPrefabs.Length)];
                var go = Instantiate(prefab, transform);
                view = go.GetComponent<BalloonView>();
            }

            return view;
        }

        private void OnBalloonExited(BalloonView balloon)
        {
            balloon.OnExited -= OnBalloonExited;
            _activeBalloons.Remove(balloon);
            balloon.gameObject.SetActive(false);
            _inactiveBalloons.Add(balloon);
        }
    }
}