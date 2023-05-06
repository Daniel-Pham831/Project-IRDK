using Cysharp.Threading.Tasks;
using Game.Players.Scripts;
using Maniac.Command;
using Maniac.Utils;

namespace Game.Players.Commands
{
    public class InteractWithInteractableCommand : Command
    {
        private NetPlayer _netPlayer => Locator<NetPlayer>.Instance;
        
        public override async UniTask Execute()
        {
            if(_netPlayer == null) return;

            if (_netPlayer.TryGetComponent<NetPlayerInput>(out var netPlayerInput))
            {
                await netPlayerInput.InteractWithInteractable();
            }
        }
    }
}