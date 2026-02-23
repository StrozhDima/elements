using JetBrains.Annotations;
using UniRx;

namespace Elements.Level
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class LevelState : ILevelState
    {
        private readonly ReactiveProperty<int> _levelIndex = new(0);
        private readonly ReactiveProperty<bool> _isNormalizing = new(false);

        IReadOnlyReactiveProperty<int> ILevelState.LevelIndex => _levelIndex;
        IReadOnlyReactiveProperty<bool> ILevelState.IsNormalizing => _isNormalizing;

        void ILevelState.SetLevelIndex(int index) => _levelIndex.Value = index;
        void ILevelState.SetNormalizing(bool value) => _isNormalizing.Value = value;
    }
}