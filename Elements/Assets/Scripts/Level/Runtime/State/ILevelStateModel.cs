using UniRx;

namespace Elements.Level
{
    public interface ILevelStateModel
    {
        IReadOnlyReactiveProperty<int> LevelIndex { get; }
        IReadOnlyReactiveProperty<bool> IsNormalizing { get; }
        void SetLevelIndex(int index);
        void SetNormalizing(bool value);
    }
}