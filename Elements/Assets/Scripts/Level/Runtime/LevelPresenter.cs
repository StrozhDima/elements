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
            _normalization = new NormalizationSystem(fieldView);
            _inputProvider = inputProvider;
            _disposables = new CompositeDisposable();
        }

        void ILevelPresenter.Initialize()
        {
            InitializeLevelState();
            InitializeField();
            _hudPresenter.RestartRequested.Subscribe(_ => OnRestartRequested()).AddTo(_disposables);
            _hudPresenter.NextLevelRequested.Subscribe(_ => OnNextLevelRequested()).AddTo(_disposables);
            InitializeInput();
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
                LoadLevel(FirstLevelIndex);
            }
        }

        private void InitializeField()
        {
            _fieldView.Setup(_level.Width, _level.Height);
            RefreshAllBlocks();
        }

        private void InitializeInput() => _inputProvider.Swiped.Subscribe(HandleInputSwipe).AddTo(_disposables);

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
            CancelNormalization();
            _state.SetNormalizing(false);
            RestartLevel();
        }

        private void OnNextLevelRequested()
        {
            CancelNormalization();
            _state.SetNormalizing(false);
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

        private void LoadNextLevel()
        {
            _saveService.Clear();
            LoadLevel(_state.LevelIndex.Value + 1);
        }

        private void RestartLevel()
        {
            _saveService.Clear();
            _level.LoadFromLevel(_dataProvider.GetLevel(_state.LevelIndex.Value));
            InitializeField();
        }

        private void LoadLevel(int index)
        {
            _state.SetLevelIndex(index);
            _level.LoadFromLevel(_dataProvider.GetLevel(index));
            InitializeField();
        }

        private void RefreshAllBlocks()
        {
            for (var col = 0; col < _level.Width; col++)
            {
                for (var row = 0; row < _level.Height; row++)
                {
                    _fieldView.RefreshBlock(col, row, _level.GetBlockType(col, row));
                }
            }
        }

        void IDisposable.Dispose()
        {
            CancelNormalization();
            _disposables.Dispose();
        }
    }
}