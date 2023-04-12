using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Game.Commands;
using Game.Networking.Lobby;
using Game.Networking.Lobby.Commands;
using Game.Scripts;
using Maniac.DataBaseSystem;
using Maniac.LanguageTableSystem;
using Maniac.TimeSystem;
using Maniac.UISystem;
using Maniac.UISystem.Command;
using Maniac.Utils;
using TMPro;
using UniRx.Triggers;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;

namespace Game
{
    public class LobbyScreen : BaseUI
    {
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;
        private LanguageTable _languageTable => Locator<LanguageTable>.Instance;
        private TimeManager _timeManager => Locator<TimeManager>.Instance;
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private LobbyConfig _lobbyConfig;

        [SerializeField] private TMP_InputField joinCodeInput;
        [SerializeField] private LobbyItemControllerInLobbyScreen lobbyItemController;
        [SerializeField] private Button refreshButton;

        public override async void OnSetup(object parameter = null)
        {
            _lobbyConfig = _dataBase.GetConfig<LobbyConfig>();
            await _lobbySystem.Reset();
            lobbyItemController.Init(OnJoinLobbyClicked);
            
            refreshButton.onClick.AddListener(async () =>
            {
                refreshButton.interactable = false;
                await OnRefreshClicked();
                _timeManager.OnTimeOut(() =>
                {
                    if (this == null || refreshButton == null) return;
                    
                    refreshButton.interactable = true;
                },_lobbyConfig.RefreshIntervelInSeconds);
            });
        }

        private async void OnJoinLobbyClicked(Lobby lobbyToJoin)
        {
            await new JoinLobbyCommand(lobbyToJoin.Id,true).Execute();
        }

        public async void OnCreateClicked()
        {
            Debug.Log("OnCreateClicked");
            await new CreateNewLobbyCommand().Execute();
        }

        public async void OnJoinByCodeClicked()
        {
            Debug.Log("OnJoinClicked");
            if (!string.IsNullOrEmpty(joinCodeInput.text))
            {
                await new JoinLobbyCommand(joinCodeInput.text,false).Execute();
            }
            else
            {
                joinCodeInput.transform.DOPunchScale(Vector3.one * 0.1f, 0.1f);
            }
        }

        private async UniTask OnRefreshClicked()
        {
            await new ShowConnectToServerCommand().Execute();
            await _lobbySystem.QueryLobbies();
            await new HideConnectToServerCommand().Execute();
        }

    }
}
