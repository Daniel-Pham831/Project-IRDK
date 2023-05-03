using Cysharp.Threading.Tasks;
using Maniac.Utils;
using UnityEngine;

namespace Maniac.CameraSystem
{
    public class CameraFollowTargetCommand:Command.Command
    {
        private CameraController _cameraController => Locator<CameraController>.Instance;
        private Transform _targetTransform;
        
        public CameraFollowTargetCommand(Transform targetTransform)
        {
            _targetTransform = targetTransform;
        }
        
        public override async UniTask Execute()
        {
            _cameraController.SubscribeToCameraMovement(_targetTransform);
        }
    }
}