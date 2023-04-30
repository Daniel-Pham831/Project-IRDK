using System;
using Game.Networking.Network.NetworkModels;
using Maniac.Utils;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Game.Scenes.TestScene
{
    public class ClientTransmitter : Transmitter<ClientTransmitter>
    {
        public void SendToServer(HubModel modelToSend)
        {
            var dataInBytes = modelToSend.ToBytes();
            using (var writer = new FastBufferWriter(dataInBytes.Length, Allocator.Temp))
            {
                writer.WriteValueSafe(dataInBytes);
                _networkManager.CustomMessagingManager.SendUnnamedMessage(NetworkManager.ServerClientId,
                    writer);
            }
        }

        protected override void OnDataReceived(byte[] data, ulong clientId)
        {
            base.OnDataReceived(data, clientId);
            
            _netModelHub.ReceiveHubModel(Helper.Deserialize<HubModel>(data));
        }
    }
}