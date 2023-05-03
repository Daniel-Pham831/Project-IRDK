using Cysharp.Threading.Tasks;
using Game.Scenes;
using Maniac.CameraSystem;
using Maniac.Command;
using Maniac.UISystem;
using Maniac.Utils;

namespace Game.Commands
{
    public class LoadEmptySceneCommand: Command
    {
        private readonly bool _loadWithLoadingScreen;
        private UIManager _uiManager => Locator<UIManager>.Instance;
        private CameraController _cameraController => Locator<CameraController>.Instance;
        public LoadEmptySceneCommand(bool loadWithLoadingScreen = false)
        {
            _loadWithLoadingScreen = loadWithLoadingScreen;
        }
        
        public override async UniTask Execute()
        {
            _uiManager.CloseAllCurrentShowed();
            _cameraController.ResetCamera();
            await new LoadSceneCommand(SceneName.EmptyScene).Execute();
            await UniTask.CompletedTask;
        }
    }
}