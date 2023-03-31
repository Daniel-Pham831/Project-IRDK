using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.UISystem.Command;

namespace Game.Networking.LobbySystem.Commands
{
    public class ShowLobbyScreenCommand : Command
    {
        public override async UniTask Execute()
        {
            var result = (LobbyScreen.Result)await ShowScreenCommand.Create<LobbyScreen>().ExecuteAndReturnResult();

            switch (result)
            {
                case LobbyScreen.Result.Create:
                    await new CreateNewLobbyCommand().Execute();
                    break;
                case LobbyScreen.Result.Join:
                    
                    break;
                case LobbyScreen.Result.QuickJoin:
                    
                    break;
            }
        }
    }
}