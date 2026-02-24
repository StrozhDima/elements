using System;
using UnityEngine;

namespace Elements.Level
{
    [CreateAssetMenu(fileName = "Level", menuName = "Elements/Level Data")]
    public sealed class SoLevelData : ScriptableObject, ILevelData
    {
        [Serializable]
        private struct RowData
        {
            [SerializeField]
            private int[] _cells;

            public int Count => _cells.Length;

            public int GetCell(int col) => col >= 0 && col < Count ? _cells[col] : -1;
        }

        [SerializeField]
        private RowData[] _rows;

        public int Width => _rows.Length > 0 ? _rows[0].Count : 0;
        public int Height => _rows.Length;

        public BlockType? GetCell(int col, int row)
        {
            if (col < 0 || col >= Width || row < 0 || row >= Height)
            {
                return null;
            }

            var cell = _rows[row].GetCell(col);
            return cell < 0 ? null : (BlockType)cell;
        }
    }
}