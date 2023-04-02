using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Game.Commands;
using Game.Networking.LobbySystem;
using Game.Networking.LobbySystem.Commands;
using Game.Scripts;
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

        [SerializeField] private TMP_InputField joinCodeInput;

        public override async void OnSetup(object parameter = null)
        {
            await _lobbySystem.ResetJoinedLobby();
        }

        public async void OnCreateClicked()
        {
            Debug.Log("OnCreateClicked");
            await new CreateNewLobbyCommand(Back, () => { }).Execute();
        }

        public async void OnJoinByCodeClicked()
        {
            Debug.Log("OnJoinClicked");
            if (!string.IsNullOrEmpty(joinCodeInput.text))
            {
                await new JoinLobbyByCodeCommand(joinCodeInput.text, Back, () => { }).Execute();
            }
        }

        public async void OnQuickJoinClicked()
        {
            Debug.Log("OnQuickJoinClicked");
            await new QuickJoinLobbyCommand(Back, () => { }).Execute();
        }

        
    }
}
