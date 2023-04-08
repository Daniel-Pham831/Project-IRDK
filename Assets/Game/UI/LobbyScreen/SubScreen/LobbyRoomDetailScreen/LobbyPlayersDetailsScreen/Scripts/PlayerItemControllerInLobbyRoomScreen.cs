using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Game.Scripts
{
    public class PlayerItemControllerInLobbyRoomScreen : MonoBehaviour
    {
        [SerializeField] private List<PlayerItemInLobbyRoomScreen> playerItems;

        public void UpdateLobbyPlayerItems(Lobby lobbyInfo)
        {
            for (int i = 0; i < playerItems.Count; i++)
            {
                var isContainPlayer = i < lobbyInfo.Players.Count;
                
                playerItems[i].gameObject.SetActive(isContainPlayer);
                if (isContainPlayer)
                {
                    playerItems[i].UpdateInfo(lobbyInfo.Players[i], lobbyInfo);
                }
            }
        }
    }
}