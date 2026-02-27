using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Elements.Level
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class LevelEntryPoint : IInitializable, IDisposable
    {
        private readonly ILevelPresenter _presenter;
        private readonly IHUDPresenter _hudPresenter;
        private readonly IBlockViewFactory _blockViewFactory;
        private readonly int _blockPoolWarmupSize;
        private readonly CancellationToken _cancellationToken;

        public LevelEntryPoint(
            ILevelPresenter presenter,
            IHUDPresenter hudPresenter,
            IBlockViewFactory blockViewFactory,
            int blockPoolWarmupSize,
            CancellationToken cancellationToken)
        {
            _presenter = presenter;
            _hudPresenter = hudPresenter;
            _blockViewFactory = blockViewFactory;
            _blockPoolWarmupSize = blockPoolWarmupSize;
            _cancellationToken = cancellationToken;
        }

        void IInitializable.Initialize()
        {
            _blockViewFactory.Initialize(_blockPoolWarmupSize);
            _hudPresenter.Initialize();
            _presenter.InitializeAsync(_cancellationToken).Forget();
            Application.quitting += OnApplicationQuitting;
        }

        void IDisposable.Dispose() => Application.quitting -= OnApplicationQuitting;

        private void OnApplicationQuitting() => _presenter.SaveProgress();
    }
}