using System;
using Elements.Common;
using UniRx;
using UnityEngine;

namespace Elements.Level
{
    public interface ILevelModel
    {
        int Width { get; }
        int Height { get; }
        IObservable<Unit> OnBoardChanged { get; }
        BlockType? GetBlockType(int col, int row);
        BlockState GetBlockState(int col, int row);
        void SetCell(int col, int row, BlockType? type);
        void SetState(int col, int row, BlockState state);
        bool IsEmpty();
        void NotifyChanged();
        bool TryMove(int col, int row, Vector2Int direction);
        void LoadFromLevel(ILevelData data);
        void LoadFromSave(GameSaveData saveData);
        GameSaveData ToSaveData(int levelIndex);
    }
}