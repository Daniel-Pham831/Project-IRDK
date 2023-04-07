using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.UISystem;
using Maniac.Utils;
using UnityEngine.SceneManagement;

namespace Game.Commands
{
    public class LoadSceneCommand: Command
    {
        private readonly string _sceneName;

        public LoadSceneCommand(string sceneName)
        {
            _sceneName = sceneName;
        }

        public override async UniTask Execute()
        {
            var loadedScene = SceneManager.LoadSceneAsync(_sceneName);
            loadedScene.allowSceneActivation = false;
            await UniTask.WaitUntil(() => loadedScene.progress >= 0.9f);
            loadedScene.allowSceneActivation = true;
            await UniTask.CompletedTask;
        }
    }
}