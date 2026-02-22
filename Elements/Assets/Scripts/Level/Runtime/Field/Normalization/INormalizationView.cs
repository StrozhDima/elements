using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Elements.Level
{
    public interface INormalizationView
    {
        UniTask PlayDestroyAsync(IReadOnlyList<Vector2Int> cells, CancellationToken cancellationToken);
        UniTask PlayFallAsync(Vector2Int from, Vector2Int to, CancellationToken cancellationToken);
    }
}
