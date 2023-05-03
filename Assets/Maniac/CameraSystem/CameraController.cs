using Cinemachine;
using Maniac.Utils;
using UnityEngine;

namespace Maniac.CameraSystem
{
    public class CameraController : MonoLocator<CameraController>
    {
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        [SerializeField] private CinemachineConfiner2D _confiner2D;
        [SerializeField] private Transform _cameraMover;

        private Transform _targetToFollow;

        public override void Awake()
        {
            base.Awake();
            
            _virtualCamera.Follow = _cameraMover;
        }

        public void SetConfiner(PolygonCollider2D polygonCollider2D)
        {
            _confiner2D.m_BoundingShape2D = polygonCollider2D;
        }
        
        public void ResetCamera()
        {
            _targetToFollow = null;
            _cameraMover.position = new Vector3(0, 0, _cameraMover.position.z);
            _confiner2D.m_BoundingShape2D = null;
        }
        
        public void SubscribeToCameraMovement(Transform targetTransform)
        {
            _targetToFollow = targetTransform;
        }

        private void LateUpdate()
        {
            if (_targetToFollow != null)
            {
                _cameraMover.position = _targetToFollow.position;
            }
        }
    }
}