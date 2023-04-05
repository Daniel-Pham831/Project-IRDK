using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.CloudProfileSystem;
using Game.Networking.Lobby.Commands;
using Game.Networking.Lobby.Extensions;
using Game.Networking.Lobby.Models;
using Maniac.DataBaseSystem;
using Maniac.TimeSystem;
using Maniac.Utils;
using Maniac.Utils.Extension;
using UniRx;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Game.Networking.Lobby
{
    public class LobbySystem
    {
        private CloudProfileManager _cloudProfileManager => Locator<CloudProfileManager>.Instance;
        private UserProfile _userProfile;
        private TimeManager _timeManager => Locator<TimeManager>.Instance;
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private LobbyConfig _lobbyConfig;

        private ILobbyService _lobbyService;
        private HandleLocalPlayerBecomeHostCommand _handleBecomeHostCommand;
        private HandleBeingKickedCommand _handleBeingKickCommand;

        public ReactiveProperty<Unity.Services.Lobbies.Models.Lobby> HostLobbyToPing { get; private set; } =
            new ReactiveProperty<Unity.Services.Lobbies.Models.Lobby>();

        public ReactiveProperty<Unity.Services.Lobbies.Models.Lobby> JoinedLobby { get; private set; } =
            new ReactiveProperty<Unity.Services.Lobbies.Models.Lobby>();

        public ReactiveProperty<QueryResponse> QueryResponse { get; set; } = new ReactiveProperty<QueryResponse>();

        public async UniTask Init()
        {
            _userProfile = await _cloudProfileManager.Get<UserProfile>();
            _lobbyService = LobbyService.Instance;
            _lobbyConfig = _dataBase.Get<LobbyConfig>();

            InitCommands();

            Reset();
            HandleLobbyHeartBeat();
            HandleJoinedLobbyUpdate();
            HandleQueryLobbiesUpdate();
            await UniTask.CompletedTask;
        }

        private void InitCommands()
        {
            _handleBecomeHostCommand = new HandleLocalPlayerBecomeHostCommand();
            _handleBeingKickCommand = new HandleBeingKickedCommand();
        }
        
        private void HandleQueryLobbiesUpdate()
        {
            _timeManager.OnTimeOut(async () =>
            {
                if (QueryResponse.Value != null)
                {
                    await QueryLobbies();
                }

                HandleQueryLobbiesUpdate();
            }, _lobbyConfig.LobbiesQueryIntervalInSeconds);
        }

        public async UniTask StartQuery()
        {
            await QueryLobbies();
        }

        public async UniTask StopQuery()
        {
            QueryResponse.Value = null;
        }
        
        public async UniTask QueryLobbies()
        {
            try
            {
                QueryResponse.Value = await _lobbyService.QueryLobbiesAsync(LobbyHelper.QueryLobbiesOptions);
            }
            catch
            {
                QueryResponse.Value = null;
            }
        }

        private void HandleJoinedLobbyUpdate()
        {
            _timeManager.OnTimeOut(async () =>
            {
                if (JoinedLobby.Value != null)
                {
                    try
                    {
                        JoinedLobby.Value = await _lobbyService.GetLobbyAsync(JoinedLobby.Value.Id);
                        await _handleBecomeHostCommand.Execute();
                    }
                    catch
                    {
                        JoinedLobby.Value = null;
                        await _handleBeingKickCommand.Execute();
                    }
                }

                HandleJoinedLobbyUpdate();
            }, _lobbyConfig.LobbyUpdateIntervalInSeconds);
        }

        public async UniTask Reset()
        {
            HostLobbyToPing.Value = null;
            JoinedLobby.Value = null;
        }

        private void HandleLobbyHeartBeat()
        {
            _timeManager.OnTimeOut(async () =>
            {
                if (HostLobbyToPing.Value != null)
                {
                    await _lobbyService.SendHeartbeatPingAsync(HostLobbyToPing.Value.Id);
                    Debug.Log($"Heartbeat {HostLobbyToPing.Value.Name} Sent");
                }

                HandleLobbyHeartBeat();
            }, _lobbyConfig.HeartBeatIntervalInSeconds);
        }

        public async UniTask<bool> KickPlayerFromLobby(string lobbyId, string playerId)
        {
            var isHost = AmITheHost();
            if (isHost)
            {
                try
                {
                    await _lobbyService.RemovePlayerAsync(lobbyId, playerId);
                    return true;
                }
                catch
                {
                    // ignored
                }
            }

            return false;
        }

        public async UniTask UpdatePlayerData(string key, string data)
        {
            try
            {
                if (JoinedLobby.Value == null) return;
                //
                // await _lobbyService.UpdatePlayerAsync(JoinedLobby.Value.Id,AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions(){})
            }
            catch
            {
                // ignored
            }
        }

        public async UniTask<Unity.Services.Lobbies.Models.Lobby> CreateLobby(LobbyModel model)
        {
            try
            {
                if (model.LobbyName == string.Empty)
                {
                    model.LobbyName = $"Room #{Random.Range(0, 100000)}";
                }

                var newLobby = await _lobbyService.CreateLobbyAsync(model.LobbyName, model.MaxPlayers,
                    model.ToCreateLobbyOptions());

                Debug.Log($"Lobby {newLobby.Name}-{newLobby.LobbyCode} {"Created".AddColor(Color.yellow)}");
                JoinedLobby.Value = newLobby;
            }
            catch
            {
                JoinedLobby.Value = null;
            }

            HostLobbyToPing.Value = JoinedLobby.Value;
            return JoinedLobby.Value;
        }

        public Player GetPlayerInJoinedLobby(string playerId)
        {
            if (JoinedLobby.Value != null)
            {
                return JoinedLobby.Value.GetPlayer(playerId);
            }

            return null;
        }

        // public async UniTask<Unity.Services.Lobbies.Models.Lobby> JoinLobbyByCode(string lobbyCode)
        // {
        //     try
        //     {
        //         var joinLobbyData = GetLobbyPlayerDataWithName(_userProfile.DisplayName);
        //         var joinedLobby = await _lobbyService.JoinLobbyByCodeAsync(lobbyCode,
        //             new JoinLobbyByCodeOptions { Player = joinLobbyData });
        //         Debug.Log($"Joined Lobby {joinedLobby.Id} {joinedLobby.LobbyCode}");
        //         JoinedLobby.Value = joinedLobby;
        //     }
        //     catch
        //     {
        //         JoinedLobby.Value = null;
        //     }
        //
        //     HostLobbyToPing.Value = null;
        //     return JoinedLobby.Value;
        // }
        //
        // public async UniTask<Unity.Services.Lobbies.Models.Lobby> QuickJoinLobby(
        //     QuickJoinLobbyOptions options = default)
        // {
        //     try
        //     {
        //         options ??= new QuickJoinLobbyOptions();
        //
        //         var joinLobbyData = GetLobbyPlayerDataWithName(_userProfile.DisplayName);
        //         options.Player = joinLobbyData;
        //         var joinedLobby = await _lobbyService.QuickJoinLobbyAsync(options);
        //         Debug.Log($"Joined Lobby {joinedLobby.Id} {joinedLobby.LobbyCode}");
        //         JoinedLobby.Value = joinedLobby;
        //     }
        //     catch
        //     {
        //         JoinedLobby.Value = null;
        //     }
        //
        //     HostLobbyToPing.Value = null;
        //     return JoinedLobby.Value;
        // }

        public async UniTask LeaveLobby()
        {
            try
            {
                if (JoinedLobby.Value != null)
                {
                    _lobbyService.RemovePlayerAsync(JoinedLobby.Value.Id, AuthenticationService.Instance.PlayerId);
                    JoinedLobby.Value = null;
                }
            }
            catch (Exception e)
            {
                // ignored
            }
        }
        
        public async UniTask<bool> UpdateLobbyData(List<(string key,string data,DataObject.VisibilityOptions visibility)> listOfData)
        {
            try
            {
                var isHost = AmITheHost();
                if (isHost)
                {
                    var updateLobbyOptions = new UpdateLobbyOptions();
                    updateLobbyOptions.Data = new Dictionary<string, DataObject>();
                    foreach (var (key,data,visibility) in listOfData)
                    {
                        updateLobbyOptions.Data.Add(key,new DataObject(visibility,data));
                    }

                    JoinedLobby.Value = await _lobbyService.UpdateLobbyAsync(JoinedLobby.Value.Id, updateLobbyOptions);

                    return true;
                }
            }
            catch
            {
                // ignored
            }

            return false;
        }

        public bool AmITheHost()
        {
            return JoinedLobby.Value != null && JoinedLobby.Value.HostId == AuthenticationService.Instance.PlayerId;
        }

        public async UniTask<Unity.Services.Lobbies.Models.Lobby> JoinLobbyById(string lobbyId)
        {
            try
            {
                var joinedLobby = await _lobbyService.JoinLobbyByIdAsync(lobbyId);
                Debug.Log($"Joined Lobby {joinedLobby.Name}");
                JoinedLobby.Value = joinedLobby;
            }
            catch
            {
                JoinedLobby.Value = null;
            }
            
            HostLobbyToPing.Value = null;
            return JoinedLobby.Value;
        }

        public async UniTask<Unity.Services.Lobbies.Models.Lobby> JoinLobbyByCode(string joinCode)
        {
            try
            {
                var joinedLobby = await _lobbyService.JoinLobbyByCodeAsync(joinCode);
                Debug.Log($"Joined Lobby {joinedLobby.Name}");
                JoinedLobby.Value = joinedLobby;
            }
            catch
            {
                JoinedLobby.Value = null;
            }
            
            HostLobbyToPing.Value = null;
            return JoinedLobby.Value;
        }
    }
}
