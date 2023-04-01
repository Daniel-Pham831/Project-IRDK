using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Game.Networking;
using Game.Networking.LobbySystem;
using Game.Networking.LobbySystem.Commands;
using Game.Networking.LobbySystem.Extensions;
using Game.Scripts;
using Maniac.DataBaseSystem;
using Maniac.LanguageTableSystem;
using Maniac.UISystem;
using Maniac.Utils;
using TMPro;
using UniRx;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine.UI;

namespace Game
{
    public class LobbyRoomDetailScreen : BaseUI
    {
        private LocalData _localData => Locator<LocalData>.Instance;
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private LobbyConfig _lobbyConfig;

        [SerializeField] private PlayerItemControllerInLobbyRoomScreen playerItemController;

        [SerializeField] private Button colorButtonPrefab;
        [SerializeField] private Transform colorPickerHolder;
        [SerializeField] private TMP_Text roomName;
        [SerializeField] private TMP_Text roomCode;
        [SerializeField] private TMP_Text startOrReady;

        [SerializeField] private LanguageItem startLangItem;
        [SerializeField] private LanguageItem readyLangItem;
        private Lobby _joinedLobby;


        public override void OnSetup(object parameter = null)
        {
            _lobbyConfig = _dataBase.Get<LobbyConfig>();

            SubscribeLobby();
            SetupAllColorPickerButtons();
            
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

        private void SetupAllColorPickerButtons()
        {
            foreach (var color in _lobbyConfig.RoomColors)
            {
                var newButton = Instantiate(colorButtonPrefab, colorPickerHolder);
                newButton.GetComponent<Image>().color = color;
                newButton.onClick.AddListener(async ()=>
                {
                    await ApplyColorToLocalPlayer(color);
                });
            }
        }

        private async UniTask ApplyColorToLocalPlayer(Color color)
        {
            await new UpdatePlayerDataCommand(_joinedLobby.Id, _localData.LocalPlayer.Id,
                LobbyDataKey.PlayerSlotColor, color).Execute();
        }

        private void UpdateLobbyRoom()
        {
            roomName.text = _joinedLobby.Name;
            roomCode.text = _joinedLobby.LobbyCode;

            var isHost = _joinedLobby.HostId == _localData.LocalPlayer.Id;
            startOrReady.text = isHost
                ? startLangItem.GetCurrentLanguageText()
                : readyLangItem.GetCurrentLanguageText();

            playerItemController.UpdateLobbyPlayerItems(_joinedLobby);
        }

        public async void OnStartOrReadyClicked()
        {
            bool isLocalPlayerHost = _joinedLobby.HostId == _localData.LocalPlayer.Id;

            if (isLocalPlayerHost)
            {
                
            }
            else
            {
                var localPlayer = _joinedLobby.GetLocalPlayer();
                if (localPlayer != null)
                {
                    await new UpdatePlayerDataCommand(_joinedLobby.Id, _localData.LocalPlayer.Id,
                        LobbyDataKey.PlayerSlotReady, !localPlayer.GetData<bool>(LobbyDataKey.PlayerSlotReady)).Execute();
                }
            }
        }
    }
}
