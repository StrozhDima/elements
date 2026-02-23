using UniRx;

namespace Elements.Level
{
    public interface ILevelState
    {
        IReadOnlyReactiveProperty<int> LevelIndex { get; }
        IReadOnlyReactiveProperty<bool> IsNormalizing { get; }
        void SetLevelIndex(int index);
        void SetNormalizing(bool value);
    }
}