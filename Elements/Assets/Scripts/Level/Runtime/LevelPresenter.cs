using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Elements.SwipeInput;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace Elements.Level
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class LevelPresenter : ILevelPresenter, IDisposable
    {
        private const int FirstLevelIndex = 0;
        private const int BlocksMergeMin = 3;

        private readonly ILevelModel _level;
        private readonly ILevelView _fieldView;
        private readonly IHUDPresenter _hudPresenter;
        private readonly ILevelState _state;
        private readonly ILevelDataProvider _dataProvider;
        private readonly ISaveService _saveService;
        private readonly IMoveValidator _moveValidator;
        private readonly INormalizationSystem _normalization;
        private readonly ISwipeInputProvider _inputProvider;
        private readonly CompositeDisposable _disposables;

        private IDisposable _inputDisposable;
        private CancellationTokenSource _initializeCts;
        private CancellationTokenSource _normalizationCts;

        public LevelPresenter(
            ILevelModel level,
            ILevelView fieldView,
            IHUDPresenter hudPresenter,
            ILevelState state,
            ILevelDataProvider dataProvider,
            ISaveService saveService,
            ISwipeInputProvider inputProvider)
        {
            _level = level;
            _fieldView = fieldView;
            _hudPresenter = hudPresenter;
            _state = state;
            _dataProvider = dataProvider;
            _saveService = saveService;
            _moveValidator = new MoveValidator(level);
            _normalization = new NormalizationSystem(fieldView, BlocksMergeMin);
            _inputProvider = inputProvider;
            _disposables = new CompositeDisposable();
        }

        async UniTask ILevelPresenter.InitializeAsync(CancellationToken cancellationToken)
        {
            InitializeLevelState();
            _hudPresenter.RestartRequested.Subscribe(_ => OnRestartRequested()).AddTo(_disposables);
            _hudPresenter.NextLevelRequested.Subscribe(_ => OnNextLevelRequested()).AddTo(_disposables);
            _initializeCts = new CancellationTokenSource();
            await InitializeFieldAsync(CancellationTokenSource.CreateLinkedTokenSource(_initializeCts.Token, cancellationToken).Token);
            InitializeInput();
            _normalizationCts = new CancellationTokenSource();
            RunNormalizationAsync(_normalizationCts.Token).Forget();
        }

        public void SaveProgress()
        {
            var saveData = _level.ToSaveData(_state.LevelIndex.Value);
            _saveService.Save(saveData);
        }

        private void InitializeLevelState()
        {
            if (_saveService.TryLoad(out var saveData))
            {
                _level.LoadFromSave(saveData);
                _state.SetLevelIndex(saveData.LevelIndex);
            }
            else
            {
                LoadLevelAsync(FirstLevelIndex, _initializeCts.Token).Forget();
            }
        }

        private UniTask InitializeFieldAsync(CancellationToken cancellationToken)
        {
            _fieldView.Setup(_level.Width, _level.Height);
            return InitializeBlocksAsync(cancellationToken);
        }

        private void InitializeInput() => _inputDisposable = _inputProvider.Swiped.Subscribe(HandleInputSwipe);

        private void DeInitializeInput() => _inputDisposable?.Dispose();

        private void HandleInputSwipe(SwipeInputData data)
        {
            if (!_fieldView.TryGetCellAtScreen(data.Position, out var col, out var row))
            {
                return;
            }

            HandleSwipe(col, row, data.Direction);
        }

        private void HandleSwipe(int col, int row, Vector2Int direction)
        {
            if (!_moveValidator.IsValidMove(col, row, direction))
            {
                return;
            }

            _level.TryMove(col, row, direction);
            _fieldView.SwapBlocks(col, row, col + direction.x, row + direction.y);
            SaveProgress();

            if (!_state.IsNormalizing.Value)
            {
                CancelNormalization();
                _normalizationCts = new CancellationTokenSource();
                RunNormalizationAsync(_normalizationCts.Token).Forget();
            }
        }

        private async UniTaskVoid RunNormalizationAsync(CancellationToken cancellationToken)
        {
            _state.SetNormalizing(true);
            await _normalization.NormalizeAsync(_level, cancellationToken);
            _state.SetNormalizing(false);

            if (_level.IsEmpty())
            {
                LoadNextLevel();
            }
        }

        private void OnRestartRequested()
        {
            CancelInitialize();
            CancelNormalization();
            _state.SetNormalizing(false);
            _initializeCts = new CancellationTokenSource();
            RestartLevelAsync(_initializeCts.Token).Forget();
        }

        private void OnNextLevelRequested()
        {
            CancelInitialize();
            CancelNormalization();
            _state.SetNormalizing(false);
            _initializeCts = new CancellationTokenSource();
            LoadNextLevel();
        }

        private void CancelNormalization()
        {
            if (_normalizationCts == null)
            {
                return;
            }

            _normalizationCts.Cancel();
            _normalizationCts.Dispose();
            _normalizationCts = null;
        }

        private void CancelInitialize()
        {
            if (_initializeCts == null)
            {
                return;
            }

            _initializeCts.Cancel();
            _initializeCts.Dispose();
            _initializeCts = null;
        }

        private void LoadNextLevel()
        {
            _saveService.Clear();
            LoadLevelAsync(_state.LevelIndex.Value + 1, _initializeCts.Token).Forget();
        }

        private async UniTaskVoid RestartLevelAsync(CancellationToken cancellationToken)
        {
            DeInitializeInput();
            _saveService.Clear();
            _level.LoadFromLevel(_dataProvider.GetLevel(_state.LevelIndex.Value));
            await InitializeFieldAsync(cancellationToken);
            InitializeInput();
        }

        private async UniTaskVoid LoadLevelAsync(int index, CancellationToken cancellationToken)
        {
            DeInitializeInput();
            _state.SetLevelIndex(index);
            _level.LoadFromLevel(_dataProvider.GetLevel(index));
            await InitializeFieldAsync(cancellationToken);
            InitializeInput();
        }

        private async UniTask InitializeBlocksAsync(CancellationToken cancellationToken)
        {
            for (var col = 0; col < _level.Height; col++)
            {
                var reverse = col % 2 != 0;
                var levelWidth = _level.Width;

                for (var row = 0; row < levelWidth; row++)
                {
                    var realRow = reverse ? levelWidth - 1 - row : row;
                    var blockType = _level.GetBlockType(realRow, col);
                    await _fieldView.RefreshBlockAsync(realRow, col, blockType, cancellationToken);
                }
            }
        }

        void IDisposable.Dispose()
        {
            _disposables.Dispose();
            CancelNormalization();
            CancelInitialize();
            DeInitializeInput();
        }
    }
}