using Elements.Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Elements.Level
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class LevelModel : ILevelModel
    {
        private const int EmptyCellId = -1;

        private BlockType?[,] _cells;
        private BlockState[,] _states;

        private int Width { get; set; }
        int ILevelModel.Width => Width;
        private int Height { get; set; }
        int ILevelModel.Height => Height;

        BlockType? ILevelModel.GetBlockType(int col, int row) => _cells[col, row];

        BlockState ILevelModel.GetBlockState(int col, int row) => _states[col, row];

        void ILevelModel.SetCell(int col, int row, BlockType? type)
        {
            _cells[col, row] = type;
            _states[col, row] = BlockState.Idle;
        }

        void ILevelModel.SetState(int col, int row, BlockState state) => _states[col, row] = state;

        bool ILevelModel.IsEmpty()
        {
            for (var col = 0; col < Width; col++)
            {
                for (var row = 0; row < Height; row++)
                {
                    if (_cells[col, row].HasValue)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        bool ILevelModel.TryMove(int col, int row, Vector2Int direction)
        {
            var targetCol = col + direction.x;
            var targetRow = row + direction.y;
            (_cells[col, row], _cells[targetCol, targetRow]) = (_cells[targetCol, targetRow], _cells[col, row]);
            return true;
        }

        void ILevelModel.LoadFromLevel(ILevelData data)
        {
            Width = data.Width;
            Height = data.Height;
            _cells = new BlockType?[Width, Height];
            _states = new BlockState[Width, Height];

            for (var col = 0; col < Width; col++)
            {
                for (var row = 0; row < Height; row++)
                {
                    _cells[col, row] = data.GetCell(col, row);
                }
            }
        }

        void ILevelModel.LoadFromSave(GameSaveData saveData)
        {
            Width = saveData.Width;
            Height = saveData.Height;
            _cells = new BlockType?[Width, Height];
            _states = new BlockState[Width, Height];

            for (var col = 0; col < Width; col++)
            {
                for (var row = 0; row < Height; row++)
                {
                    var index = row * Width + col;
                    var encoded = saveData.Cells[index];
                    _cells[col, row] = encoded == EmptyCellId ? null : (BlockType)encoded;
                }
            }
        }

        GameSaveData ILevelModel.ToSaveData(int levelIndex)
        {
            var cells = new int[Width * Height];

            for (var col = 0; col < Width; col++)
            {
                for (var row = 0; row < Height; row++)
                {
                    var cell = _cells[col, row];
                    cells[row * Width + col] = cell.HasValue ? (int)cell.Value : EmptyCellId;
                }
            }

            return new GameSaveData
            {
                LevelIndex = levelIndex,
                Width = Width,
                Height = Height,
                Cells = cells
            };
        }
    }
}