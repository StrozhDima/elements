using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Elements.Level
{
    public sealed class BlockView : MonoBehaviour, IBlockView
    {
        private static readonly int _destroyAnimHash = Animator.StringToHash("Destroy");

        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        [SerializeField]
        private float _fallDuration = 0.25f;
        [SerializeField]
        private Ease _fallEase = Ease.InQuad;
        [SerializeField]
        private float _scaleDuration = 0.2f;
        [SerializeField]
        private AnimationCurve _scaleEase;
        [SerializeField]
        private float _destroyDuration = 1.42f;

        void IBlockView.SetLocalPosition(Vector3 position) => transform.localPosition = position;

        void IBlockView.SetLocalScale(float scale) => transform.localScale = Vector3.one * scale;

        public UniTask SetLocalScaleAnimatedAsync(float scale, CancellationToken cancellationToken)
        {
            var targetScale = Vector3.one * scale;
            return transform.DOScale(targetScale, _scaleDuration).SetEase(_scaleEase).SetLink(gameObject)
                .ToUniTask(cancellationToken: cancellationToken);
        }

        void IBlockView.SetSortingOrder(int order) => _spriteRenderer.sortingOrder = order;

        UniTask IBlockView.PlayFallAsync(Vector3 targetLocalPosition, CancellationToken cancellationToken)
            => transform.DOLocalMove(targetLocalPosition, _fallDuration).SetEase(_fallEase).WithCancellation(cancellationToken);

        UniTask IBlockView.PlayDestroyAsync(CancellationToken cancellationToken)
        {
            _animator.SetTrigger(_destroyAnimHash);
            return UniTask.WaitForSeconds(_destroyDuration, cancellationToken: cancellationToken);
        }
    }
}