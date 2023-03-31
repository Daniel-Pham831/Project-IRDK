using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Game.Networking;
using Game.Networking.LobbySystem;
using Game.Scripts;
using Maniac.DataBaseSystem;
using Maniac.LanguageTableSystem;
using Maniac.UISystem;
using Maniac.Utils;
using TMPro;
using UniRx;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;

namespace Game
{
    public class LobbyRoomDetailScreen : BaseUI
    {
        private LocalData LocalData => Locator<LocalData>.Instance;
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
                
                UpdateLobbyRoom();
            }).AddTo(this);
        }

        private void SetupAllColorPickerButtons()
        {
            foreach (var color in _lobbyConfig.RoomColors)
            {
                var newButton = Instantiate(colorButtonPrefab, colorPickerHolder);
                newButton.GetComponent<Image>().color = color;
                newButton.onClick.AddListener(()=>
                {
                    ApplyColorToLocalPlayer(color);
                });
            }
        }

        private void ApplyColorToLocalPlayer(Color color)
        {
            throw new System.NotImplementedException();
        }

        private void UpdateLobbyRoom()
        {
            roomName.text = _lobbySystem.JoinedLobby.Value.Name;
            roomCode.text = _lobbySystem.JoinedLobby.Value.LobbyCode;

            var isHost = _lobbySystem.JoinedLobby.Value.HostId == LocalData.LocalPlayer.Id;
            startOrReady.text = isHost
                ? startLangItem.GetCurrentLanguageText()
                : readyLangItem.GetCurrentLanguageText();

            playerItemController.UpdateLobbyPlayerItems(_lobbySystem.JoinedLobby.Value);
        }

        public async void OnStartOrReadyClicked()
        {
            Debug.Log("OnStartOrReadyClicked");
        }

        public async void OnKickPlayerClicked()
        {
            
        }
    }
}
