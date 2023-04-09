using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Maniac.LanguageTableSystem;
using Maniac.UISystem;
using Maniac.Utils;
using TMPro;
using UniRx;
using UnityEngine.UI;

namespace Game
{
    public class LoadingScreen : BaseUI , IDisposable
    {
        [SerializeField] private Slider progressBarSlider;
        [SerializeField] private TMP_Text loadingText;
        [SerializeField] private LanguageItem languageItem;
        private string _loadingFormat;
        private FloatReactiveProperty _progress;

        public override void OnSetup(object parameter)
        {
            base.OnSetup(parameter);
            _progress = (FloatReactiveProperty)parameter;
            
            _loadingFormat = languageItem.GetCurrentLanguageText();
            progressBarSlider.value = 0;
            UpdateLoadingProgress(0f);
            _progress.Subscribe(UpdateLoadingProgress).AddTo(this);
        }

        private void UpdateLoadingProgress(float value)
        {
            progressBarSlider.value = value;
            loadingText.text = string.Format(_loadingFormat, (value * 100f).ToString("F1"));
        }

        public void Dispose()
        {
            _progress?.Dispose();
        }
    }
}
