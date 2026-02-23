using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Elements.Level
{
    [CreateAssetMenu(fileName = "BlockPrefabConfig", menuName = "Elements/Block Prefab Config")]
    public sealed class SoBlockPrefabConfig : ScriptableObject, IBlockPrefabConfig
    {
        [Serializable]
        private struct Entry
        {
            public BlockType Type;
            public GameObject Prefab;
        }

        [SerializeField]
        private Entry[] _entries;

        IEnumerable<BlockType> IBlockPrefabConfig.Types => _entries.Select(entry => entry.Type);

        GameObject IBlockPrefabConfig.GetPrefab(BlockType type)
        {
            foreach (var entry in _entries)
            {
                if (entry.Type == type)
                {
                    return entry.Prefab;
                }
            }

            return null;
        }
    }
}