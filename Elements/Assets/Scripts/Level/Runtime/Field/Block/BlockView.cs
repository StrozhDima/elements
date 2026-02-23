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

        private StateExitBehaviour _destroyStateBehaviour;

        private StateExitBehaviour DestroyStateBehaviour
            => _destroyStateBehaviour ??= _animator.GetBehaviour<StateExitBehaviour>();

        void IBlockView.SetLocalPosition(Vector3 position) => transform.localPosition = position;

        void IBlockView.SetLocalScale(float scale) => transform.localScale = Vector3.one * scale;

        void IBlockView.SetSortingOrder(int order) => _spriteRenderer.sortingOrder = order;

        UniTask IBlockView.PlayFallAsync(Vector3 targetLocalPosition, CancellationToken cancellationToken)
            => transform.DOLocalMove(targetLocalPosition, _fallDuration).SetEase(_fallEase).WithCancellation(cancellationToken);

        UniTask IBlockView.PlayDestroyAsync(CancellationToken cancellationToken)
        {
            var awaiter = DestroyStateBehaviour.Exited.ToUniTask(useFirstValue: true, cancellationToken: cancellationToken);
            _animator.SetTrigger(_destroyAnimHash);
            return awaiter;
        }
    }
}