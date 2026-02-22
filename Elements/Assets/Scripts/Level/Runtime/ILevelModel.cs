using Elements.Common;
using UnityEngine;

namespace Elements.Level
{
    public interface ILevelModel
    {
        int Width { get; }
        int Height { get; }
        BlockType? GetBlockType(int col, int row);
        BlockState GetBlockState(int col, int row);
        void SetCell(int col, int row, BlockType? type);
        void SetState(int col, int row, BlockState state);
        bool IsEmpty();
        bool TryMove(int col, int row, Vector2Int direction);
        void LoadFromLevel(ILevelData data);
        void LoadFromSave(GameSaveData saveData);
        GameSaveData ToSaveData(int levelIndex);
    }
}