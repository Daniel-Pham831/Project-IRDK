using System;
using Cysharp.Threading.Tasks;
using Game.Networking;
using Game.Networking.Lobby.Commands;
using Game.Networking.Network.NetworkModels;
using Game.Networking.Network.NetworkModels.Handlers;
using Maniac.Utils;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class PlayerItemInLobbyRoomScreen : MonoBehaviour
    {
        private NetModelHub _netModelHub => Locator<NetModelHub>.Instance;
        private NetPlayerModelHandler _NetPlayerModelHandler;

        [SerializeField] private TMP_Text playerName;
        [SerializeField] private GameObject kickButton;
        [SerializeField] private Image playerSlotImage;

        [SerializeField] private Color localColor;
        [SerializeField] private Color normalColor;
        
        private Lobby _lobby;
        private Player _lobbyPlayer;

        public void UpdateInfo(Player lobbyPlayer, Lobby lobby)
        {
            _NetPlayerModelHandler ??= _netModelHub.GetHandler<NetPlayerModelHandler>();
            _lobbyPlayer = lobbyPlayer;
            _lobby = lobby;
            
            var isLocalPlayerSlot = lobbyPlayer.Id == AuthenticationService.Instance.PlayerId;
            var isLocalPlayerHost = AuthenticationService.Instance.PlayerId == lobby.HostId;

            kickButton.SetActive(isLocalPlayerHost && !isLocalPlayerSlot);
            playerSlotImage.color = isLocalPlayerSlot ? localColor : normalColor;

            var netPlayerModel = _NetPlayerModelHandler.GetModelByPlayerId(lobbyPlayer.Id);

            playerName.text = netPlayerModel?.Name ?? "";
        }

        public async void OnKickPlayerClicked()
        {
            await new KickPlayerFromLobbyCommand(_lobby.Id, _lobbyPlayer.Id).Execute();
        }
    }
}