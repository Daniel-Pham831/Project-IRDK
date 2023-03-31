using System;
using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.Utils;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

namespace Game.Networking.LobbySystem.Commands
{
    public class FetchNewLobbiesListCommand : ResultCommand<QueryResponse>
    {
        private readonly QueryLobbiesOptions _options;
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;

        public FetchNewLobbiesListCommand(QueryLobbiesOptions options = default)
        {
            _options = options;
        }

        public override async UniTask Execute()
        {
            _result = await _lobbySystem.FetchLobbiesList(_options);
        }
    }
}