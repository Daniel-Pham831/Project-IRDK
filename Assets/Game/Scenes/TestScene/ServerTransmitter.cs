using System;
using System.Collections.Generic;
using Game.Networking.Network.NetworkModels;
using Maniac.Utils;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Game.Scenes.TestScene
{
    public class ServerTransmitter : Transmitter<ServerTransmitter>
    {
        // Received data from a client and update to all the other clients
        protected override void OnDataReceived(byte[] data, ulong clientId)
        {
            List<ulong> clientIds = (List<ulong>)NetworkManager.Singleton.ConnectedClientsIds;
            
            // Remove server
            clientIds.Remove(NetworkManager.ServerClientId);
            // update manually for server
            _netModelHub.ReceiveHubModel(Helper.Deserialize<HubModel>(data));
            
            // Remove the client that sent the data, due to that client already has the data
            clientIds.Remove(clientId);
            
            SendDataToClients(data,clientIds);
        }
        
        private void SendDataToClients(byte[] dataToSend,IReadOnlyList<ulong> clientIds)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                Debug.LogWarning("Only server can send data to clients");
                return;
            }
            
            using (var writer = new FastBufferWriter(dataToSend.Length, Allocator.Temp))
            {
                writer.WriteValueSafe(dataToSend);
                _networkManager.CustomMessagingManager.SendUnnamedMessage(clientIds,
                    writer);
            }
        }
    }
}