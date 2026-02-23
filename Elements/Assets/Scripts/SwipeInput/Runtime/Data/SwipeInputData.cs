using UnityEngine;

namespace Elements.SwipeInput
{
    public readonly struct SwipeInputData
    {
        public readonly Vector2 Position;
        public readonly Vector2Int Direction;

        public SwipeInputData(Vector2 position, Vector2Int direction)
        {
            Position = position;
            Direction = direction;
        }
    }
}