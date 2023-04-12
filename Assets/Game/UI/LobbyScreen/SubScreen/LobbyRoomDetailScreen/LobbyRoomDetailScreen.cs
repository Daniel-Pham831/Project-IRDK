using UnityEngine;
using Game.Commands;
using Game.Networking.Lobby;
using Game.Networking.Lobby.Commands;
using Game.Networking.NetPlayerComponents;
using Game.Networking.Relay;
using Maniac.DataBaseSystem;
using Maniac.LanguageTableSystem;
using Maniac.UISystem;
using Maniac.UISystem.Command;
using Maniac.Utils;
using TMPro;
using UniRx;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;

namespace Game
{
    public class LobbyRoomDetailScreen : BaseUI
    {
        private RelaySystem _relaySystem => Locator<RelaySystem>.Instance;
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private PingHandler _pingHandler => Locator<PingHandler>.Instance;
        private LobbyConfig _lobbyConfig;

        [SerializeField] private TMP_Text lobbyNameTxt;
        [SerializeField] private TMP_Text lobbyStateTxt;
        [SerializeField] private TMP_Text lobbyPlayerCountTxt;
        [SerializeField] private TMP_Text lobbyRegionTxt;
        [SerializeField] private TMP_Text lobbyCodeTxt;
        [SerializeField] private TMP_Text lobbyPingTxt;
        
        [SerializeField] private LanguageItem privateLangItem;
        [SerializeField] private LanguageItem publicLangItem;

        private readonly string _playerCountFormat = "{0}/{1}";
        private Lobby _joinedLobby;

        public override async void OnSetup(object parameter = null)
        {
            _lobbyConfig = _dataBase.Get<LobbyConfig>();

            SubscribeEvents();
            
            base.OnSetup(parameter);
        }

        private void SubscribeEvents()
        {
            _lobbySystem.JoinedLobby.Subscribe(value =>
            {
                if (_lobbySystem.JoinedLobby.Value == null)  return;

                _joinedLobby = _lobbySystem.JoinedLobby.Value;
                UpdateLobbyRoom();
            }).AddTo(this);

            _pingHandler.PingInMilliSeconds.Subscribe(value =>
            {
                lobbyPingTxt.text = $"{value:F1} ms";
            }).AddTo(this);
        }

        private void UpdateLobbyRoom()
        {
            lobbyNameTxt.text = _joinedLobby.Name;
            lobbyPlayerCountTxt.text =
                string.Format(_playerCountFormat, _joinedLobby.Players.Count, _joinedLobby.MaxPlayers);
            lobbyCodeTxt.text = $"CODE\n{_joinedLobby.LobbyCode}";
            lobbyStateTxt.text = _joinedLobby.IsPrivate
                ? privateLangItem.GetCurrentLanguageText()
                : publicLangItem.GetCurrentLanguageText();

            var regionDescription = string.Empty;
            if (_joinedLobby.Data.TryGetValue(LobbyDataKey.LobbyRegion, out var dataObject))
            {
                regionDescription = _relaySystem.GetRegion(dataObject.Value)?.Description;
            }
            
            lobbyRegionTxt.text = regionDescription;
        }

        public async void OnStartClicked()
        {
            bool isLocalPlayerHost = _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;

            if (isLocalPlayerHost)
            {
                
            }
        }
        
        public override async void Back()
        {
            var yes = await new ShowConfirmationDialogCommand(LanguageTable.Confirmation_LeaveRoomHeader,
                LanguageTable.Confirmation_LeaveRoomBody).ExecuteAndGetResult();
            
            if(yes)
                await new LeaveLobbyCommand().Execute();
        }

        public async void OnSettingClicked()
        {
            
        }

        public async void OnAccountClicked()
        {
        }

        public async void OnLobbyPlayersDetailsClicked()
        {
            await new ShowScreenCommand<LobbyPlayersDetailsScreen>().Execute();
        }
    }
}
