using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Game.Scripts
{
    public class LobbyControllerInScreen : MonoBehaviour
    {
        [SerializeField] private LobbyItemInScreen lobbyItemPrefab;
        [SerializeField] private Transform lobbyItemsHolder;

        private List<LobbyItemInScreen> availableItems = new List<LobbyItemInScreen>();

        public void UpdateLobbies(List<Lobby> responseResults)
        {
            throw new System.NotImplementedException();
        }
    }
}