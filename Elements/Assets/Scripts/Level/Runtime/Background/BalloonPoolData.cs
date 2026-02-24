namespace Elements.Level
{
    public readonly struct BalloonPoolData
    {
        public readonly int PrefabIndex;
        public readonly int SortingOrder;

        public BalloonPoolData(int prefabIndex, int sortingOrder)
        {
            PrefabIndex = prefabIndex;
            SortingOrder = sortingOrder;
        }
    }
}
