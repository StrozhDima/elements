using System;
using UniRx;
using UnityEngine;

namespace Elements.Level
{
    public sealed class StateExitBehaviour : StateMachineBehaviour
    {
        private readonly Subject<Unit> _exited = new();

        public IObservable<Unit> Exited => _exited;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) =>
            _exited.OnNext(Unit.Default);
    }
}