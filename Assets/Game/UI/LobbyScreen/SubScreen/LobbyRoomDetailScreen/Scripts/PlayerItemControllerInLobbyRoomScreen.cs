using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Game.Scripts
{
    public class PlayerItemControllerInLobbyRoomScreen : MonoBehaviour
    {
        [SerializeField] private List<PlayerItemInLobbyRoomScreen> playerItems;

        public async UniTask Setup(Lobby lobbyInfo)
        {
        }
    }
}