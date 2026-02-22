using System;
using Elements.Common;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace Elements.Level
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class LevelModel : ILevelModel
    {
        private BlockType?[,] _cells;
        private BlockState[,] _states;
        private readonly Subject<Unit> _onBoardChanged = new();

        private int Width { get; set; }
        int ILevelModel.Width => Width;
        private int Height { get; set; }
        int ILevelModel.Height => Height;
        IObservable<Unit> ILevelModel.OnBoardChanged => _onBoardChanged;

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

        public void NotifyChanged() => _onBoardChanged.OnNext(Unit.Default);

        bool ILevelModel.TryMove(int col, int row, Vector2Int direction)
        {
            var targetCol = col + direction.x;
            var targetRow = row + direction.y;
            (_cells[col, row], _cells[targetCol, targetRow]) = (_cells[targetCol, targetRow], _cells[col, row]);
            NotifyChanged();
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

            NotifyChanged();
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
                    _cells[col, row] = saveData.Cells[index] switch
                    {
                        0 => BlockType.Water,
                        1 => BlockType.Fire,
                        _ => null
                    };
                }
            }

            NotifyChanged();
        }

        GameSaveData ILevelModel.ToSaveData(int levelIndex)
        {
            var cells = new int[Width * Height];

            for (var col = 0; col < Width; col++)
            {
                for (var row = 0; row < Height; row++)
                {
                    cells[row * Width + col] = _cells[col, row] switch
                    {
                        BlockType.Water => 0,
                        BlockType.Fire => 1,
                        _ => -1
                    };
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