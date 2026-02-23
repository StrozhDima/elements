using System;
using UnityEngine;

namespace Elements.Level
{
    public sealed class BalloonView : MonoBehaviour
    {
        [SerializeField]
        private Transform _transform;

        public event Action<BalloonView> OnExited;

        private BalloonViewData _data;

        public void Initialize(BalloonViewData data) => _data = data;

        private void Update()
        {
            var pos = _transform.position;
            pos.x += _data.Direction * _data.Speed * Time.deltaTime;
            pos.y = _data.BaseY + _data.Amplitude * Mathf.Sin(_data.Frequency * Time.time);
            _transform.position = pos;

            if ((_data.Direction > 0 && pos.x > _data.ExitX) || (_data.Direction < 0 && pos.x < _data.ExitX))
            {
                OnExited?.Invoke(this);
            }
        }
    }
}