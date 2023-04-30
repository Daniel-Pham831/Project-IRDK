using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Networking.Lobby;
using Game.Networking.NetDataTransmitterComponents;
using Maniac.Command;
using Maniac.DataBaseSystem;
using Maniac.Utils;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Game.Networking.Network.Commands
{
    public class StartNetworkSessionCommand : Command
    {
        private LobbySystem _lobbySystem = Locator<LobbySystem>.Instance;
        private NetworkSystem _networkSystem => Locator<NetworkSystem>.Instance;
        private DataBase _dataBase = Locator<DataBase>.Instance;
        private LobbyConfig _lobbyConfig;
        private NetDataTransmitter _transmitter;

        public override async UniTask Execute()
        {
            _lobbyConfig = _dataBase.GetConfig<LobbyConfig>();
            await CheckNetPlayerInstance();
            
            if (_lobbySystem.AmITheHost())
            {
                _networkSystem.NetworkManager.StartHost();
            }
            else
            {
                _networkSystem.NetworkManager.StartClient();
            }
            
            _transmitter.Init();
        }

        private async UniTask CheckNetPlayerInstance()
        {
            var counter = 0f;
            var transmitter = Locator<NetDataTransmitter>.Instance;
            if (transmitter == null)
            {
                var go = new GameObject();
                GameObject.DontDestroyOnLoad(go);
                _transmitter = go.AddComponent<NetDataTransmitter>();
                go.name = _lobbySystem.AmITheHost() ? "Server-Side Transmitter" : "Client-Side Transmitter";
            }
        }
    }
}