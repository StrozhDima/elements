using System;

namespace Elements.Level
{
    [Serializable]
    public struct GameSaveData
    {
        public int LevelIndex;
        public int Width;
        public int Height;
        public int[] Cells;
    }
}