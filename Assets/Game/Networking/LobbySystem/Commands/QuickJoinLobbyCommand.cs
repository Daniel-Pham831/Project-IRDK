using System;
using Cysharp.Threading.Tasks;
using Game.Commands;
using Maniac.Command;
using Maniac.Utils;
using Unity.Services.Lobbies;

namespace Game.Networking.LobbySystem.Commands
{
    public class QuickJoinLobbyCommand : Command
    {
        private readonly string _joinCode;
        private readonly Action _onSuccess;
        private readonly Action _onFail;
        
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;
        
        public QuickJoinLobbyCommand(Action onSuccess, Action onFail)
        {
            _onSuccess = onSuccess;
            _onFail = onFail;
        }


        public override async UniTask Execute()
        {
            await new ShowConnectToServerCommand().Execute();
            var joinedLobby = await _lobbySystem.QuickJoinLobby();
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