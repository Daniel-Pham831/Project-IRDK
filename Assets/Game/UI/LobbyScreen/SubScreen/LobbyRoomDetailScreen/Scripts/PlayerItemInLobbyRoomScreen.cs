using Cysharp.Threading.Tasks;
using Game.Networking;
using Game.Networking.LobbySystem;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class PlayerItemInLobbyRoomScreen : MonoBehaviour
    {
        [SerializeField] private GameObject isHostIcon;
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private GameObject isReadyIcon;
        [SerializeField] private GameObject kickButton;
        [SerializeField] private Image background;

        public void UpdateInfo(Player lobbyPlayer, Lobby lobby)
        {
            var isHost = lobbyPlayer.Id == lobby.HostId;
            
            isHostIcon.gameObject.SetActive(isHost);
            kickButton.gameObject.SetActive(isHost);

            playerName.text = lobbyPlayer.Data[LobbyDataKey.PlayerName].Value;
            
            var isReady = (lobbyPlayer.Data[LobbyDataKey.PlayerReady] as PlayerDataObject<bool>)?.TValue;
            isReadyIcon.gameObject.SetActive(isReady != null && isReady.Value);
            
            var color = (lobbyPlayer.Data[LobbyDataKey.PlayerReady] as PlayerDataObject<Color>)?.TValue;
            if (color != null)
                background.color = color.Value;
        }
    }
}