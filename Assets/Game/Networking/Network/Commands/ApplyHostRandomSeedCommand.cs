using System;
using Cysharp.Threading.Tasks;
using Game.Networking.Lobby;
using Game.Networking.Network.NetworkModels;
using Game.Networking.Network.NetworkModels.Handlers.NetLobbyModel;
using Maniac.Command;
using Maniac.DataBaseSystem;
using Maniac.RandomSystem;
using Maniac.Utils;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Networking.Network.Commands
{
    public class ApplyHostRandomSeedCommand : Command
    {
        private NetModelHub _netModelHub => Locator<NetModelHub>.Instance;
        private DataBase _dataBase = Locator<DataBase>.Instance;
        private NetLobbyModelHandler _netLobbyModelHandler;
        private LobbyConfig _lobbyConfig;

        public override async UniTask Execute()
        {
            if (NetworkManager.Singleton.IsHost) return;
            
            _lobbyConfig = _dataBase.GetConfig<LobbyConfig>();
            _netLobbyModelHandler ??= _netModelHub.GetHandler<NetLobbyModelHandler>();
            NetLobbyModel hostNetLobbyModel = null;
            var lobbySystem = Locator<LobbySystem>.Instance;

            var counter = 0f;
            while (true)
            {
                hostNetLobbyModel = await _netLobbyModelHandler
                    .GetModelByPlayerId(lobbySystem.JoinedLobby.Value.HostId);

                if (hostNetLobbyModel != null)
                    break;
                
                if (counter >= _lobbyConfig.ConnectLobbyTimeoutInSeconds)
                    throw new Exception("ApplyHostRandomSeed timeout!");

                counter += Time.deltaTime;
                await UniTask.Delay(100);
            }
            
            Locator<Randomer>.Instance.SetSeed(hostNetLobbyModel.RandomSeed);
            await UniTask.CompletedTask;
        }
    }
}