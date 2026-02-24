using JetBrains.Annotations;

namespace Elements.Level
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class SoLevelDataProvider : ILevelDataProvider
    {
        private readonly SoLevelContainer _container;

        public SoLevelDataProvider(SoLevelContainer container) => _container = container;

        ILevelData ILevelDataProvider.GetLevel(int index) => _container.GetLevel(index);
    }
}