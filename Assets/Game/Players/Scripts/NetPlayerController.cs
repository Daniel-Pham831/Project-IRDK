using Maniac.DataBaseSystem;
using Maniac.Utils;
using Unity.Netcode;
using UnityEngine;

namespace Game.Players.Scripts
{
    public class NetPlayerController : NetworkBehaviour
    {
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private PlayerConfig _playerConfig;
        
        [SerializeField]
        private Rigidbody2D rb;
        private Transform _transform;
        private bool _movable = false;

        private void Awake()
        {
            _playerConfig = _dataBase.GetConfig<PlayerConfig>();
            _transform = transform;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                enabled = false;

            _movable = true;
            base.OnNetworkSpawn();
        }

        private void FixedUpdate()
        {
            if (IsOwner && _movable)
                UpdateMovement();
        }

        private void UpdateMovement()
        {
            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");

            rb.velocity = new Vector2(horizontal, vertical).normalized * _playerConfig.MoveSpeed;
        }
    }
}