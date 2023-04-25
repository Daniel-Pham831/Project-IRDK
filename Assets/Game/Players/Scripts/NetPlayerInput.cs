using Unity.Netcode;
using UnityEngine;

namespace Game.Players.Scripts
{
    public class NetPlayerInput : NetworkBehaviour
    {
        public Vector2 RawInputVector { get; private set; }
        public Vector2 InputVector { get; private set; }
        
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
                InputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            }
        }
    }
}