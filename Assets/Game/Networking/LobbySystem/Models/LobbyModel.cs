using System;
using System.Collections.Generic;

namespace Game.Networking.LobbySystem.Models
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