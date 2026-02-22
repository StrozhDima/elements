using Elements.SwipeInput;
using UnityEngine;
using Zenject;

namespace Elements.Level
{
    public sealed class LevelInstaller : MonoInstaller
    {
        [SerializeField]
        private LevelView _levelView;
        [SerializeField]
        private HUDView _hudView;
        [SerializeField]
        private SoLevelContainer _soLevelContainer;
        [SerializeField]
        private SoBlockPrefabConfig _blockPrefabConfig;
        [SerializeField]
        private Transform _blockPoolRoot;
        [SerializeField]
        private SwipeInputProvider _swipeInputProvider;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<LevelModel>().AsSingle();
            Container.BindInterfacesTo<LevelPresenter>().AsSingle();
            Container.BindInterfacesTo<LevelEntryPoint>().AsSingle();
            Container.BindInterfacesTo<LevelView>().FromInstance(_levelView).AsSingle();
            Container.BindInterfacesTo<BlockViewFactory>().AsSingle().WithArguments(_blockPrefabConfig, _blockPoolRoot);
            Container.BindInterfacesTo<LevelStateModel>().AsSingle();
            Container.BindInterfacesTo<MoveValidator>().AsSingle();
            Container.BindInterfacesTo<NormalizationSystem>().AsSingle();
            Container.BindInterfacesTo<SoLevelDataProvider>().AsSingle().WithArguments(_soLevelContainer);
            Container.BindInterfacesTo<SwipeInputProvider>().FromInstance(_swipeInputProvider).AsSingle();
            Container.BindInterfacesTo<HUDView>().FromInstance(_hudView).AsSingle();
            Container.BindInterfacesTo<SaveService>().AsSingle();
        }
    }
}