using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Networking.Lobby;
using Maniac.Command;
using Maniac.Utils;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;

namespace Game.Networking.Relay.Commands
{
    public class UpdateRelayDataForLobbyCommand : ResultCommand<bool>
    {
        private readonly (Allocation, string) _relayData;
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;
        
        public UpdateRelayDataForLobbyCommand((Allocation,string) relayData)
        {
            _relayData = relayData;
        }

        public override async UniTask Execute()
        {
            var listOfData = new List<(string, string, DataObject.VisibilityOptions)>
            {
                (LobbyDataKey.LobbyRegion,_relayData.Item1.Region,DataObject.VisibilityOptions.Public),
                (LobbyDataKey.RelayJoinCode, _relayData.Item2, DataObject.VisibilityOptions.Member),
            };
            
            _result = await _lobbySystem.UpdateLobbyData(listOfData);
        }
    }
}