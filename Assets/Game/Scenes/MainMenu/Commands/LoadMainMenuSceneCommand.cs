using Cysharp.Threading.Tasks;
using Game.Commands;
using Maniac.Command;
using Maniac.UISystem;
using Maniac.Utils;

namespace Game.Scenes.MainMenu.Commands
{
    public class LoadMainMenuSceneCommand : Command
    {
        private UIManager _uiManager => Locator<UIManager>.Instance;
        public override async UniTask Execute()
        {
            await new LoadEmptySceneCommand().Execute();
            await new LoadSceneCommand(SceneName.MainMenu).Execute();
            await _uiManager.Show<MainMenuScreen>();
        }
    }
}