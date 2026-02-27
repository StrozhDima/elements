using System.Threading;
using Cysharp.Threading.Tasks;

namespace Elements.Level
{
    public interface ILevelPresenter
    {
        UniTask InitializeAsync(CancellationToken cancellationToken);
        void SaveProgress();
    }
}