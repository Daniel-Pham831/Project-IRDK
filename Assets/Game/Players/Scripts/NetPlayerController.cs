using Unity.Netcode;
using UnityEngine;

namespace Game.Players.Scripts
{
    public class NetPlayerController : NetworkBehaviour
    {
        private Transform _transform;
        private bool _movable = false;

        private void Awake()
        {
            _transform = transform;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                enabled = false;

            _movable = true;
            base.OnNetworkSpawn();
        }

        private void Update()
        {
            if (IsOwner && _movable)
                UpdateMovement();
        }

        private void UpdateMovement()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            _transform.position += new Vector3(horizontal, vertical, _transform.position.z).normalized * Time.deltaTime;
        }
    }
}