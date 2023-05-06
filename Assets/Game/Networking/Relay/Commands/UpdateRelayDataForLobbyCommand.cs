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
            var listOfData = new List<(string, DataObject)>
            {
                (LobbyDataKey.LobbyRegion,
                    new DataObject(DataObject.VisibilityOptions.Public, _relayData.Item1.Region)),
                (LobbyDataKey.RelayJoinCode, new DataObject(DataObject.VisibilityOptions.Member, _relayData.Item2))
            };
            
            _result = await _lobbySystem.UpdateLobbyData(listOfData);
        }
    }
}