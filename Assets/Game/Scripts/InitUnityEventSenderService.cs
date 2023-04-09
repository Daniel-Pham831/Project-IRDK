using Cysharp.Threading.Tasks;
using Maniac.Services;
using UnityEngine;

namespace Game.Scripts
{
    public class InitUnityEventSenderService : Service
    {
        public override async UniTask<IService.Result> Execute()
        {
            var newObj = new GameObject("Unity Event Sender");
            newObj.AddComponent<UnityEventFunctionSender>();
            Object.DontDestroyOnLoad(newObj);

            return IService.Result.Success;
        }
    }
}