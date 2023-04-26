using Unity.Netcode;
using UnityEngine;

namespace Game.Players.Scripts
{
    public class NetPlayerInput : NetworkBehaviour
    {
        private Vector2 _rawInput;
        private Vector2 _smoothInput;
        
        public Vector2 RawInputVector
        {
            get => IsOwner ? _rawInput : _rawInputVector.Value;
            private set => _rawInput = value;
        }

        public Vector2 SmoothInputVector 
        {
            get => IsOwner ? _smoothInput : _smoothInputVector.Value;
            private set => _smoothInput = value;
        }

        private NetworkVariable<Vector2> _rawInputVector = new NetworkVariable<Vector2>(default,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        private NetworkVariable<Vector2> _smoothInputVector = new NetworkVariable<Vector2>(default,
            NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                enabled = false;

            base.OnNetworkSpawn();
        }
        
        public void Update()
        {
            if (IsOwner)
            {
                RawInputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
                SmoothInputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
                _rawInputVector.Value = RawInputVector;
                _smoothInputVector.Value = SmoothInputVector;
            }
        }
    }
}