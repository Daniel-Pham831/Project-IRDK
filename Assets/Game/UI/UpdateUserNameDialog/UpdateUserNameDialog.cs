using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Game.CloudProfileSystem;
using Game.Commands;
using Game.Networking;
using Game.Services.UnityServices;
using Maniac.LanguageTableSystem;
using Maniac.UISystem;
using Maniac.Utils;
using TMPro;
using UnityEngine.UI;

namespace Game
{
    public class UpdateUserNameDialog : BaseUI
    {
        private CloudProfileManager _cloudProfileManager => Locator<CloudProfileManager>.Instance;
        private UserProfile _userProfile;
        
        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private TMP_Text welcomeText;

        [SerializeField] private LanguageItem welcomeLangItem;
        [SerializeField] private LanguageItem unknownLangItem;

        [SerializeField] private Button closeButton;

        public override async void OnSetup(object parameter = null) //first
        {
            _userProfile = await _cloudProfileManager.Get<UserProfile>();
            
            bool hasUserHaveName = !string.IsNullOrEmpty(_userProfile.DisplayName);

            welcomeText.text = string.Format(welcomeLangItem.GetCurrentLanguageText(),
                hasUserHaveName ? _userProfile.DisplayName : unknownLangItem.GetCurrentLanguageText());

            closeButton.gameObject.SetActive(hasUserHaveName);
            base.OnSetup(parameter);
        }

        public async void OnConfirmClicked()
        {
            var trimmedName = nameInput.text.Trim();
            if (string.IsNullOrEmpty(trimmedName) || string.IsNullOrWhiteSpace(trimmedName))
            {
                ShowErrorInInputField();
            }
            else
            {
                await SubmitNameToServer(trimmedName);
            }
        }
        
        private async UniTask SubmitNameToServer(string nameToSubmit)
        {
            await new ShowConnectToServerCommand().Execute();
            _userProfile.DisplayName = nameToSubmit;
            await _userProfile.Save();
            await new HideConnectToServerCommand().Execute();
            Close(nameToSubmit);
        }

        private void ShowErrorInInputField()
        {
            nameInput.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f);
        }
    }
}
