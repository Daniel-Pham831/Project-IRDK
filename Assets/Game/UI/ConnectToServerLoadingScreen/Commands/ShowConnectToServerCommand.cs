using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.UISystem;
using Maniac.Utils;

namespace Game.Commands
{
    public class ShowConnectToServerCommand : Command
    {
        public override async UniTask Execute()
        {
            await Locator<UIManager>.Instance.Show<ConnectToServerLoadingScreen>();
        }
    }
}