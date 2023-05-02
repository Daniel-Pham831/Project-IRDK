using Cysharp.Threading.Tasks;
using Game.MazeSystem;
using Game.Scenes.NetGamePlay.Environment.Scripts;
using Game.Scenes.NetGamePlay.Scripts;
using Maniac.Command;
using Maniac.Utils;

namespace Game.Scenes.NetGamePlay.Commands
{
    public class InitNetGamePlayCommand : Command
    {
        private NetGamePlayController _netGamePlayController => Locator<NetGamePlayController>.Instance;
        private EnvironmentController _environmentController => Locator<EnvironmentController>.Instance;
        private MazeGenerator _mazeGenerator => Locator<MazeGenerator>.Instance;
        
        public override async UniTask Execute()
        {
            await _environmentController.Init();
            await _netGamePlayController.Init();
        }
    }
}