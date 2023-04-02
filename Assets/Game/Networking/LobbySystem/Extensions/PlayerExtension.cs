using Unity.Services.Lobbies.Models;

namespace Game.Networking.LobbySystem.Extensions
{
    public static class PlayerExtension
    {
        public static string GetPlayerName(this Player player)
        {
            return player.Data[LobbyDataKey.PlayerName].Value;
        }
        
    }
}