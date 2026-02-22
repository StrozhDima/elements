using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Elements.Level
{
    public sealed class HUDView : MonoBehaviour, IHUDView
    {
        [SerializeField]
        private Button _restartButton;
        [SerializeField]
        private Button _nextButton;
        [SerializeField]
        private TextMeshProUGUI _levelIndexText;

        private bool _initialized;
        private IObservable<Unit> _restartClicked;
        private IObservable<Unit> _nextClicked;

        IObservable<Unit> IHUDView.RestartClicked => _restartClicked;
        IObservable<Unit> IHUDView.NextClicked => _nextClicked;

        void IHUDView.Initialize()
        {
            if (_initialized)
            {
                throw new InvalidOperationException("HUD view is already initialized");
            }

            _initialized = true;
            _restartClicked = _restartButton.OnClickAsObservable().Share();
            _nextClicked = _nextButton.OnClickAsObservable().Share();
        }

        void IHUDView.SetLevelIndex(int index) => _levelIndexText.text = $"Level {index + 1}";
    }
}