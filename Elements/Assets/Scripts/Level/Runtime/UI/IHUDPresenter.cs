using System;
using UniRx;

namespace Elements.Level
{
    public interface IHUDPresenter
    {
        IObservable<Unit> RestartRequested { get; }
        IObservable<Unit> NextLevelRequested { get; }
        void Initialize();
    }
}