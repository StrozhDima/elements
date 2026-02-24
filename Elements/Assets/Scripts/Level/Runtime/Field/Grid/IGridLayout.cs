using UnityEngine;

namespace Elements.Level
{
    public interface IGridLayout
    {
        float CellSize { get; }
        void Compute(int cols, int rows);
        Vector3 GetCellLocalPosition(int col, int row);
        bool TryGetCellAtScreen(Vector2 screenPos, out int col, out int row);
    }
}