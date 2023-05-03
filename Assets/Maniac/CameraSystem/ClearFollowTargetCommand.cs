using Cysharp.Threading.Tasks;
using Maniac.Utils;

namespace Maniac.CameraSystem
{
    public class ClearFollowTargetCommand:Command.Command
    {
        private CameraController _cameraController => Locator<CameraController>.Instance;
        
        public override async UniTask Execute()
        {
            _cameraController.ResetCamera();
        }
    }
}