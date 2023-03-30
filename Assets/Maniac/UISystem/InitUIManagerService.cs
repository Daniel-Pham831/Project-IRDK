using Cysharp.Threading.Tasks;
using Maniac.Services;
using Maniac.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Maniac.UISystem
{
    public class InitUIManagerService : Service
    {
        private readonly UIData _uiData;
        private readonly UIManager _uiManagerPrefab;

        public InitUIManagerService(UIData uiData,UIManager uiManagerPrefab)
        {
            _uiData = uiData;
            _uiManagerPrefab = uiManagerPrefab;
        }
        
        public override async UniTask<IService.Result> Execute()
        {
            var uiManager = Object.Instantiate(_uiManagerPrefab);
            uiManager.name = "UI Manager";

            var unityEventSystem = new GameObject("Unity Event System");
            unityEventSystem.AddComponent<StandaloneInputModule>();
            
            Object.DontDestroyOnLoad(unityEventSystem);
            
            Locator<UIData>.Set(_uiData);
            Locator<UIManager>.Set(uiManager);
            
            uiManager.Init();
            
            Object.DontDestroyOnLoad(uiManager);
            return IService.Result.Success;
        }
    }
}