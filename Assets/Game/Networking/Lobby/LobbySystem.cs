using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.CloudProfileSystem;
using Game.Networking.Lobby.Commands;
using Game.Networking.Lobby.Extensions;
using Game.Networking.Lobby.Models;
using Game.Networking.NetMessages;
using Game.Scripts;
using Maniac.DataBaseSystem;
using Maniac.MessengerSystem.Base;
using Maniac.MessengerSystem.Messages;
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
    public class LobbySystem : IMessageListener , IDisposable
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
            
            Messenger.Register<ApplicationQuitMessage>(this);
            Messenger.Register<TransportFailureMessage>(this);
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
                        await _handleBeingKickCommand.Execute();
                        await _handleBecomeHostCommand.Execute();
                    }
                    catch
                    {
                        JoinedLobby.Value = null;
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

        public async UniTask LeaveLobby()
        {
            Debug.Log($"LeaveLobby");
            try
            {
                if (JoinedLobby.Value != null)
                {
                    await _lobbyService.RemovePlayerAsync(JoinedLobby.Value.Id, AuthenticationService.Instance.PlayerId);
                }
            }
            catch (Exception e)
            {
                // ignored
            }

            Messenger.SendMessage(new LeaveLobbyMessage());
            await Reset();
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

        public async void OnMessagesReceived(Message receivedMessage)
        {
            switch (receivedMessage)
            {
                case ApplicationQuitMessage:
                case TransportFailureMessage:
                    if (JoinedLobby.Value != null || HostLobbyToPing != null)
                        await LeaveLobby();
                    break;
            }
        }

        public void Dispose()
        {
            HostLobbyToPing?.Dispose();
            JoinedLobby?.Dispose();
            QueryResponse?.Dispose();
        }
    }
}
