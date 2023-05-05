using Cysharp.Threading.Tasks;
using Game.Players.Scripts;
using Maniac.Command;
using Maniac.Utils;

namespace Game.Players.Commands
{
    public class EnablePlayerInputCommand : Command
    {
        public override async UniTask Execute()
        {
            var player = Locator<NetPlayer>.Instance;
            if (player == null) return;

            var input = player.GetComponent<NetPlayerInput>();
            if (input == null) return;

            input.enabled = true;
        }
    }
}