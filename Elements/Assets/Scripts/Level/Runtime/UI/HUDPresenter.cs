using System;
using JetBrains.Annotations;
using UniRx;

namespace Elements.Level
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class HUDPresenter : IHUDPresenter, IDisposable
    {
        private readonly IHUDView _view;
        private readonly ILevelStateModel _state;

        private IDisposable _disposable;

        IObservable<Unit> IHUDPresenter.RestartRequested => _view.RestartClicked;
        IObservable<Unit> IHUDPresenter.NextLevelRequested => _view.NextClicked;

        public HUDPresenter(IHUDView view, ILevelStateModel state)
        {
            _view = view;
            _state = state;
        }

        void IHUDPresenter.Initialize()
        {
            _view.Initialize();
            _disposable = _state.LevelIndex.Subscribe(index => _view.SetLevelIndex(index));
        }

        void IDisposable.Dispose() => _disposable?.Dispose();
    }
}
