using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Elements.Common;
using UnityEngine;
using Zenject;

namespace Elements.Level
{
    public sealed class LevelView : MonoBehaviour, ILevelView
    {
        [SerializeField]
        private Transform _blockContainer;

        private IGridLayout _gridLayout;
        private IBlockViewFactory _blockViewFactory;
        private IBlockView[,] _blockViews;
        private BlockType?[,] _blockTypes;
        private int _width;

        [Inject]
        public void Construct(
            IGridLayout gridLayout,
            IBlockViewFactory blockViewFactory)
        {
            _gridLayout = gridLayout;
            _blockViewFactory = blockViewFactory;
        }

        void ILevelView.Setup(int width, int height)
        {
            _width = width;

            if (_blockViews != null)
            {
                ReleaseBlockViews();
            }

            PrepareData(width, height);
            _gridLayout.Compute(width, height);
        }

        void ILevelView.RefreshBlock(int col, int row, BlockType? type)
        {
            if (_blockTypes == null || _blockTypes[col, row] == type)
            {
                return;
            }

            _blockTypes[col, row] = type;

            if (_blockViews[col, row] != null)
            {
                _blockViewFactory.Release(_blockViews[col, row]);
                _blockViews[col, row] = null;
            }

            if (!type.HasValue)
            {
                return;
            }

            var view = _blockViewFactory.Get(type.Value, _blockContainer);
            view.SetLocalPosition(_gridLayout.GetCellLocalPosition(col, row));
            view.SetLocalScale(_gridLayout.CellSize);
            view.SetSortingOrder(row * _width + col);

            _blockViews[col, row] = view;
        }

        void ILevelView.SwapBlocks(int colA, int rowA, int colB, int rowB)
        {
            (_blockViews[colA, rowA], _blockViews[colB, rowB]) = (_blockViews[colB, rowB], _blockViews[colA, rowA]);
            (_blockTypes[colA, rowA], _blockTypes[colB, rowB]) = (_blockTypes[colB, rowB], _blockTypes[colA, rowA]);

            UpdateBlockTransform(colA, rowA);
            UpdateBlockTransform(colB, rowB);
        }

        bool ILevelView.TryGetCellAtScreen(Vector2 screenPos, out int col, out int row) =>
            _gridLayout.TryGetCellAtScreen(screenPos, out col, out row);

        async UniTask INormalizationView.PlayDestroyAsync(IReadOnlyList<Vector2Int> cells, CancellationToken cancellationToken)
        {
            var tasks = new List<UniTask>();

            foreach (var cell in cells)
            {
                var view = _blockViews[cell.x, cell.y];
                tasks.Add(view.PlayDestroyAsync(cancellationToken));
            }

            await UniTask.WhenAll(tasks);

            foreach (var cell in cells)
            {
                _blockViewFactory.Release(_blockViews[cell.x, cell.y]);
                _blockViews[cell.x, cell.y] = null;
                _blockTypes[cell.x, cell.y] = null;
            }
        }

        UniTask INormalizationView.PlayFallAsync(Vector2Int from, Vector2Int to, CancellationToken cancellationToken)
        {
            var view = _blockViews[from.x, from.y];
            _blockViews[to.x, to.y] = view;
            _blockViews[from.x, from.y] = null;
            _blockTypes[to.x, to.y] = _blockTypes[from.x, from.y];
            _blockTypes[from.x, from.y] = null;

            view.SetSortingOrder(to.y * _width + to.x);
            return view.PlayFallAsync(_gridLayout.GetCellLocalPosition(to.x, to.y), cancellationToken);
        }

        private void ReleaseBlockViews()
        {
            for (var col = 0; col < _blockViews.GetLength(0); col++)
            {
                for (var row = 0; row < _blockViews.GetLength(1); row++)
                {
                    if (_blockViews[col, row] != null)
                    {
                        _blockViewFactory.Release(_blockViews[col, row]);
                    }
                }
            }
        }

        private void PrepareData(int width, int height)
        {
            var needsResize = _blockViews == null
                           || _blockViews.GetLength(0) != width
                           || _blockViews.GetLength(1) != height;

            if (needsResize)
            {
                _blockViews = new IBlockView[width, height];
                _blockTypes = new BlockType?[width, height];
            }
            else
            {
                Array.Clear(_blockViews, 0, _blockViews.Length);
                Array.Clear(_blockTypes, 0, _blockTypes.Length);
            }
        }

        private void UpdateBlockTransform(int col, int row)
        {
            var view = _blockViews[col, row];

            if (view == null)
            {
                return;
            }

            view.SetLocalPosition(_gridLayout.GetCellLocalPosition(col, row));
            view.SetSortingOrder(row * _width + col);
        }
    }
}