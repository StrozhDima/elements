using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Elements.SwipeInput
{
    public sealed class SwipeInputProvider : MonoBehaviour, ISwipeInputProvider
    {
        [SerializeField]
        private float _swipeThresholdPixels = 30f;

        public IObservable<SwipeInputData> Swiped => _onSwipe ??= new Subject<SwipeInputData>();

        private InputAction _pressAction;
        private InputAction _positionAction;
        private Vector2 _startPosition;
        private Subject<SwipeInputData> _onSwipe;

        private void Awake()
        {
            _pressAction = new InputAction(binding: "<Pointer>/press");
            _positionAction = new InputAction(binding: "<Pointer>/position");
            _pressAction.started += OnPressStarted;
            _pressAction.canceled += OnPressEnded;
            _pressAction.Enable();
            _positionAction.Enable();
        }

        private void OnPressStarted(InputAction.CallbackContext context) =>
            _startPosition = _positionAction.ReadValue<Vector2>();

        private void OnPressEnded(InputAction.CallbackContext context)
        {
            var endScreenPos = _positionAction.ReadValue<Vector2>();
            var delta = endScreenPos - _startPosition;

            if (delta.magnitude < _swipeThresholdPixels)
            {
                return;
            }

            var direction = GetCardinalDirection(delta);

            if (direction == Vector2Int.zero)
            {
                return;
            }

            _onSwipe?.OnNext(new SwipeInputData(_startPosition, direction));
        }

        private static Vector2Int GetCardinalDirection(Vector2 delta)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                return delta.x > 0 ? Vector2Int.right : Vector2Int.left;
            }

            return delta.y > 0 ? Vector2Int.up : Vector2Int.down;
        }

        private void OnDestroy()
        {
            _pressAction.started -= OnPressStarted;
            _pressAction.canceled -= OnPressEnded;
            _pressAction.Disable();
            _positionAction.Disable();
            _pressAction.Dispose();
            _positionAction.Dispose();
        }
    }
}