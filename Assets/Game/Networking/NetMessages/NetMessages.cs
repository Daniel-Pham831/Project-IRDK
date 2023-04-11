using System;
using Game.Networking.NetPlayerComponents;
using Maniac.MessengerSystem.Messages;

namespace Game.Networking.NetMessages
{
    public class NetMessage : Message { }

    public class ApplicationQuitMessage : Message { }
    
    // Lobby
    public class LeaveLobbyMessage : NetMessage{}

    public class ClientConnectedMessage : NetMessage
    {
        public ulong ClientId { get; set; }
    }

    public class ClientDisconnectedMessage : NetMessage
    {
        public ulong ClientId { get; set; }
    }
    
    public class LocalClientNetworkSpawn : NetMessage{}
    
    public class TransportFailureMessage : NetMessage{}
}