using Maniac.Utils;
using Unity.Netcode;
using UnityEngine;

namespace Game.Scenes.TestScene
{
    public class NetDummyPlayer : NetworkBehaviour
    {
        private GameObject _transmitter;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                if(Locator<ServerTransmitter>.Instance == null)
                {
                    var go = new GameObject();
                    go.name = "Server";
                    go.AddComponent<ServerTransmitter>();
                    _transmitter = go;
                }
            }
            else
            {
                if(Locator<ClientTransmitter>.Instance == null)
                {
                    var go = new GameObject();
                    go.name = "Client";
                    go.AddComponent<ClientTransmitter>();
                    _transmitter = go;
                }
            }
        }

        public override void OnNetworkDespawn()
        {
            Destroy(_transmitter);
            base.OnNetworkDespawn();
        }
    }
}