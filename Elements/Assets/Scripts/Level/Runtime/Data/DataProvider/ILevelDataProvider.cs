namespace Elements.Level
{
    public interface ILevelDataProvider
    {
        int Count { get; }
        ILevelData GetLevel(int index);
    }
}
