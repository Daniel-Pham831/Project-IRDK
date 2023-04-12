using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Maniac.UISystem;

namespace Game
{
    public class LobbyAccountDetailScreen : BaseUI
    {
        // This method will be call first, at this point, UI hasn't show up on canvas yet.
        // Use this to init your UI
        public override void OnSetup(object parameter = null) //first
        {
            base.OnSetup(parameter);
        }

        // This method is called during the UI transition. You can make custom transition after base.OnTransitionEnter(parameter);
        public override async UniTask OnTransitionEnter(object parameter = null) //second
        {
            await base.OnTransitionEnter(parameter);
        }

        // This method is called when TransitionEnter finished all of it transitions
        public override void OnShow(object parameter = null) // last
        {
            base.OnShow(parameter);
        }

        // This method is called during TransitionExit
        public override async UniTask OnTransitionExit()
        {
            await base.OnTransitionExit();
        }
    }
}
