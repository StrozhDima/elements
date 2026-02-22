using System;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Elements.Level
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class LevelEntryPoint : IInitializable, IDisposable
    {
        private const int BlockPoolWarmupSize = 8;

        private readonly ILevelPresenter _presenter;
        private readonly IBlockViewFactory _blockViewFactory;

        public LevelEntryPoint(ILevelPresenter presenter, IBlockViewFactory blockViewFactory)
        {
            _presenter = presenter;
            _blockViewFactory = blockViewFactory;
        }

        void IInitializable.Initialize()
        {
            _blockViewFactory.Initialize(BlockPoolWarmupSize);
            _presenter.Initialize();
            Application.quitting += OnApplicationQuitting;
        }

        void IDisposable.Dispose()
        {
            Application.quitting -= OnApplicationQuitting;
            _presenter.DeInitialize();
        }

        private void OnApplicationQuitting() => _presenter.SaveProgress();
    }
}
