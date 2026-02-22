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
        private const float GridHeightFraction = 0.70f;
        private const float GridBottomOffsetFraction = 0.10f;

        [SerializeField]
        private Transform _gridRoot;
        [SerializeField]
        private Camera _camera;

        private IBlockViewFactory _blockViewFactory;
        private IBlockView[,] _blockViews;
        private BlockType?[,] _blockTypes;
        private int _width;
        private int _height;
        private float _worldCellSize;
        private Vector2 _gridOrigin;

        [Inject]
        public void Construct(IBlockViewFactory blockViewFactory) => _blockViewFactory = blockViewFactory;

        public void InitializeGrid(int width, int height)
        {
            _width = width;
            _height = height;

            if (_blockViews != null)
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

            _blockViews = new IBlockView[width, height];
            _blockTypes = new BlockType?[width, height];

            ComputeLayout(width, height);
        }

        public void RefreshBlock(int col, int row, BlockType? type)
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

            var view = _blockViewFactory.Get(type.Value, _gridRoot);
            view.SetLocalPosition(GetCellLocalPosition(col, row));
            view.SetLocalScale(_worldCellSize);
            view.SetSortingOrder(row * _width + col);

            _blockViews[col, row] = view;
        }

        public void SwapBlocks(int colA, int rowA, int colB, int rowB)
        {
            (_blockViews[colA, rowA], _blockViews[colB, rowB]) = (_blockViews[colB, rowB], _blockViews[colA, rowA]);
            (_blockTypes[colA, rowA], _blockTypes[colB, rowB]) = (_blockTypes[colB, rowB], _blockTypes[colA, rowA]);

            UpdateBlockTransform(colA, rowA);
            UpdateBlockTransform(colB, rowB);
        }

        public bool TryGetCellAtScreen(Vector2 screenPos, out int col, out int row)
        {
            var worldPos = (Vector2)_camera.ScreenToWorldPoint(
                new Vector3(screenPos.x, screenPos.y, 0f));

            col = Mathf.RoundToInt((worldPos.x - _gridOrigin.x) / _worldCellSize);
            row = Mathf.RoundToInt((worldPos.y - _gridOrigin.y) / _worldCellSize);
            return col >= 0 && col < _width && row >= 0 && row < _height;
        }

        public async UniTask PlayDestroyAsync(IReadOnlyList<Vector2Int> cells, CancellationToken cancellationToken)
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

        public UniTask PlayFallAsync(Vector2Int from, Vector2Int to, CancellationToken cancellationToken)
        {
            var view = _blockViews[from.x, from.y];
            _blockViews[to.x, to.y] = view;
            _blockViews[from.x, from.y] = null;
            _blockTypes[to.x, to.y] = _blockTypes[from.x, from.y];
            _blockTypes[from.x, from.y] = null;

            view.SetSortingOrder(to.y * _width + to.x);
            return view.PlayFallAsync(GetCellLocalPosition(to.x, to.y), cancellationToken);
        }

        private void UpdateBlockTransform(int col, int row)
        {
            var view = _blockViews[col, row];

            if (view == null)
            {
                return;
            }

            view.SetLocalPosition(GetCellLocalPosition(col, row));
            view.SetSortingOrder(row * _width + col);
        }

        private void ComputeLayout(int cols, int rows)
        {
            var pixelsPerUnit = Screen.height / (_camera.orthographicSize * 2f);
            var cellSizePixels = Mathf.Min(
                (float)Screen.width / cols,
                Screen.height * GridHeightFraction / rows);

            _worldCellSize = cellSizePixels / pixelsPerUnit;

            var gridWorldWidth = _worldCellSize * cols;
            var bottomOffsetWorld = Screen.height * GridBottomOffsetFraction / pixelsPerUnit;

            _gridOrigin = new Vector2(
                -gridWorldWidth * 0.5f + _worldCellSize * 0.5f,
                -_camera.orthographicSize + bottomOffsetWorld + _worldCellSize * 0.5f);
        }

        private Vector3 GetCellLocalPosition(int col, int row) =>
            new(_gridOrigin.x + col * _worldCellSize, _gridOrigin.y + row * _worldCellSize, 0f);
    }
}
