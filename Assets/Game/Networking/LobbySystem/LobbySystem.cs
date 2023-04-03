using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.CloudProfileSystem;
using Game.Networking.LobbySystem.Commands;
using Game.Networking.LobbySystem.Extensions;
using Maniac.DataBaseSystem;
using Maniac.TimeSystem;
using Maniac.Utils;
using Maniac.Utils.Extension;
using UniRx;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;


namespace Game.Networking.LobbySystem
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
        
        public async UniTask Init()
        {
            _userProfile = await _cloudProfileManager.Get<UserProfile>();
            _lobbyService = LobbyService.Instance;
            _lobbyConfig = _dataBase.Get<LobbyConfig>();

            InitCommands();
            
            ResetJoinedLobby();
            HandleLobbyHeartBeat();
            HandleJoinedLobbyUpdate();
            await UniTask.CompletedTask;
        }

        private void InitCommands()
        {
            _handleBecomeHostCommand = new HandleLocalPlayerBecomeHostCommand();
            _handleBeingKickCommand = new HandleBeingKickedCommand();
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
            },_lobbyConfig.LobbyUpdateIntervalInSeconds);
        }

        public async UniTask ResetJoinedLobby()
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
            },_lobbyConfig.HeartBeatIntervalInSeconds);
        }

        public async UniTask<bool> KickPlayerFromLobby(string lobbyId,string playerId)
        {
            var isLocalPlayerHost = JoinedLobby.Value != null && JoinedLobby.Value.HostId == AuthenticationService.Instance.PlayerId;
            if(isLocalPlayerHost)
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
        
        public async UniTask<Unity.Services.Lobbies.Models.Lobby> CreateLobby(string lobbyName = "",int maxPlayer = 4)
        {
            try
            {
                if (lobbyName == string.Empty)
                    lobbyName = "Room";
                
                lobbyName += $" #{UnityEngine.Random.Range(0, 10000)}";

                var lobbyHostPlayerData = GetLobbyPlayerDataWithName(_userProfile.DisplayName);
                var newLobby = await _lobbyService.CreateLobbyAsync(lobbyName, maxPlayer,
                    new CreateLobbyOptions { Player = lobbyHostPlayerData });

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

        private Player GetLobbyPlayerDataWithName(string name)
        {
            return new Player
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {
                        LobbyDataKey.PlayerName, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, name)
                    }
                }
            };
        }

        public Player GetPlayerInJoinedLobby(string playerId)
        {
            if (JoinedLobby.Value != null)
            {
                return JoinedLobby.Value.GetPlayer(playerId);
            }

            return null;
        }

        public async UniTask<Unity.Services.Lobbies.Models.Lobby> JoinLobbyByCode(string lobbyCode)
        {
            try
            {
                var joinLobbyData = GetLobbyPlayerDataWithName(_userProfile.DisplayName);
                var joinedLobby = await _lobbyService.JoinLobbyByCodeAsync(lobbyCode,new JoinLobbyByCodeOptions{Player = joinLobbyData});
                Debug.Log($"Joined Lobby {joinedLobby.Id} {joinedLobby.LobbyCode}");
                JoinedLobby.Value = joinedLobby;
            }
            catch
            {
                JoinedLobby.Value = null;
            }

            HostLobbyToPing.Value = null;
            return JoinedLobby.Value;
        }
        
        public async UniTask<Unity.Services.Lobbies.Models.Lobby> QuickJoinLobby(QuickJoinLobbyOptions options = default)
        {
            try
            {
                options ??= new QuickJoinLobbyOptions();
                
                var joinLobbyData = GetLobbyPlayerDataWithName(_userProfile.DisplayName);
                options.Player = joinLobbyData;
                var joinedLobby = await _lobbyService.QuickJoinLobbyAsync(options);
                Debug.Log($"Joined Lobby {joinedLobby.Id} {joinedLobby.LobbyCode}");
                JoinedLobby.Value = joinedLobby;
            }
            catch
            {
                JoinedLobby.Value = null;
            }

            HostLobbyToPing.Value = null;
            return JoinedLobby.Value;
        }

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
    }
}