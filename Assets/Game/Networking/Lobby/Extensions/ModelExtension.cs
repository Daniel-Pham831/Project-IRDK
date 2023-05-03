using System.Collections.Generic;
using Game.Networking.Lobby.Models;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

namespace Game.Networking.Lobby.Extensions
{
    public static class ModelExtension
    {
        public static CreateLobbyOptions ToCreateLobbyOptions(this LobbyModel model)
        {
            var result = new CreateLobbyOptions
            {
                IsPrivate = model.IsPrivateLobby,
                Data = new Dictionary<string, DataObject>()
                {
                    {LobbyDataKey.IsPlaying,new DataObject(DataObject.VisibilityOptions.Public,"false",DataObject.IndexOptions.S1)}
                }
            };
            return result;
        }
    }
}