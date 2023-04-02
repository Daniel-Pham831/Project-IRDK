using Maniac.Utils;
using Unity.Services.Lobbies.Models;

namespace Game.Networking.LobbySystem.Extensions
{
    public static class LobbyExtension
    {
        private static LocalData _localData => Locator<LocalData>.Instance;

        public static Player GetLocalPlayer(this Unity.Services.Lobbies.Models.Lobby lobby)
        {
            foreach (var player in lobby.Players)
            {
                if (player.Id == _localData.LocalPlayer.Id)
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