using System.Collections.Generic;
using Elements.Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Elements.Level
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class BlockViewFactory : IBlockViewFactory
    {
        private readonly IBlockPrefabConfig _config;
        private readonly Transform _poolRoot;
        private readonly Dictionary<BlockType, Queue<BlockView>> _pools;
        private readonly Dictionary<IBlockView, BlockType> _activeViews;

        public BlockViewFactory(
            IBlockPrefabConfig config,
            Transform poolRoot)
        {
            _config = config;
            _poolRoot = poolRoot;
            _pools = new Dictionary<BlockType, Queue<BlockView>>();
            _activeViews = new Dictionary<IBlockView, BlockType>();
        }

        void IBlockViewFactory.Initialize(int poolSize)
        {
            foreach (var type in _config.Types)
            {
                _pools[type] = new Queue<BlockView>();
                Prewarm(type, _pools[type], poolSize);
            }
        }

        IBlockView IBlockViewFactory.Get(BlockType type, Transform parent)
        {
            var pool = _pools[type];
            var view = pool.Count > 0
                ? pool.Dequeue()
                : Object.Instantiate(_config.GetPrefab(type), parent).GetComponent<BlockView>();

            view.transform.SetParent(parent);
            view.gameObject.SetActive(true);
            _activeViews[view] = type;

            return view;
        }

        void IBlockViewFactory.Release(IBlockView blockView)
        {
            if (!_activeViews.Remove(blockView, out var type))
            {
                return;
            }

            if (blockView is not BlockView view)
            {
                return;
            }

            view.gameObject.SetActive(false);
            view.transform.SetParent(_poolRoot);
            _pools[type].Enqueue(view);
        }

        private void Prewarm(BlockType type, Queue<BlockView> pool, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var instance = Object.Instantiate(_config.GetPrefab(type), _poolRoot);
                instance.SetActive(false);
                pool.Enqueue(instance.GetComponent<BlockView>());
            }
        }
    }
}