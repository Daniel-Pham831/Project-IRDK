using System;
using System.Collections.Generic;
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
        private readonly HandleBeingKickedCommand _beingKickedCommand = new HandleBeingKickedCommand();
        
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
                        await _beingKickedCommand.Execute();
                    }
                    catch (Exception e)
                    {
                        JoinedLobby.Value = null;
                    }
                }

                HandleJoinedLobbyUpdate();
            },_lobbyConfig.LobbyUpdateIntervalInSeconds);
        }

        private void HandleLocalPlayerBecomeHost()
        {
            bool isLocalPlayerBecomeJoinedLobbyHost = JoinedLobby.Value.HostId == _localData.LocalPlayer.Id;
            if (isLocalPlayerBecomeJoinedLobbyHost)
                HostLobbyToPing.Value = JoinedLobby.Value;
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

                var lobbyHostPlayerData = GetLobbyPlayerData(_localData.LocalPlayer.DisplayName,Color.grey);
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

        private Player GetLobbyPlayerData(string name, Color color)
        {
            return new Player
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {
                        LobbyDataKey.PlayerName, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, name)
                    },
                    {
                        LobbyDataKey.PlayerSlotColor,
                        new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, color.ToHexString())
                    }
                }
            };
        }

        public async UniTask UpdatePlayerData(string lobbyId,string playerId,string key, string jsonData)
        {
            var dataToSend = new UpdatePlayerOptions()
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {
                        key, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, jsonData)
                    }
                }
            };

            try
            {
                var updatedLobby = await _lobbyService.UpdatePlayerAsync(lobbyId, playerId, dataToSend);
                JoinedLobby.Value = updatedLobby;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
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
                var joinLobbyData = GetLobbyPlayerData(_localData.LocalPlayer.DisplayName, Color.gray);
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
        
        public async UniTask<Unity.Services.Lobbies.Models.Lobby> JoinLobbyById(string lobbyId)
        {
            try
            {
                var joinLobbyData = GetLobbyPlayerData(_localData.LocalPlayer.DisplayName, Color.gray);
                var joinedLobby = await _lobbyService.JoinLobbyByIdAsync(lobbyId,new JoinLobbyByIdOptions{Player = joinLobbyData});
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
                
                var joinLobbyData = GetLobbyPlayerData(_localData.LocalPlayer.DisplayName, Color.gray);
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
    }
}