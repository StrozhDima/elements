using UnityEngine;

namespace Elements.Level
{
    [CreateAssetMenu(fileName = "LevelContainer", menuName = "Elements/Level Container")]
    public sealed class SoLevelContainer : ScriptableObject
    {
        [SerializeField]
        private SoLevelData[] _levels;

        public int Count => _levels.Length;

        public SoLevelData GetLevel(int index) => _levels[index % _levels.Length];
    }
}