using UnityEngine;

namespace Elements.Level
{
    [CreateAssetMenu(fileName = "GridLayoutConfig", menuName = "Elements/Grid Layout Config")]
    public sealed class SoGridLayoutConfig : ScriptableObject, IGridLayoutConfig
    {
        [SerializeField]
        private float _bottomOffsetFraction = 0.10f;
        [SerializeField]
        private float _topOffsetFraction = 0.05f;
        [SerializeField]
        private float _sidePaddingFraction = 0.05f;
        [SerializeField]
        private float _maxCellSize;

        float IGridLayoutConfig.BottomOffsetFraction => _bottomOffsetFraction;
        float IGridLayoutConfig.TopOffsetFraction => _topOffsetFraction;
        float IGridLayoutConfig.SidePaddingFraction => _sidePaddingFraction;
        float IGridLayoutConfig.MaxCellSize => _maxCellSize;
    }
}
