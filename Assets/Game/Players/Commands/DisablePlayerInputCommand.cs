using Cysharp.Threading.Tasks;
using Game.Players.Scripts;
using Maniac.Command;
using Maniac.Utils;
using UnityEngine;

namespace Game.Players.Commands
{
    public class DisablePlayerInputCommand : Command
    {
        public override async UniTask Execute()
        {
            var player = Locator<NetPlayer>.Instance;
            if (player == null) return;

            var input = player.GetComponent<NetPlayerInput>();
            if (input == null) return;

            input.SetRawInput(Vector2.zero);
            input.SetSmoothInput(Vector2.zero);
            input.enabled = false;
        }
    }
}