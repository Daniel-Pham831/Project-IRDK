using System;
using Cysharp.Threading.Tasks;
using Game.Commands;
using Maniac.Command;
using Maniac.UISystem;
using Maniac.Utils;
using UnityEngine;

namespace Game.Networking.LobbySystem.Commands
{
    public class JoinLobbyByCodeCommand : Command
    {
        private readonly string _joinCode;
        private readonly Action _onSuccess;
        private readonly Action _onFail;
        
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;

        public JoinLobbyByCodeCommand(string joinCode, Action onSuccess, Action onFail)
        {
            _joinCode = joinCode;
            _onSuccess = onSuccess;
            _onFail = onFail;
        }

        public override async UniTask Execute()
        {
            // await new ShowConnectToServerCommand().Execute();
            // var joinedLobby = await _lobbySystem.JoinLobbyByCode(_joinCode);
            // await new HideConnectToServerCommand().Execute();
            //
            // if (joinedLobby != null)
            // {
            //     await new ShowLobbyRoomDetailScreenCommand().Execute();
            //     _onSuccess?.Invoke();
            // }
            // else
            //     _onFail?.Invoke();
        }
    }
}