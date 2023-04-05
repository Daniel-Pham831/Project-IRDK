﻿using System;
using Cysharp.Threading.Tasks;
using Game.Commands;
using Maniac.Command;
using Maniac.Utils;

namespace Game.Networking.Lobby.Commands
{
    public class JoinLobbyByCodeCommand : Command
    {
        private readonly string _lobbyId;
        private readonly Action _onSuccess;
        private readonly Action _onFail;
        
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;

        public JoinLobbyByCodeCommand(string lobbyId, Action onSuccess, Action onFail)
        {
            _lobbyId = lobbyId;
            _onSuccess = onSuccess;
            _onFail = onFail;
        }

        public override async UniTask Execute()
        {
            await new ShowConnectToServerCommand().Execute();
            var joinedLobby = await _lobbySystem.JoinLobbyByCode(_lobbyId);
            await new HideConnectToServerCommand().Execute();
            
            if (joinedLobby != null)
            {
                await new ShowLobbyRoomDetailScreenCommand().Execute();
                _onSuccess?.Invoke();
            }
            else
                _onFail?.Invoke();
        }
    }
}