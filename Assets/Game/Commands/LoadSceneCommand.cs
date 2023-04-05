using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.UISystem;
using Maniac.Utils;
using UnityEngine.SceneManagement;

namespace Game.Commands
{
    public class LoadSceneCommand: Command
    {
        private readonly Param param;
        private LoadingScreen loadingScreen;

        public LoadSceneCommand(Param param)
        {
            this.param = param;
        }

        private UIManager uiManager => Locator<UIManager>.Instance;

        public override async UniTask Execute()
        {
            var mainGameScene = SceneManager.LoadSceneAsync(param.sceneName);
            mainGameScene.allowSceneActivation = false;

            if (param.shouldLoadWithLoadingScreen)
            {
                loadingScreen = await uiManager.Show<LoadingScreen>() as LoadingScreen;
                do
                {
                    await UniTask.Delay(100);
                    loadingScreen.UpdateProgressBar(mainGameScene.progress);
                } while (mainGameScene.progress < 0.9f);

                loadingScreen.UpdateProgressBar(1);
            }
            else
            {
                await UniTask.WaitUntil(() => mainGameScene.progress >= 0.9f);
            }

            await UniTask.Delay(2000);
            mainGameScene.allowSceneActivation = true;
            if (loadingScreen != null)
                await loadingScreen.Close();

            await UniTask.CompletedTask;
        }

        public class Param
        {
            public string sceneName;
            public bool shouldLoadWithLoadingScreen;

            public Param(string sceneName, bool shouldLoadWithLoadingScreen)
            {
                this.sceneName = sceneName;
                this.shouldLoadWithLoadingScreen = shouldLoadWithLoadingScreen;
            }
        }
    }
}