using JetBrains.Annotations;
using UnityEngine;

namespace Elements.Level
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class GridLayout : IGridLayout
    {
        private readonly IGridLayoutConfig _config;
        private readonly Camera _camera;

        private float _worldCellSize;
        private Vector2 _gridOrigin;
        private int _width;
        private int _height;

        float IGridLayout.CellSize => _worldCellSize;

        public GridLayout(
            IGridLayoutConfig config,
            Camera camera)
        {
            _config = config;
            _camera = camera;
        }

        void IGridLayout.Compute(int cols, int rows)
        {
            _width = cols;
            _height = rows;

            var cameraHeight = _camera.orthographicSize * 2f;
            var cameraWidth = cameraHeight * _camera.aspect;

            var bottomOffset = cameraHeight * _config.BottomOffsetFraction;
            var topOffset = cameraHeight * _config.TopOffsetFraction;
            var sideOffset = cameraWidth * _config.SidePaddingFraction;

            var availableWidth = cameraWidth - 2f * sideOffset;
            var availableHeight = cameraHeight - bottomOffset - topOffset;

            _worldCellSize = Mathf.Min(availableWidth / cols, availableHeight / rows);

            if (_config.MaxCellSize > 0f)
            {
                _worldCellSize = Mathf.Min(_worldCellSize, _config.MaxCellSize);
            }

            var gridWorldWidth = _worldCellSize * cols;

            _gridOrigin = new Vector2(
                -gridWorldWidth * 0.5f + _worldCellSize * 0.5f,
                -_camera.orthographicSize + bottomOffset + _worldCellSize * 0.5f);
        }

        Vector3 IGridLayout.GetCellLocalPosition(int col, int row) =>
            new(_gridOrigin.x + col * _worldCellSize, _gridOrigin.y + row * _worldCellSize, 0f);

        bool IGridLayout.TryGetCellAtScreen(Vector2 screenPos, out int col, out int row)
        {
            var worldPos = (Vector2)_camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
            col = Mathf.RoundToInt((worldPos.x - _gridOrigin.x) / _worldCellSize);
            row = Mathf.RoundToInt((worldPos.y - _gridOrigin.y) / _worldCellSize);
            return col >= 0 && col < _width && row >= 0 && row < _height;
        }
    }
}