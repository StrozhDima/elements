using System.Collections.Generic;
using UnityEngine;

namespace Elements.Level
{
    public sealed class BalloonSpawnerComponent : MonoBehaviour
    {
        private const int MaxBalloons = 3;
        private const float MinSpawnInterval = 2f;
        private const float MaxSpawnInterval = 6f;
        private const float MinSpeed = 0.5f;
        private const float MaxSpeed = 2f;
        private const float MinAmplitude = 0.3f;
        private const float MaxAmplitude = 0.8f;
        private const float MinFrequency = 0.5f;
        private const float MaxFrequency = 1.5f;

        [SerializeField]
        private GameObject[] _balloonPrefabs;

        private readonly List<BalloonView> _activeBalloons = new();
        private float _nextSpawnTime;
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
            ScheduleNextSpawn();
        }

        private void Update()
        {
            if (_activeBalloons.Count >= MaxBalloons)
            {
                return;
            }

            if (Time.time < _nextSpawnTime)
            {
                return;
            }

            SpawnBalloon();
            ScheduleNextSpawn();
        }

        private void SpawnBalloon()
        {
            if (_balloonPrefabs == null || _balloonPrefabs.Length == 0)
            {
                return;
            }

            var prefab = _balloonPrefabs[Random.Range(0, _balloonPrefabs.Length)];
            var go = Instantiate(prefab, transform);

            var halfWidth = _camera.orthographicSize * _camera.aspect;
            var direction = Random.value > 0.5f ? 1f : -1f;
            var spawnX = direction > 0 ? -halfWidth - 1f : halfWidth + 1f;
            var exitX = direction > 0 ? halfWidth + 2f : -halfWidth - 2f;

            var halfHeight = _camera.orthographicSize;
            var baseY = Random.Range(-halfHeight * 0.33f, halfHeight * 0.33f);

            go.transform.position = new Vector3(spawnX, baseY, 0f);

            var scaleX = direction > 0 ? 1f : -1f;
            go.transform.localScale = new Vector3(scaleX, 1f, 1f);

            var view = go.AddComponent<BalloonView>();
            view.Initialize(
                speed: Random.Range(MinSpeed, MaxSpeed),
                direction: direction,
                baseY: baseY,
                amplitude: Random.Range(MinAmplitude, MaxAmplitude),
                frequency: Random.Range(MinFrequency, MaxFrequency),
                exitX: exitX);

            view.OnExited += OnBalloonExited;
            _activeBalloons.Add(view);
        }

        private void OnBalloonExited(BalloonView balloon)
        {
            balloon.OnExited -= OnBalloonExited;
            _activeBalloons.Remove(balloon);
            Destroy(balloon.gameObject);
        }

        private void ScheduleNextSpawn() => _nextSpawnTime = Time.time + Random.Range(MinSpawnInterval, MaxSpawnInterval);
    }
}