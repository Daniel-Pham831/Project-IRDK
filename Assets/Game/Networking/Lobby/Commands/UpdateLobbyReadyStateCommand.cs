using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.Utils;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Game.Networking.Lobby.Commands
{
    public class UpdateLobbyReadyStateCommand : Command
    {
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;
        
        public override async UniTask Execute()
        {
            if (!_lobbySystem.AmITheHost()) return;
            
            Debug.Log("UpdateLobbyReadyStateCommand");
            var listOfData = new List<(string, DataObject)>
            {
                (LobbyDataKey.IsLobbyReady,
                    new DataObject(DataObject.VisibilityOptions.Public, "true", DataObject.IndexOptions.S2))
            };
            
            await _lobbySystem.UpdateLobbyData(listOfData);
        }
    }
}