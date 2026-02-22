using System;
using UnityEngine;

namespace Elements.Level
{
    public sealed class BalloonView : MonoBehaviour
    {
        public event Action<BalloonView> OnExited;

        private float _speed;
        private float _direction;
        private float _baseY;
        private float _amplitude;
        private float _frequency;
        private float _exitX;

        public void Initialize(float speed, float direction, float baseY, float amplitude, float frequency, float exitX)
        {
            _speed = speed;
            _direction = direction;
            _baseY = baseY;
            _amplitude = amplitude;
            _frequency = frequency;
            _exitX = exitX;
        }

        private void Update()
        {
            var pos = transform.position;
            pos.x += _direction * _speed * Time.deltaTime;
            pos.y = _baseY + _amplitude * Mathf.Sin(_frequency * Time.time);
            transform.position = pos;

            if ((_direction > 0 && pos.x > _exitX) || (_direction < 0 && pos.x < _exitX))
                OnExited?.Invoke(this);
        }
    }
}
