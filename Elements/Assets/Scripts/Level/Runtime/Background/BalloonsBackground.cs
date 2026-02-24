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
        private readonly Dictionary<BalloonView, BalloonPoolData> _balloonData = new();
        private readonly Stack<int> _availableSortingOrders = new();
        private List<BalloonView>[] _inactivePools;
        private int _nextPrefabIndex;

        private void Awake()
        {
            _inactivePools = new List<BalloonView>[_balloonPrefabs.Length];

            for (var i = 0; i < _balloonPrefabs.Length; i++)
            {
                _inactivePools[i] = new List<BalloonView>();
            }

            for (var i = 0; i < _config.MaxBalloons; i++)
            {
                _availableSortingOrders.Push(i);
            }
        }

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

            if (_availableSortingOrders.Count == 0)
            {
                return;
            }

            var halfHeight = _camera.orthographicSize;
            var halfWidth = _camera.orthographicSize * _camera.aspect;
            var direction = Random.value > 0.5f ? 1f : -1f;
            var x = direction > 0 ? -halfWidth - 1f : halfWidth + 1f;
            var y = Random.Range(-halfHeight * 0.33f, halfHeight * 0.33f);
            var index = GetNextIndex();
            var view = GetBalloonView(index);
            view.transform.position = new Vector3(x, y, 0f);
            view.transform.localScale = new Vector3(direction > 0 ? 1f : -1f, 1f, 1f);

            var viewData = new BalloonViewData(
                speed: Random.Range(_config.MinSpeed, _config.MaxSpeed),
                direction: direction,
                baseY: y,
                amplitude: Random.Range(_config.MinAmplitude, _config.MaxAmplitude),
                frequency: Random.Range(_config.MinFrequency, _config.MaxFrequency),
                exitX: direction > 0 ? halfWidth + 2f : -halfWidth - 2f);
            InitializeBallonView(view, viewData, index);
        }

        private int GetNextIndex()
        {
            var index = _nextPrefabIndex;
            _nextPrefabIndex = (_nextPrefabIndex + 1) % _balloonPrefabs.Length;
            return index;
        }

        private BalloonView GetBalloonView(int index)
        {
            var pool = _inactivePools[index];
            BalloonView view;

            if (pool.Count > 0)
            {
                view = pool[^1];
                pool.RemoveAt(pool.Count - 1);
                view.gameObject.SetActive(true);
            }
            else
            {
                var go = Instantiate(_balloonPrefabs[index], transform);
                view = go.GetComponent<BalloonView>();
            }

            return view;
        }

        private void InitializeBallonView(BalloonView view, BalloonViewData viewData, int prefabIndex)
        {
            view.Initialize(viewData);
            var sortingOrder = _availableSortingOrders.Pop();
            view.SetSortingOrder(sortingOrder);
            _balloonData[view] = new BalloonPoolData(prefabIndex, sortingOrder);
            view.OnExited += OnBalloonExited;
            _activeBalloons.Add(view);
        }

        private void OnBalloonExited(BalloonView balloon)
        {
            balloon.OnExited -= OnBalloonExited;
            _activeBalloons.Remove(balloon);
            var data = _balloonData[balloon];
            _balloonData.Remove(balloon);
            balloon.gameObject.SetActive(false);
            _inactivePools[data.PrefabIndex].Add(balloon);
            _availableSortingOrders.Push(data.SortingOrder);
        }
    }
}