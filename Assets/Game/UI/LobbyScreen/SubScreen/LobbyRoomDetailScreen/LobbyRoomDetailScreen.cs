using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Game.Networking.LobbySystem;
using Game.Scripts;
using Maniac.DataBaseSystem;
using Maniac.UISystem;
using Maniac.Utils;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;

namespace Game
{
    public class LobbyRoomDetailScreen : BaseUI
    {
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private LobbyConfig _lobbyConfig;

        [SerializeField] private List<PlayerItemInLobbyRoomScreen> playerItems;

        [SerializeField] private Button colorButtonPrefab;
        [SerializeField] private GameObject colorPickerHolder;
        [SerializeField] private TMP_Text roomName;
        [SerializeField] private TMP_Text roomCode;
        [SerializeField] private TMP_Text startOrReady;
        
        private Lobby _lobby;

        public override void OnSetup(object parameter = null)
        {
            _lobbyConfig = _dataBase.Get<LobbyConfig>();
            _lobby = _lobbySystem.GetCorrectLobby();

            UpdateLobbyRoom();
            
            base.OnSetup(parameter);
        }

        private void UpdateLobbyRoom()
        {
            if (_lobby == null)
            {
                Debug.LogError("Lobby Room cannot be null. Please Investigate!");
                return;
            }
            
            
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
