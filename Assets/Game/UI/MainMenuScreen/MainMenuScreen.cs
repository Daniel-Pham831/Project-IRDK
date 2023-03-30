using System.Collections;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Game.Networking;
using Maniac.LanguageTableSystem;
using Maniac.UISystem;
using Maniac.UISystem.Command;
using Maniac.Utils;
using TMPro;

namespace Game
{
    public class MainMenuScreen : BaseUI
    {
        private LocalSystem _localSystem => Locator<LocalSystem>.Instance;
        private UIManager _uiManager => Locator<UIManager>.Instance;

        [Header("Welcome Dialog")] 
        [SerializeField] private TMP_Text welcomeText;
        [SerializeField] private CanvasGroup aboveWelcomeDialog;
        [SerializeField] private LanguageItem welcomeLangItem;

        public override void OnSetup(object parameter = null)
        {
            aboveWelcomeDialog.alpha = 0;
            SetInteraction(false);
            
            base.OnSetup(parameter);
        }

        public override async void OnShow(object parameter = null)
        {
            await CheckPlayerName();
            SetInteraction(true);
            await UniTask.Delay(1500); 
            await ShowAboveWelcomeDialog();
        }

        private async UniTask ShowAboveWelcomeDialog()
        {
            welcomeText.text = string.Format(welcomeLangItem.GetCurrentLanguageText(),
                _localSystem.LocalPlayer.DisplayName);

            await aboveWelcomeDialog.DOFade(1, 0.4f).AsyncWaitForCompletion();
            await UniTask.Delay(3500); 
            aboveWelcomeDialog.DOFade(0, 0.4f);
        }

        public async UniTask CheckPlayerName()
        {
            bool hasUserHaveName = _localSystem.LocalPlayer.DisplayName != string.Empty;
            if (hasUserHaveName) return;

            await ShowScreenCommand.Create<UpdateUserNameDialog>().ExecuteAndReturnResult();
        }

        public async void OnPlaySingleClicked()
        {
            
        }

        public async void OnPlayMultiClicked()
        {
            
        }

        public async void OnSettingClicked()
        {
            
        }

        public async void OnShowAccountDetailsClicked()
        {
            await _uiManager.Show<AccountDetailsScreen>();
        }
    }
}
