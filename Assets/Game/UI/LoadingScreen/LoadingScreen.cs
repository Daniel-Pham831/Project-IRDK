using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Maniac.LanguageTableSystem;
using Maniac.UISystem;
using Maniac.Utils;
using TMPro;
using UnityEngine.UI;

namespace Game
{
    public class LoadingScreen : BaseUI
    {
        [SerializeField] private Slider progressBarSlider;
        [SerializeField] private TMP_Text loadingText;
        [SerializeField] private LanguageItem languageItem;
        private string _loadingFormat;

        public override void OnSetup(object parameter)
        {
            base.OnSetup(parameter);

            _loadingFormat = languageItem.GetCurrentLanguageText();
            progressBarSlider.value = 0;
            UpdateLoadingText(0f);
        }

        private void UpdateLoadingText(float value)
        {
            loadingText.text = string.Format(_loadingFormat, value);
        }

        public override async UniTask Close(object param = null)
        {
            await UniTask.Delay(300);
            await base.Close(param);
        }

        public void UpdateProgressBar(float percent)
        {
            progressBarSlider.DOValue(percent, 0.05f);
            UpdateLoadingText((percent * 100));
        }
    }
}
