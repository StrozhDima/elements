using System;

namespace Elements.SwipeInput
{
    public interface ISwipeInputProvider
    {
        IObservable<SwipeInputData> Swiped { get; }
    }
}