using Cysharp.Threading.Tasks;
using Maniac.Command;

namespace Game.Networking.NetMessengerSystem
{
    public class InitNetMessageTransmitterCommand : Command
    {
        public override async UniTask Execute()
        {
            var netMessageTransmitter = new NetMessageTransmitter();
            await netMessageTransmitter.Init();
        }
    }
}