using System;
using Cysharp.Threading.Tasks;
using Game.Commands;
using Maniac.Command;
using Maniac.UISystem.Command;
using Maniac.Utils;

namespace Game.Networking.LobbySystem.Commands
{
    public class CreateNewLobbyCommand : Command
    {
        private readonly Action _onSuccess;
        private readonly Action _onFail;
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;

        public CreateNewLobbyCommand(Action onSuccess, Action onFail)
        {
            _onSuccess = onSuccess;
            _onFail = onFail;
        }

        public override async UniTask Execute()
        {
            var lobbyName = (string)await ShowScreenCommand.Create<CreateLobbyScreen>().ExecuteAndReturnResult();

            await new ShowConnectToServerCommand().Execute();
            var createdLobby = await _lobbySystem.CreateLobby(lobbyName);
            await new HideConnectToServerCommand().Execute();

            if (createdLobby != null)
            {
                await new ShowLobbyRoomDetailScreenCommand().Execute();
                _onSuccess?.Invoke();
            }
            else
                _onFail?.Invoke();
        }
    }
}