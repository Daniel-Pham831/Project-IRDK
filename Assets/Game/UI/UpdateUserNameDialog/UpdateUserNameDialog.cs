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

namespace Game
{
    public class UpdateUserNameDialog : BaseUI
    {
        private LocalSystem _localSystem => Locator<LocalSystem>.Instance;
        
        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private TMP_Text welcomeText;

        [SerializeField] private LanguageItem welcomeLangItem;
        [SerializeField] private LanguageItem unknownLangItem;
        
        public override void OnSetup(object parameter = null) //first
        {
            welcomeText.text = string.Format(welcomeLangItem.GetCurrentLanguageText(),
                unknownLangItem.GetCurrentLanguageText());
            
            base.OnSetup(parameter);
        }

        public async void OnConfirmClicked()
        {
            var name = nameInput.text.Trim();
            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
            {
                ShowErrorInInputField();
            }
            else
            {
                await SubmitNameToServer(name);
            }
        }

        private async UniTask SubmitNameToServer(string name)
        {
            await new ShowConnectToServerCommand().Execute();
            await new SaveCsDataCommand(CloudSaveKey.UserName, name).Execute();
            await new HideConnectToServerCommand().Execute();
            _localSystem.LocalPlayer.DisplayName = name;
            Close(name);
        }

        private void ShowErrorInInputField()
        {
            nameInput.transform.DOPunchScale(Vector3.one * 0.05f, 0.1f);
        }
    }
}
