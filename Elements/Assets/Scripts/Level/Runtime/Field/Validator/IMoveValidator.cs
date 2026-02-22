using UnityEngine;

namespace Elements.Level
{
    public interface IMoveValidator
    {
        bool IsValidMove(int col, int row, Vector2Int direction);
    }
}