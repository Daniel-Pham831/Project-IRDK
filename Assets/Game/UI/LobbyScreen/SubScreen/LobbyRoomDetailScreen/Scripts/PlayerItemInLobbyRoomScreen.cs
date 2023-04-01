﻿using Cysharp.Threading.Tasks;
using Game.Networking;
using Game.Networking.LobbySystem;
using Game.Networking.LobbySystem.Commands;
using Game.Networking.LobbySystem.Extensions;
using Maniac.Utils;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class PlayerItemInLobbyRoomScreen : MonoBehaviour
    {
        private LocalData _localData => Locator<LocalData>.Instance;
        [SerializeField] private GameObject isHostIcon;
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private GameObject isReadyIcon;
        [SerializeField] private GameObject kickButton;
        [SerializeField] private Image background;
        [SerializeField] private Image nameBorderColor;
        
        private Lobby _lobby;
        private Player _lobbyPlayer;

        public void UpdateInfo(Player lobbyPlayer, Lobby lobby)
        {
            _lobbyPlayer = lobbyPlayer;
            _lobby = lobby;
            
            var isHostSlot = lobbyPlayer.Id == lobby.HostId;
            var isLocalPlayerHost = _localData.LocalPlayer.Id == lobby.HostId;

            isHostIcon.SetActive(isHostSlot);
            kickButton.SetActive(isLocalPlayerHost && !isHostSlot);

            playerName.text = lobbyPlayer.GetData<string>(LobbyDataKey.PlayerName);
            isReadyIcon.SetActive(lobbyPlayer.GetData<bool>(LobbyDataKey.PlayerSlotReady));
            background.color = lobbyPlayer.GetData<Color>(LobbyDataKey.PlayerSlotColor);
            
            var isLocalPlayer = lobbyPlayer.Id == _localData.LocalPlayer.Id;
            nameBorderColor.color = isLocalPlayer ? Color.black : Color.white;
        }

        public async void OnKickPlayerClicked()
        {
            await new KickPlayerFromLobbyCommand(_lobby.Id, _lobbyPlayer.Id).Execute();
        }
    }
}