using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Networking.Lobby;
using Game.Networking.NetPlayerComponents;
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
        private float _timeoutCounter;

        public override async UniTask Execute()
        {
            _lobbyConfig = _dataBase.Get<LobbyConfig>();
            _timeoutCounter = 0;
            if (_lobbySystem.AmITheHost())
            {
                _networkSystem.NetworkManager.StartHost();
            }
            else
            {
                _networkSystem.NetworkManager.StartClient();
            }

            await CheckNetPlayerInstance();
        }

        private async UniTask CheckNetPlayerInstance()
        {
            var counter = 0f;
            var shouldReconnect = false;
            while (true)
            {
                if (Locator<NetPlayer>.Instance != null)
                {
                    return;
                }
                counter += Time.deltaTime;
                _timeoutCounter += Time.deltaTime;
                if (counter >= _lobbyConfig.RetryConnectLobbyTimeInSeconds)
                {
                    shouldReconnect = true;
                    break;
                }

                if (_timeoutCounter >= _lobbyConfig.ConnectLobbyTimeoutInSeconds)
                {
                    throw new Exception("Cannot connect to Server!");
                }

                await UniTask.Delay(100);
            }

            if (shouldReconnect)
            {
                _networkSystem.NetworkManager.Shutdown();
                await Execute();
            }
        }
    }
}