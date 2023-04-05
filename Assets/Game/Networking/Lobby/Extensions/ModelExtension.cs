using Game.Networking.Lobby.Models;
using Unity.Services.Lobbies;

namespace Game.Networking.Lobby.Extensions
{
    public static class ModelExtension
    {
        public static CreateLobbyOptions ToCreateLobbyOptions(this LobbyModel model)
        {
            var result = new CreateLobbyOptions
            {
                IsPrivate = model.IsPrivateLobby
            };
            return result;
        }
    }
}