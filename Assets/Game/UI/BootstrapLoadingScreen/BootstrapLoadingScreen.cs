using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Maniac.LanguageTableSystem;
using Maniac.UISystem;
using TMPro;
using UniRx;
using UnityEngine.UI;

namespace Game
{
    public class BootstrapLoadingScreen : BaseUI , IDisposable
    {
        [SerializeField] private Slider progressSlider;
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private LanguageItem progressLanguageItem;
        
        private FloatReactiveProperty _progress;
        private string _loadingFormatText;

        // This method will be call first, at this point, UI hasn't show up on canvas yet.
        // Use this to init your UI
        public override void OnSetup(object parameter = null) //first
        {
            _progress = (FloatReactiveProperty)parameter;
            _loadingFormatText = progressLanguageItem.GetCurrentLanguageText();

            UpdateLoadingProgress(0f);
            _progress.Subscribe(UpdateLoadingProgress).AddTo(this);
            base.OnSetup(parameter);
        }

        public void UpdateLoadingProgress(float value)
        {
            progressSlider.value = value;
            progressText.text = string.Format(_loadingFormatText, (value * 100f).ToString("F1"));
        }

        public void Dispose()
        {
            _progress?.Dispose();
        }
    }
}
