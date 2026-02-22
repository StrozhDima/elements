using Elements.Common;
using UnityEngine;

namespace Elements.Level
{
    public interface ILevelView : INormalizationView
    {
        void InitializeGrid(int width, int height);
        void RefreshBlock(int col, int row, BlockType? type);
        void SwapBlocks(int colA, int rowA, int colB, int rowB);
        bool TryGetCellAtScreen(Vector2 screenPos, out int col, out int row);
    }
}
