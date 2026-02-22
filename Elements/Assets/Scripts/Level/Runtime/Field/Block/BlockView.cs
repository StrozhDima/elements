using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Elements.Level
{
    public sealed class BlockView : MonoBehaviour, IBlockView
    {
        private const float DestroyAnimationDuration = 1.42f;
        private const float FallDuration = 0.25f;

        private static readonly int _destroyHash = Animator.StringToHash("Destroy");

        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        void IBlockView.SetLocalPosition(Vector3 position) => transform.localPosition = position;

        void IBlockView.SetLocalScale(float scale) => transform.localScale = Vector3.one * scale;

        void IBlockView.SetSortingOrder(int order) => _spriteRenderer.sortingOrder = order;

        UniTask IBlockView.PlayFallAsync(Vector3 targetLocalPosition, CancellationToken cancellationToken)
            => transform.DOLocalMove(targetLocalPosition, FallDuration)
                .SetEase(Ease.InQuad)
                .WithCancellation(cancellationToken);

        async UniTask IBlockView.PlayDestroyAsync(CancellationToken cancellationToken)
        {
            _animator.SetTrigger(_destroyHash);
            await UniTask.Delay(TimeSpan.FromSeconds(DestroyAnimationDuration), cancellationToken: cancellationToken);
        }
    }
}