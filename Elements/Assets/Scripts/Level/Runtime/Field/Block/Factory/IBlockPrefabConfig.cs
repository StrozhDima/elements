using System.Collections.Generic;
using Elements.Common;
using UnityEngine;

namespace Elements.Level
{
    public interface IBlockPrefabConfig
    {
        IEnumerable<BlockType> Types { get; }
        GameObject GetPrefab(BlockType type);
    }
}
