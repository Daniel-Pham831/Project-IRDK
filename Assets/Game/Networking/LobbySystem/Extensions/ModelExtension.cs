using System.Collections.Generic;
using Game.Networking.LobbySystem.Models;
using Maniac.Utils;
using Newtonsoft.Json;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

namespace Game.Networking.LobbySystem.Extensions
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