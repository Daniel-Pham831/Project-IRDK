using Cysharp.Threading.Tasks;
using Game.Scenes;
using Maniac.Command;
using Maniac.UISystem;
using Maniac.Utils;

namespace Game.Commands
{
    public class LoadEmptySceneCommand: Command
    {
        private readonly bool _loadWithLoadingScreen;
        private UIManager _uiManager => Locator<UIManager>.Instance;
        public LoadEmptySceneCommand(bool loadWithLoadingScreen = false)
        {
            _loadWithLoadingScreen = loadWithLoadingScreen;
        }
        
        public override async UniTask Execute()
        {
            _uiManager.CloseAllCurrentShowed();
            await new LoadSceneCommand(new LoadSceneCommand.Param(SceneName.EmptyScene, _loadWithLoadingScreen)).Execute();
            await UniTask.CompletedTask;
        }
    }
}