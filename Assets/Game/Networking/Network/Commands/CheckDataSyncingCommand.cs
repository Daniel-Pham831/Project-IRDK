using System;
using Cysharp.Threading.Tasks;
using Game.Networking.Network.NetworkModels;
using Game.Networking.Network.NetworkModels.Handlers;
using Game.Networking.Network.NetworkModels.Handlers.NetLobbyModel;
using Maniac.Command;
using Maniac.DataBaseSystem;
using Maniac.Utils;
using UnityEngine;

namespace Game.Networking.Network.Commands
{
    public class CheckDataSyncingCommand : Command
    {
        private NetModelHub _netModelHub => Locator<NetModelHub>.Instance;
        private DataBase _dataBase = Locator<DataBase>.Instance;
        private LobbyConfig _lobbyConfig;

        public override async UniTask Execute()
        {
            var netLobbyModelHandler = _netModelHub.GetHandler<NetLobbyModelHandler>();
            _lobbyConfig = _dataBase.GetConfig<LobbyConfig>();

            var counter = 0f;
            while (!netLobbyModelHandler.IsAllClientsDataSyncReady())
            {
                if (counter >= _lobbyConfig.ConnectLobbyTimeoutInSeconds)
                    throw new Exception("Data synchronization failed!");

                counter += Time.deltaTime;
                await UniTask.Delay(100);
            }
        }
    }
}