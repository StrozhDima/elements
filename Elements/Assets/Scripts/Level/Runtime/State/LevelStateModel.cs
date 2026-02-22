using JetBrains.Annotations;
using UniRx;

namespace Elements.Level
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class LevelStateModel : ILevelStateModel
    {
        private readonly ReactiveProperty<int> _levelIndex = new(0);
         private readonly ReactiveProperty<bool> _isNormalizing = new(false);

        IReadOnlyReactiveProperty<int> ILevelStateModel.LevelIndex => _levelIndex;
        IReadOnlyReactiveProperty<bool> ILevelStateModel.IsNormalizing => _isNormalizing;

        void ILevelStateModel.SetLevelIndex(int index) => _levelIndex.Value = index;
        void ILevelStateModel.SetNormalizing(bool value) => _isNormalizing.Value = value;
    }
}