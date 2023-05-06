using System.Collections.Generic;
using Maniac.DataBaseSystem;
using Maniac.Utils;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

namespace Game.Networking.Lobby
{
    public static class LobbyHelper
    {
        private static DataBase _dataBase => Locator<DataBase>.Instance;
        private static LobbyConfig _lobbyConfig => _dataBase.GetConfig<LobbyConfig>();
        private static QueryLobbiesOptions _queryLobbiesOptions;
        
        public static QueryLobbiesOptions QueryLobbiesOptions
        {
            get
            {
                if (_queryLobbiesOptions == null)
                {
                    _queryLobbiesOptions = new QueryLobbiesOptions();
                    _queryLobbiesOptions.Count = _lobbyConfig.NumOfLobbyPerQuery;

                    // Filter for open lobbies only
                    _queryLobbiesOptions.Filters = new List<QueryFilter>()
                    {
                        // Only query for lobbies that have available slots
                        new QueryFilter(
                            field: QueryFilter.FieldOptions.AvailableSlots,
                            op: QueryFilter.OpOptions.GT,
                            value: "0")
                        ,
                        // Only query for lobbies that are NOT playing
                        new QueryFilter(
                            field: QueryFilter.FieldOptions.S1, // IsPlaying
                            op: QueryFilter.OpOptions.EQ,
                            value: "false")
                        ,
                        // Only query for lobbies that are ready
                        new QueryFilter(
                            field: QueryFilter.FieldOptions.S2, // IsLobbyReady
                            op: QueryFilter.OpOptions.EQ,
                            value: "true")
                    };

                    // Order by newest lobbies first
                    _queryLobbiesOptions.Order = new List<QueryOrder>()
                    {
                        new QueryOrder(
                            asc: false,
                            field: QueryOrder.FieldOptions.Created)
                    };
                }

                return _queryLobbiesOptions;
            }
        }
    }
}