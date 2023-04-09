using Cysharp.Threading.Tasks;
using Game.Networking.Network.NetworkModels;
using Maniac.Command;
using Maniac.Utils;
using UnityEngine;

namespace Game.Networking.Network.Commands
{
    public class InitNetworkModelHubCommand : Command
    {
        public override async UniTask Execute()
        {
            var newObj = new GameObject("Network Model Hub");
            Object.DontDestroyOnLoad(newObj);
            var netModelHub = newObj.AddComponent<NetModelHub>();
            await netModelHub.Init();
        }
    }
}