using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.Networking.Lobby;
using Game.Networking.NetPlayerComponents;
using Maniac.Command;
using Maniac.Utils;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Game.Networking.Network.Commands
{
    public class StartNetworkSessionCommand : Command
    {
        private LobbySystem _lobbySystem = Locator<LobbySystem>.Instance;
        private NetworkSystem _networkSystem => Locator<NetworkSystem>.Instance;

        public override async UniTask Execute()
        {
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
            while (true)
            {
                if (Locator<NetPlayer>.Instance != null)
                {
                    break;
                }

                await UniTask.Delay(100);
            }
        }
    }
}