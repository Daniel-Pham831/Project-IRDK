using Cysharp.Threading.Tasks;
using Maniac.Services;
using UnityEngine;

namespace Maniac.Bootstrap.Scripts
{
    public class LimitFrameRateService : Service
    {
        public override async UniTask<IService.Result> Execute()
        {
            Application.targetFrameRate = 60;
            return IService.Result.Success;
        }
    }
}