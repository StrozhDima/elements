namespace Elements.Level
{
    public interface ISaveService
    {
        void Save(GameSaveData data);
        bool TryLoad(out GameSaveData data);
        void Clear();
    }
}
