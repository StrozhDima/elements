using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Elements.Level
{
    public interface INormalizationSystem
    {
        UniTask NormalizeAsync(ILevelModel level, CancellationToken cancellationToken);
        List<(Vector2Int from, Vector2Int to)> ApplyGravity(ILevelModel level);
        List<Vector2Int> FindDestroyableRegions(ILevelModel level);
    }
}