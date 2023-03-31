using System;
using Cysharp.Threading.Tasks;
using Maniac.DataBaseSystem;
using Maniac.TimeSystem;
using Maniac.Utils;
using Maniac.Utils.Extension;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Lobby = Unity.Services.Lobbies.Models.Lobby;


namespace Game.Networking.LobbySystem
{
    public class LobbySystem
    {
        private TimeManager _timeManager => Locator<TimeManager>.Instance;
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private LobbyConfig _lobbyConfig;

        private ILobbyService _lobbyService;
        private Unity.Services.Lobbies.Models.Lobby _hostLobby = null;
        private Unity.Services.Lobbies.Models.Lobby _joinedLobby = null;

        public async UniTask Init()
        {
            _lobbyService = LobbyService.Instance;
            _lobbyConfig = _dataBase.Get<LobbyConfig>();
            
            HandleLobbyHeartBeat();
            await UniTask.CompletedTask;
        }

        private void HandleLobbyHeartBeat()
        {
            _timeManager.OnTimeOut(async () =>
            {
                if (_hostLobby != null)
                {
                    await _lobbyService.SendHeartbeatPingAsync(_hostLobby.Id);
                }

                HandleLobbyHeartBeat();
            },_lobbyConfig.HeartBeatIntervalInSeconds);
        }

        public async UniTask<Unity.Services.Lobbies.Models.Lobby> CreateLobby(string lobbyName = "",int maxPlayer = 4)
        {
            try
            {
                lobbyName += $" #{UnityEngine.Random.Range(0, 10000)}";

                var newLobby = await _lobbyService.CreateLobbyAsync(lobbyName, maxPlayer);

                Debug.Log($"Lobby {newLobby.Name}-{newLobby.LobbyCode} {"Created".AddColor(Color.yellow)}");
                _hostLobby = newLobby;
            }
            catch (LobbyServiceException e)
            {
                // ignored
                Debug.Log(e);
                _hostLobby = null;
            }

            return _hostLobby;
        }
        
        public async UniTask<QueryResponse> FetchLobbiesList(QueryLobbiesOptions options = default)
        {
            try
            {
                var queryResponse = await _lobbyService.QueryLobbiesAsync();
                Debug.Log($"{queryResponse.Results.Count.AddColor(Color.green)} Lobbies found.");

                if (queryResponse.Results.Count != 0)
                    return queryResponse;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }

            return null;
        }

        public async UniTask<Unity.Services.Lobbies.Models.Lobby> JoinLobbyByCode(string lobbyCode)
        {
            try
            {
                var joinedLobby = await _lobbyService.JoinLobbyByCodeAsync(lobbyCode);
                Debug.Log($"Joined Lobby {joinedLobby.Id} {joinedLobby.LobbyCode}");
                _joinedLobby = joinedLobby;
            }
            catch(LobbyServiceException e)
            {
                Debug.Log(e);
                _joinedLobby = null;
            }

            return _joinedLobby;
        }
        
        public async UniTask<Unity.Services.Lobbies.Models.Lobby> JoinLobbyById(string lobbyId)
        {
            try
            {
                var joinedLobby = await _lobbyService.JoinLobbyByIdAsync(lobbyId);
                Debug.Log($"Joined Lobby {joinedLobby.Id} {joinedLobby.LobbyCode}");
                _joinedLobby = joinedLobby;
            }
            catch(LobbyServiceException e)
            {
                Debug.Log(e);
                _joinedLobby = null;
            }
            
            return _joinedLobby;
        }
        
        public async UniTask<Unity.Services.Lobbies.Models.Lobby> QuickJoinLobby(QuickJoinLobbyOptions options = default)
        {
            try
            {
                var joinedLobby = await _lobbyService.QuickJoinLobbyAsync(options);
                Debug.Log($"Joined Lobby {joinedLobby.Id} {joinedLobby.LobbyCode}");
                _joinedLobby = joinedLobby;
            }
            catch(LobbyServiceException e)
            {
                Debug.Log(e);
                _joinedLobby = null;
            }
            
            return _joinedLobby;
        }
    }
}