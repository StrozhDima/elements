using Elements.Common;
using UnityEngine;

namespace Elements.Level
{
    [CreateAssetMenu(fileName = "Level", menuName = "Elements/Level Data")]
    public sealed class SoLevelData : ScriptableObject, ILevelData
    {
        [SerializeField]
        private int _width;
        [SerializeField]
        private int _height;
        [SerializeField]
        private int[] _cells;

        public int Width => _width;
        public int Height => _height;

        public BlockType? GetCell(int col, int row)
        {
            if (col < 0 || col >= _width || row < 0 || row >= _height)
            {
                return null;
            }

            var index = (_height - 1 - row) * _width + col;

            if (index < 0 || index >= _cells.Length)
            {
                return null;
            }

            return _cells[index] switch
            {
                0 => BlockType.Water,
                1 => BlockType.Fire,
                _ => null
            };
        }
    }
}