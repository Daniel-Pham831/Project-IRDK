using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Game.Networking.LobbySystem;
using Game.Networking.LobbySystem.Commands;
using Game.Scripts;
using Maniac.UISystem;
using Maniac.UISystem.Command;
using Maniac.Utils;

namespace Game
{
    public class LobbyScreen : BaseUI
    {
        public enum Result
        {
            None,
            Create,
            Join,
            QuickJoin,
        }
        
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;

        [SerializeField] private LobbyControllerInScreen lobbyController;
        
        public async void OnRefreshClicked()
        {
            Debug.Log("OnRefreshClicked");
        }
        
        public async void OnCreateClicked()
        {
            Debug.Log("OnCreateClicked");
            Close(Result.Create);
            await new CreateNewLobbyCommand().Execute();
        }

        public async void OnJoinClicked()
        {
            Debug.Log("OnJoinClicked");
            Close(Result.Join);
        }

        public async void OnQuickJoinClicked()
        {
            Debug.Log("OnQuickJoinClicked");
            Close(Result.QuickJoin);
        }

        public override async UniTask Close(object param = null)
        {
            await base.Close(Result.None);
        }
    }
}
