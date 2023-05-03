using System;
using Maniac.DataBaseSystem;
using Maniac.Utils;
using Unity.Netcode;
using UnityEngine;

namespace Game.Players.Scripts
{
    public class NetPlayerMovement : NetworkBehaviour
    {
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private PlayerConfig _playerConfig;
        
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Transform graphicsHolder;
        [SerializeField] private NetPlayerInput _netPlayerInput;
        [SerializeField] private Animator _animator;

        private void Awake()
        {
            _playerConfig = _dataBase.GetConfig<PlayerConfig>();
        }

        private void Update()
        {
            UpdateCharacterGraphics();
        }

        private void UpdateCharacterGraphics()
        {
            // Update character graphics direction
            var localScale = graphicsHolder.localScale;
            localScale = new Vector3(_netPlayerInput.SmoothInputVector.x > 0 ? 1 : _netPlayerInput.SmoothInputVector.x < 0 ? -1 : graphicsHolder.localScale.x, localScale.y, localScale.z);
            graphicsHolder.localScale = localScale;

            _animator.SetBool(IsMoving, _netPlayerInput.SmoothInputVector != Vector2.zero);
        }

        private void FixedUpdate()
        {
            if (IsOwner)
                UpdateMovement();
        }

        private void UpdateMovement()
        {
            rb.velocity = _netPlayerInput.RawInputVector * _playerConfig.MoveSpeed;
        }
    }
}