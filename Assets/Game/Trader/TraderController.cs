using Cysharp.Threading.Tasks;
using Game.Interfaces;
using Game.Players.Scripts;
using Maniac.UISystem.Command;
using Maniac.Utils;
using UnityEngine;

namespace Game.Trader
{
    public class TraderController : MonoBehaviour, IInteractableMono
    {
        private NetPlayer _netPlayer => Locator<NetPlayer>.Instance;
        private NetPlayerInput _netPlayerInput;

        private void Awake()
        {
            _netPlayerInput = _netPlayer.GetComponent<NetPlayerInput>();
        }

        public MonoBehaviour Mono => this;
        public int InteractPriority => 1;

        public async UniTask Interact(object interactor = null)
        {
            await new ShowScreenCommand<TraderScreen>().Execute();
        }
    }
}