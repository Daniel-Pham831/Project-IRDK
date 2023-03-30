using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
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
        private LocalSystem _localSystem => Locator<LocalSystem>.Instance;
        
        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private TMP_Text welcomeText;

        [SerializeField] private LanguageItem welcomeLangItem;
        [SerializeField] private LanguageItem unknownLangItem;

        [SerializeField] private Button closeButton;

        public override void OnSetup(object parameter = null) //first
        {
            bool hasUserHaveName = !string.IsNullOrEmpty(_localSystem.LocalPlayer.DisplayName);

            welcomeText.text = string.Format(welcomeLangItem.GetCurrentLanguageText(),
                hasUserHaveName ? _localSystem.LocalPlayer.DisplayName : unknownLangItem.GetCurrentLanguageText());

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
            await new SaveCsDataCommand(CloudSaveKey.UserName, nameToSubmit).Execute();
            await new HideConnectToServerCommand().Execute();
            _localSystem.LocalPlayer.DisplayName = nameToSubmit;
            Close(nameToSubmit);
        }

        private void ShowErrorInInputField()
        {
            nameInput.transform.DOPunchScale(Vector3.one * 0.05f, 0.1f);
        }
    }
}
