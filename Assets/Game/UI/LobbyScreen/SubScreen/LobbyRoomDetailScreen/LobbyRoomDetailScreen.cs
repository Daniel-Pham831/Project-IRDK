using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Game.CloudProfileSystem;
using Game.Networking;
using Game.Networking.Lobby;
using Game.Networking.Lobby.Commands;
using Game.Networking.Network;
using Game.Networking.Relay;
using Game.Networking.Scripts;
using Game.Scripts;
using Maniac.DataBaseSystem;
using Maniac.LanguageTableSystem;
using Maniac.UISystem;
using Maniac.Utils;
using TMPro;
using UniRx;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine.UI;

namespace Game
{
    public class LobbyRoomDetailScreen : BaseUI
    {
        private RelaySystem _relaySystem => Locator<RelaySystem>.Instance;
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private NetPlayer _netPlayer => Locator<NetPlayer>.Instance;
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

            _netPlayer.PingInMilliSeconds.Subscribe(value =>
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
            
            var regionId = _joinedLobby.Data[LobbyDataKey.LobbyRegion]?.Value;
            var region = _relaySystem.GetRegion(regionId);
            lobbyRegionTxt.text = region.Description;
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
            await new LeaveLobbyCommand().Execute();
            base.Back();
        }

        public async void OnSettingClicked()
        {
            
        }

        public async void OnAccountClicked()
        {
            
        }
    }
}
