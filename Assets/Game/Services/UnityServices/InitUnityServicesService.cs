using System;
using Cysharp.Threading.Tasks;
using Maniac.DataBaseSystem;
using Maniac.Services;
using Maniac.Utils;
#if UNITY_EDITOR
using ParrelSync;
#endif
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

namespace Game.Services.UnityServices
{
    public class InitUnityServicesService : Service
    {
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private BuildSettingConfig _buildSettingConfig => _dataBase.GetConfig<BuildSettingConfig>();
        
        public override async UniTask<IService.Result> Execute()
        {
            string serviceProfileName = "user";
#if UNITY_EDITOR
            serviceProfileName = $"{serviceProfileName}_{ClonesManager.GetCurrentProject().name.Replace(" ","_")}";
#endif
            var profileOptions = new InitializationOptions();
            profileOptions.SetProfile(serviceProfileName);
            profileOptions.SetEnvironmentName(_buildSettingConfig.GetTargetEnvironmentName);
            
            IService.Result result = IService.Result.Fail;
            try
            {
                await Unity.Services.Core.UnityServices.InitializeAsync(profileOptions);
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