using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Elements.Level
{
    public interface ILevelView : INormalizationView
    {
        void Setup(int width, int height);
        UniTask RefreshBlockAsync(int col, int row, BlockType? type, CancellationToken cancellationToken);
        void SwapBlocks(int colA, int rowA, int colB, int rowB);
        bool TryGetCellAtScreen(Vector2 screenPos, out int col, out int row);
    }
}
