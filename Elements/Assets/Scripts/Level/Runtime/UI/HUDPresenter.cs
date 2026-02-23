using System;
using JetBrains.Annotations;
using UniRx;

namespace Elements.Level
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class HUDPresenter : IHUDPresenter, IDisposable
    {
        private readonly IHUDView _view;
        private readonly ILevelState _levelState;

        private IDisposable _disposable;

        IObservable<Unit> IHUDPresenter.RestartRequested => _view.RestartClicked;
        IObservable<Unit> IHUDPresenter.NextLevelRequested => _view.NextClicked;

        public HUDPresenter(
            IHUDView view,
            ILevelState levelState)
        {
            _view = view;
            _levelState = levelState;
        }

        void IHUDPresenter.Initialize()
        {
            _view.Initialize();
            _disposable = _levelState.LevelIndex.Subscribe(index => _view.SetLevelNumber(index + 1));
        }

        void IDisposable.Dispose() => _disposable?.Dispose();
    }
}