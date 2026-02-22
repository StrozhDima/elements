using Elements.Common;

namespace Elements.Level
{
    public interface ILevelData
    {
        int Width { get; }
        int Height { get; }
        BlockType? GetCell(int col, int row);
    }
}
