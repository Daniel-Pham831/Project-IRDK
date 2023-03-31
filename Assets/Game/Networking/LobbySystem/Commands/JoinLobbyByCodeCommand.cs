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
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;

        public JoinLobbyByCodeCommand(string joinCode)
        {
            _joinCode = joinCode;
        }

        public override async UniTask Execute()
        {
            var joinedLobby = await _lobbySystem.JoinLobbyByCode(_joinCode);

            if (joinedLobby != null)
            {
                // ShowLobbyRoomDetailScreen
                await new ShowLobbyRoomDetailScreenCommand().Execute();
            }
            else
            {
                // show join fail
            }
        }
    }
}