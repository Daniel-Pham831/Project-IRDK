using System.Collections;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Game.CloudProfileSystem;
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
        private CloudProfileManager _cloudProfileManager => Locator<CloudProfileManager>.Instance;
        private UserProfile _userProfile;

        [Header("Welcome Dialog")] 
        [SerializeField] private TMP_Text welcomeText;
        [SerializeField] private CanvasGroup aboveWelcomeDialog;
        [SerializeField] private LanguageItem welcomeLangItem;

        public override async void OnSetup(object parameter = null)
        {
            _userProfile = await _cloudProfileManager.Get<UserProfile>();
            
            aboveWelcomeDialog.alpha = 0;
            SetInteraction(false);
            
            base.OnSetup(parameter);
        }

        public override async void OnShow(object parameter = null)
        {
            await CheckPlayerName();
            SetInteraction(true);
            await UniTask.Delay(1000); 
            await ShowAboveWelcomeDialog();
        }

        private async UniTask ShowAboveWelcomeDialog()
        {
            welcomeText.text = string.Format(welcomeLangItem.GetCurrentLanguageText(),
                _userProfile.DisplayName);

            await aboveWelcomeDialog.DOFade(1, 0.4f).AsyncWaitForCompletion();
            await UniTask.Delay(3000); 
            aboveWelcomeDialog.DOFade(0, 0.4f);
        }

        public async UniTask CheckPlayerName()
        {
            bool hasUserHaveName = _userProfile.DisplayName != string.Empty;
            if (hasUserHaveName) return;

            await new ShowScreenCommand<UpdateUserNameDialog>().ExecuteAndReturnResult();
        }

        public async void OnPlaySingleClicked()
        {
            
        }

        public async void OnPlayMultiClicked()
        {
            await _uiManager.Show<LobbyScreen>();
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
