using System;
using Cysharp.Threading.Tasks;
using Maniac.Utils.Extension;
using Unity.Services.Lobbies;
using UnityEngine;
using Lobby = Unity.Services.Lobbies.Models.Lobby;


namespace Game.Networking.LobbySystem
{
    public class LobbySystem
    {
        private ILobbyService _lobbyService;
        private Unity.Services.Lobbies.Models.Lobby localLobby;

        public async UniTask Init()
        {
            _lobbyService = LobbyService.Instance;
            await UniTask.CompletedTask;
        }

        public async UniTask CreateLobby(string lobbyName = "",int maxPlayer = 4)
        {
            try
            {
                lobbyName += $" #{UnityEngine.Random.Range(0, 10000)}";

                var newLobby = await _lobbyService.CreateLobbyAsync(lobbyName, maxPlayer);

                Debug.Log($"Lobby {newLobby.Name}-{newLobby.Id} {"Created".AddColor(Color.yellow)}");
                localLobby = newLobby;
            }
            catch (Exception e)
            {
                // ignored
                Debug.LogError(e);
            }
        }
    }
}