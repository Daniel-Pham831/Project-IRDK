using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Game.Networking.Lobby;
using Game.Scripts;
using Maniac.UISystem;
using Maniac.Utils;
using UniRx;

namespace Game
{
    public class LobbyPlayersDetailsScreen : BaseUI
    {
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;

        [SerializeField] private PlayerItemControllerInLobbyRoomScreen controller;
        
        public override void OnSetup(object parameter = null) //first
        {
            _lobbySystem.JoinedLobby.Subscribe(value =>
            {
                controller.UpdateLobbyPlayerItems(value);
            }).AddTo(this);
        }
    }
}
