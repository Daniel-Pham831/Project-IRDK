using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Game.Commands;
using Game.Networking.Lobby;
using Game.Networking.Lobby.Commands;
using Game.Scripts;
using Maniac.LanguageTableSystem;
using Maniac.UISystem;
using Maniac.UISystem.Command;
using Maniac.Utils;
using TMPro;
using UniRx.Triggers;

namespace Game
{
    public class LobbyScreen : BaseUI
    {
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;
        private LanguageTable _languageTable => Locator<LanguageTable>.Instance;

        [SerializeField] private TMP_InputField joinCodeInput;

        public override async void OnSetup(object parameter = null)
        {
            await _lobbySystem.ResetJoinedLobby();
        }

        public async void OnCreateClicked()
        {
            Debug.Log("OnCreateClicked");
            await new CreateNewLobbyCommand(Back, async () =>
            {
                await ShowCreateLobbyFail();
            }).Execute();
            
        }

        public async void OnJoinByCodeClicked()
        {
            Debug.Log("OnJoinClicked");
            if (!string.IsNullOrEmpty(joinCodeInput.text))
            {
                await new JoinLobbyByCodeCommand(joinCodeInput.text, Back, async () =>
                {
                    await ShowJoinFail();
                }).Execute();
            }
            else
            {
                joinCodeInput.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f);
            }
        }

        public async void OnQuickJoinClicked()
        {
            Debug.Log("OnQuickJoinClicked");
            await new QuickJoinLobbyCommand(Back, async () =>
            {
                await ShowJoinFail();
            }).Execute();
        }

        private async UniTask ShowCreateLobbyFail()
        {
            await new ShowInformationDialogCommand(LanguageTable.Information_FailToCreateLobbyHeader,
                LanguageTable.Information_FailToCreateLobbyBody).Execute();
        }
        
        private async UniTask ShowJoinFail()
        {
            await new ShowInformationDialogCommand(LanguageTable.Information_FailToJoinLobbyHeader,
                LanguageTable.Information_FailToJoinLobbyBody).Execute();
        }
    }
}
