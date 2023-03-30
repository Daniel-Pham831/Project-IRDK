using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Game.Networking.LobbySystem;
using Game.Scripts;
using Maniac.UISystem;
using Maniac.Utils;

namespace Game
{
    public class LobbyScreen : BaseUI
    {
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;

        [SerializeField] private LobbyControllerInScreen lobbyController;
        
        public async void OnCreateClicked()
        {
            Debug.Log("OnCreateClicked");
            await _lobbySystem.CreateLobby();
        }

        public async void OnJoinClicked()
        {
            Debug.Log("OnJoinClicked");
        }

        public async void OnQuickJoinClicked()
        {
            Debug.Log("OnQuickJoinClicked");
        }

        public async void OnRefreshClicked()
        {
            Debug.Log("OnRefreshClicked");
        }
    }
}
