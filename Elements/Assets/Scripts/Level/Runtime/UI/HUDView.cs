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
        private TextMeshProUGUI _levelText;
        [SerializeField]
        private string _levelTextKey;

        private IObservable<Unit> _restartClicked;
        private IObservable<Unit> _nextClicked;

        IObservable<Unit> IHUDView.RestartClicked => _restartClicked;
        IObservable<Unit> IHUDView.NextClicked => _nextClicked;

        void IHUDView.Initialize()
        {
            _restartClicked = _restartButton.OnClickAsObservable().Share();
            _nextClicked = _nextButton.OnClickAsObservable().Share();
        }

        void IHUDView.SetLevelNumber(int number) => _levelText.text = string.Format(_levelTextKey, number);
    }
}