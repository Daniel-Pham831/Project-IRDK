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
        
        private Lobby _lobby;

        public override void OnSetup(object parameter = null)
        {
            _lobbyConfig = _dataBase.Get<LobbyConfig>();
            _lobby = _lobbySystem.GetCorrectLobby();

            SetupAllColorPickerButtons();
            
            UpdateLobbyRoom();
            
            base.OnSetup(parameter);
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
            if (_lobby == null)
            {
                Debug.LogError("Lobby Room cannot be null. Please Investigate!");
                return;
            }

            roomName.text = _lobby.Name;
            roomCode.text = _lobby.LobbyCode;

            var isHost = _lobby.HostId == LocalData.LocalPlayer.Id;
            startOrReady.text = isHost
                ? startLangItem.GetCurrentLanguageText()
                : readyLangItem.GetCurrentLanguageText();

            playerItemController.UpdateLobbyPlayerItems(_lobby);
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
