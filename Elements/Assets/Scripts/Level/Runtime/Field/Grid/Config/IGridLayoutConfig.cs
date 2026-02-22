namespace Elements.Level
{
    public interface IGridLayoutConfig
    {
        float BottomOffsetFraction { get; }
        float TopOffsetFraction { get; }
        float SidePaddingFraction { get; }
        float MaxCellSize { get; }
    }
}
