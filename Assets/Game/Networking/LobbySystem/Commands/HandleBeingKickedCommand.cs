using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.Utils;
using UnityEngine;

namespace Game.Networking.LobbySystem.Commands
{
    public class HandleBeingKickedCommand : Command
    {
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;
        
        public override async UniTask Execute()
        {
            bool isBeingKicked = _lobbySystem.JoinedLobby.Value == null;
            if (isBeingKicked)
            {
                Debug.Log("You have been KICKED");
                
            }
        }
    }
}