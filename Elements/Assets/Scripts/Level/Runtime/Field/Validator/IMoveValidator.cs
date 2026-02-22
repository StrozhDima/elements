using UnityEngine;

namespace Elements.Level
{
    public interface IMoveValidator
    {
        bool IsValidMove(ILevelModel level, int col, int row, Vector2Int direction);
    }
}