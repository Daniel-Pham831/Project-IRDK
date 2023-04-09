using System;
using Cysharp.Threading.Tasks;
using Game.Networking;
using Game.Networking.Lobby.Commands;
using Game.Networking.NetPlayerComponents;
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
        private NetPlayerModelHandler _NetPlayerModelHandler => Locator<NetPlayerModelHandler>.Instance;

        [SerializeField] private TMP_Text playerName;
        [SerializeField] private GameObject kickButton;
        [SerializeField] private Image playerSlotImage;

        [SerializeField] private Color hostColor;
        [SerializeField] private Color normalColor;
        
        private Lobby _lobby;
        private Player _lobbyPlayer;

        public void UpdateInfo(Player lobbyPlayer, Lobby lobby)
        {
            _lobbyPlayer = lobbyPlayer;
            _lobby = lobby;
            
            var isHostSlot = lobbyPlayer.Id == lobby.HostId;
            var isLocalPlayerHost = AuthenticationService.Instance.PlayerId == lobby.HostId;

            kickButton.SetActive(isLocalPlayerHost && !isHostSlot);
            playerSlotImage.color = isHostSlot ? hostColor : normalColor;

            var netPlayerModel = _NetPlayerModelHandler.GetModelByPlayerId(lobbyPlayer.Id);

            playerName.text = (netPlayerModel?.Name ?? "").ToString();
            var isLocalPlayer = lobbyPlayer.Id == AuthenticationService.Instance.PlayerId ;
        }

        public async void OnKickPlayerClicked()
        {
            await new KickPlayerFromLobbyCommand(_lobby.Id, _lobbyPlayer.Id).Execute();
        }
    }
}