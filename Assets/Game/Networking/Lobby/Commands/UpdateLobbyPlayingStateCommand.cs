using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.Utils;
using Unity.Services.Lobbies.Models;

namespace Game.Networking.Lobby.Commands
{
    public class UpdateLobbyPlayingStateCommand : Command
    {
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;
        
        public override async UniTask Execute()
        {
            if (!_lobbySystem.AmITheHost()) return;

            var listOfData = new List<(string, DataObject)>
            {
                (LobbyDataKey.IsPlaying,
                    new DataObject(DataObject.VisibilityOptions.Public, "true", DataObject.IndexOptions.S1))
            };
            
            await _lobbySystem.UpdateLobbyData(listOfData);
        }
    }
}