using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Networking.LobbySystem.Commands;
using Game.Networking.LobbySystem.Extensions;
using Maniac.DataBaseSystem;
using Maniac.TimeSystem;
using Maniac.Utils;
using Maniac.Utils.Extension;
using UniRx;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;


namespace Game.Networking.LobbySystem
{
    public class LobbySystem
    {
        private LocalData _localData => Locator<LocalData>.Instance;
        private TimeManager _timeManager => Locator<TimeManager>.Instance;
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private LobbyConfig _lobbyConfig;

        private ILobbyService _lobbyService;

        public ReactiveProperty<Unity.Services.Lobbies.Models.Lobby> HostLobbyToPing { get; private set; } =
            new ReactiveProperty<Unity.Services.Lobbies.Models.Lobby>();
        public ReactiveProperty<Unity.Services.Lobbies.Models.Lobby> JoinedLobby { get; private set; } =
            new ReactiveProperty<Unity.Services.Lobbies.Models.Lobby>();

        
        public async UniTask Init()
        {
            _lobbyService = LobbyService.Instance;
            _lobbyConfig = _dataBase.Get<LobbyConfig>();

            ResetJoinedLobby();
            HandleLobbyHeartBeat();
            HandleJoinedLobbyUpdate();
            await UniTask.CompletedTask;
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

                        HandleLocalPlayerBecomeHost();
                    }
                    catch (Exception e)
                    {
                        JoinedLobby.Value = null;
                        await HandleBeingKicked();
                    }
                }

                HandleJoinedLobbyUpdate();
            },_lobbyConfig.LobbyUpdateIntervalInSeconds);
        }

        private async UniTask HandleBeingKicked()
        {
            bool isBeingKicked = JoinedLobby.Value == null;
            if (isBeingKicked)
            {
                Debug.Log("You have been KICKED");
                
            }
        }

        private void HandleLocalPlayerBecomeHost()
        {
            if(HostLobbyToPing.Value == null)
            {
                bool isLocalPlayerBecomeJoinedLobbyHost = JoinedLobby.Value.HostId == _localData.LocalPlayer.Id;
                if (isLocalPlayerBecomeJoinedLobbyHost)
                {
                    HostLobbyToPing.Value = JoinedLobby.Value;
                    Debug.Log("I become the host");
                }
            }
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
            var isLocalPlayerHost = JoinedLobby.Value != null && JoinedLobby.Value.HostId == _localData.LocalPlayer.Id;
            if(isLocalPlayerHost)
            {
                try
                {
                    await _lobbyService.RemovePlayerAsync(lobbyId, playerId);
                    return true;
                }
                catch (Exception e)
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

                var lobbyHostPlayerData = GetLobbyPlayerDataWithName(_localData.LocalPlayer.DisplayName);
                var newLobby = await _lobbyService.CreateLobbyAsync(lobbyName, maxPlayer,
                    new CreateLobbyOptions { Player = lobbyHostPlayerData });

                Debug.Log($"Lobby {newLobby.Name}-{newLobby.LobbyCode} {"Created".AddColor(Color.yellow)}");
                JoinedLobby.Value = newLobby;
            }
            catch (LobbyServiceException e)
            {
                // ignored
                Debug.Log(e);
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

        public async UniTask<Unity.Services.Lobbies.Models.Lobby> JoinLobbyByCode(string lobbyCode)
        {
            try
            {
                var joinLobbyData = GetLobbyPlayerDataWithName(_localData.LocalPlayer.DisplayName);
                var joinedLobby = await _lobbyService.JoinLobbyByCodeAsync(lobbyCode,new JoinLobbyByCodeOptions{Player = joinLobbyData});
                Debug.Log($"Joined Lobby {joinedLobby.Id} {joinedLobby.LobbyCode}");
                JoinedLobby.Value = joinedLobby;
            }
            catch(LobbyServiceException e)
            {
                Debug.Log(e);
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
                
                var joinLobbyData = GetLobbyPlayerDataWithName(_localData.LocalPlayer.DisplayName);
                options.Player = joinLobbyData;
                var joinedLobby = await _lobbyService.QuickJoinLobbyAsync(options);
                Debug.Log($"Joined Lobby {joinedLobby.Id} {joinedLobby.LobbyCode}");
                JoinedLobby.Value = joinedLobby;
            }
            catch(LobbyServiceException e)
            {
                Debug.Log(e);
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
                    _lobbyService.RemovePlayerAsync(JoinedLobby.Value.Id, _localData.LocalPlayer.Id);
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