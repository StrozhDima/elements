using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Elements.Level
{
    public interface IBlockView
    {
        void SetLocalPosition(Vector3 position);
        void SetLocalScale(float scale);
        UniTask SetLocalScaleAnimatedAsync(float scale, CancellationToken cancellationToken);
        void SetSortingOrder(int order);
        UniTask PlayFallAsync(Vector3 targetLocalPosition, CancellationToken cancellationToken);
        UniTask PlayDestroyAsync(CancellationToken cancellationToken);
    }
}
