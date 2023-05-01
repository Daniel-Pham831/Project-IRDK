using Maniac.MessengerSystem.Messages;

namespace Game.Networking.NormalMessages
{
    public class ApplicationQuitMessage : Message { }
    
    // Lobby
    public class LeaveLobbyMessage : Message{}

    public class ClientConnectedMessage : Message
    {
        public ulong ClientId { get; set; }
    }

    public class ClientDisconnectedMessage : Message
    {
        public ulong ClientId { get; set; }
    }
    
    public class LocalClientNetworkSpawn : Message{}
    
    public class TransportFailureMessage : Message{}
}