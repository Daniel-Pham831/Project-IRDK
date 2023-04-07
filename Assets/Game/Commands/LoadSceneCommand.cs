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
            await SceneManager.LoadSceneAsync(_sceneName);
            await UniTask.CompletedTask;
        }
    }
}