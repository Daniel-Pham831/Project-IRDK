using System;
using Cysharp.Threading.Tasks;
using Unity.Services.Lobbies;
using UnityEngine;
using Lobby = Unity.Services.Lobbies.Models.Lobby;


namespace Game.Networking.LobbySystem
{
    public class LobbySystem
    {
        private ILobbyService _lobbyService;

        public async UniTask Init()
        {
            _lobbyService = LobbyService.Instance;
            await UniTask.CompletedTask;
        }

        public async UniTask<Unity.Services.Lobbies.Models.Lobby> CreateLobby(string lobbyName = "",int maxPlayer = 4)
        {
            try
            {
                lobbyName += $" #{UnityEngine.Random.Range(0, 10000)}";

                var newLobby = await _lobbyService.CreateLobbyAsync(lobbyName, maxPlayer);

                return newLobby;
            }
            catch (Exception e)
            {
                // ignored
                Debug.LogError(e);
                return null;
            }
        }
    }
}