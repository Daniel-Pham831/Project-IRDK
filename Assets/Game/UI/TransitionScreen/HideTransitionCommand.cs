using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.UISystem;
using Maniac.Utils;

namespace Game
{
    public class HideTransitionCommand : Command
    {
        private UIManager _uiManager => Locator<UIManager>.Instance;
        
        public override async UniTask Execute()
        {
            var transitionScreen = _uiManager.GetShowedUI(typeof(TransitionScreen)) as TransitionScreen;
            if (transitionScreen != null)
            {
                await transitionScreen.Close();
            }
        }
    }
}