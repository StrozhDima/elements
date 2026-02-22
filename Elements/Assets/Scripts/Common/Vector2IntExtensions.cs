using UnityEngine;

namespace Elements.Common
{
    public static class Vector2IntExtensions
    {
        public static bool IsCardinal(this Vector2Int direction)
        {
            return direction == Vector2Int.up
                   || direction == Vector2Int.down
                   || direction == Vector2Int.left
                   || direction == Vector2Int.right;
        }
    }
}
