using System;
using Cysharp.Threading.Tasks;
using Maniac.Services;
using UnityEngine;

namespace Game.Services.UnityServices
{
    public class InitUnityServicesService : Service
    {
        public override async UniTask<IService.Result> Execute()
        {
            IService.Result result = IService.Result.Fail;
            try
            {
                await Unity.Services.Core.UnityServices.InitializeAsync();
                result = IService.Result.Success;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            
            return result;
        }
    }
}