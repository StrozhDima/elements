using System.Threading;
using Cysharp.Threading.Tasks;
using Elements.SwipeInput;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace Elements.Level
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class LevelPresenter : ILevelPresenter
    {
        private readonly ILevelModel _level;
        private readonly ILevelView _fieldView;
        private readonly IHUDView _hudView;
        private readonly ILevelStateModel _state;
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
            IHUDView hudView,
            ILevelStateModel state,
            ILevelDataProvider dataProvider,
            ISaveService saveService,
            IMoveValidator moveValidator,
            INormalizationSystem normalization,
            ISwipeInputProvider inputProvider)
        {
            _level = level;
            _fieldView = fieldView;
            _hudView = hudView;
            _state = state;
            _dataProvider = dataProvider;
            _saveService = saveService;
            _moveValidator = moveValidator;
            _normalization = normalization;
            _inputProvider = inputProvider;
            _disposables = new CompositeDisposable();
        }

        void ILevelPresenter.Initialize()
        {
            InitializeLevelState();
            InitializeField();
            InitializeHUDView();
            InitializeInput();
        }

        void ILevelPresenter.DeInitialize()
        {
            CancelNormalization();
            _disposables.Dispose();
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
                LoadLevel(0);
            }
        }

        private void InitializeField()
        {
            _fieldView.InitializeGrid(_level.Width, _level.Height);
            RefreshAllBlocks();
        }

        private void InitializeHUDView()
        {
            _hudView.Initialize();
            _state.LevelIndex.Subscribe(index => _hudView.SetLevelIndex(index)).AddTo(_disposables);
            _hudView.RestartClicked.Subscribe(_ => OnRestartClicked()).AddTo(_disposables);
            _hudView.NextClicked.Subscribe(_ => OnNextClicked()).AddTo(_disposables);
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
            if (!_moveValidator.IsValidMove(_level, col, row, direction))
            {
                return;
            }

            _level.TryMove(col, row, direction);
            _fieldView.SwapBlocks(col, row, col + direction.x, row + direction.y);
            SaveProgress();

            if (!_state.IsNormalizing.Value)
            {
                RunNormalizationAsync().Forget();
            }
        }

        private async UniTaskVoid RunNormalizationAsync()
        {
            CancelNormalization();
            _state.SetNormalizing(true);
            _normalizationCts = new CancellationTokenSource();
            await _normalization.NormalizeAsync(_level, _normalizationCts.Token);
            _state.SetNormalizing(false);

            if (_level.IsEmpty())
            {
                LoadNextLevel();
            }
        }

        private void OnRestartClicked()
        {
            CancelNormalization();
            _state.SetNormalizing(false);
            RestartLevel();
        }

        private void OnNextClicked()
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
    }
}