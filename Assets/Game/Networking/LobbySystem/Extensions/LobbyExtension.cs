using Maniac.Utils;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;

namespace Game.Networking.LobbySystem.Extensions
{
    public static class LobbyExtension
    {
        public static Player GetLocalPlayer(this Unity.Services.Lobbies.Models.Lobby lobby)
        {
            foreach (var player in lobby.Players)
            {
                if (player.Id == AuthenticationService.Instance.PlayerId)
                    return player;
            }

            return null;
        }
        
        public static Player GetPlayer(this Unity.Services.Lobbies.Models.Lobby lobby,string playerId)
        {
            foreach (var player in lobby.Players)
            {
                if (player.Id == playerId)
                    return player;
            }

            return null;
        }
    }
}