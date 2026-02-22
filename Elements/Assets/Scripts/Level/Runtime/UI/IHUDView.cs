using System;
using UniRx;

namespace Elements.Level
{
    public interface IHUDView
    {
        IObservable<Unit> RestartClicked { get; }
        IObservable<Unit> NextClicked { get; }

        void Initialize();
        void SetLevelIndex(int index);
    }
}
