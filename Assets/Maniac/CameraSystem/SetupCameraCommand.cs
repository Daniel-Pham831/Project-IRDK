using Cysharp.Threading.Tasks;
using Game.Players.Scripts;
using Game.Scenes.NetGamePlay.Environment.Scripts;
using Maniac.Utils;

namespace Maniac.CameraSystem
{
    public class SetupCameraCommand : Command.Command
    {
        private CameraController _cameraController => Locator<CameraController>.Instance;
        private NetPlayer _netPlayer => Locator<NetPlayer>.Instance;
        private EnvironmentController _environmentController => Locator<EnvironmentController>.Instance;
        
        public override async UniTask Execute()
        {
            _cameraController.ResetCamera();
            _cameraController.SetConfiner(_environmentController.Confiner);
            _cameraController.SubscribeToCameraMovement(_netPlayer.transform);
        }
    }
}