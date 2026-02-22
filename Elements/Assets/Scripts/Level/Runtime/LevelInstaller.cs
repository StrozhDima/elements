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
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private SoGridLayoutConfig _gridLayoutConfig;
        [SerializeField]
        private int _blockPoolWarmupSize = 8;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<LevelModel>().AsSingle();
            Container.BindInterfacesTo<LevelPresenter>().AsSingle().WithArguments(_levelView);
            Container.BindInterfacesTo<HUDPresenter>().AsSingle().WithArguments(_hudView);
            Container.BindInterfacesTo<LevelEntryPoint>().AsSingle().WithArguments(_blockPoolWarmupSize);
            Container.BindInterfacesTo<BlockViewFactory>().AsSingle().WithArguments(_blockPrefabConfig, _blockPoolRoot);
            Container.BindInterfacesTo<GridLayout>().AsSingle().WithArguments(_gridLayoutConfig, _camera);
            Container.BindInterfacesTo<LevelStateModel>().AsSingle();
            Container.BindInterfacesTo<SoLevelDataProvider>().AsSingle().WithArguments(_soLevelContainer);
            Container.BindInterfacesTo<SwipeInputProvider>().FromInstance(_swipeInputProvider).AsSingle();
            Container.BindInterfacesTo<SaveService>().AsSingle();
        }
    }
}