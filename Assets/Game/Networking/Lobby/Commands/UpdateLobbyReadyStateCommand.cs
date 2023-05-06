﻿using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.Utils;
using Unity.Services.Lobbies.Models;

namespace Game.Networking.Lobby.Commands
{
    public class UpdateLobbyReadyStateCommand : Command
    {
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;
        
        public override async UniTask Execute()
        {
            if (!_lobbySystem.AmITheHost()) return;
            
            var listOfData = new List<(string, string, DataObject.VisibilityOptions)>
            {
                (LobbyDataKey.IsLobbyReady,"true",DataObject.VisibilityOptions.Public)
            };
            
            await _lobbySystem.UpdateLobbyData(listOfData);
        }
    }
}