using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Game.CloudProfileSystem;
using Game.Networking;
using Game.Networking.Lobby;
using Game.Networking.Lobby.Commands;
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
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private LobbyConfig _lobbyConfig;

        [SerializeField] private PlayerItemControllerInLobbyRoomScreen playerItemController;

        [SerializeField] private TMP_Text roomName;
        [SerializeField] private TMP_Text roomCode;

        [SerializeField] private GameObject startBtn;
        [SerializeField] private TMP_Text startOrReady;

        [SerializeField] private LanguageItem startLangItem;
        private Lobby _joinedLobby;


        public override async void OnSetup(object parameter = null)
        {
            _lobbyConfig = _dataBase.Get<LobbyConfig>();

            SubscribeLobby();
            
            base.OnSetup(parameter);
        }

        private void SubscribeLobby()
        {
            _lobbySystem.JoinedLobby.Subscribe(value =>
            {
                if (_lobbySystem.JoinedLobby.Value == null)  return;

                _joinedLobby = _lobbySystem.JoinedLobby.Value;
                UpdateLobbyRoom();
            }).AddTo(this);
        }

        private void UpdateLobbyRoom()
        {
            roomName.text = _joinedLobby.Name;
            roomCode.text = _joinedLobby.LobbyCode;

            var isHost = _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
            
            startBtn.SetActive(isHost);
            startOrReady.text = startLangItem.GetCurrentLanguageText();

            playerItemController.UpdateLobbyPlayerItems(_joinedLobby);
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
    }
}
