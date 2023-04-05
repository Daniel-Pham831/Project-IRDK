using System;

namespace Game.Networking.Lobby.Models
{
    [Serializable]
    public class LobbyModel
    {
        // For showing on UI
        public string LobbyName { get; set; } 
        public int MaxPlayers { get; set; }
        public bool IsPrivateLobby { get; set; }
    }
}