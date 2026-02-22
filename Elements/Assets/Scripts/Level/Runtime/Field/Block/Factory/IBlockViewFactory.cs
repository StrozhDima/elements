using Elements.Common;
using UnityEngine;

namespace Elements.Level
{
    public interface IBlockViewFactory
    {
        void Initialize(int poolSize);
        IBlockView Get(BlockType type, Transform parent);
        void Release(IBlockView view);
    }
}
