using UnityEngine;

namespace Elements.Level
{
    [CreateAssetMenu(fileName = "BalloonSpawnerConfig", menuName = "Elements/Balloons Background Config")]
    public sealed class BalloonsBackgroundConfig : ScriptableObject
    {
        [SerializeField]
        private int _maxBalloons = 3;
        [SerializeField]
        private float _minSpawnInterval = 2f;
        [SerializeField]
        private float _maxSpawnInterval = 6f;
        [SerializeField]
        private float _minSpeed = 0.5f;
        [SerializeField]
        private float _maxSpeed = 2f;
        [SerializeField]
        private float _minAmplitude = 0.3f;
        [SerializeField]
        private float _maxAmplitude = 0.8f;
        [SerializeField]
        private float _minFrequency = 0.5f;
        [SerializeField]
        private float _maxFrequency = 1.5f;

        public int MaxBalloons => _maxBalloons;
        public float MinSpawnInterval => _minSpawnInterval;
        public float MaxSpawnInterval => _maxSpawnInterval;
        public float MinSpeed => _minSpeed;
        public float MaxSpeed => _maxSpeed;
        public float MinAmplitude => _minAmplitude;
        public float MaxAmplitude => _maxAmplitude;
        public float MinFrequency => _minFrequency;
        public float MaxFrequency => _maxFrequency;
    }
}