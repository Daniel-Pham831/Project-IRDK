using Cysharp.Threading.Tasks;
using Maniac.DataBaseSystem;
using Maniac.Services;
using Maniac.Utils;
using Maniac.Utils.Extension;
using Unity.Services.RemoteConfig;
using UnityEngine;

namespace Game.Services.UnityServices
{
    public class InitRemoteConfigService : Service
    {
        private DataBase _dataBase => Locator<DataBase>.Instance;
        
        public struct userAttributes {}
        public struct appAttributes {}

        private bool _isCompleted = false;
        private IService.Result _result;

        public override async UniTask<IService.Result> Execute()
        {
            var remoteConfigService = RemoteConfigService.Instance;

            remoteConfigService.FetchConfigs(new userAttributes(), new appAttributes());

            remoteConfigService.FetchCompleted += ApplyRemoteSettings;

            await UniTask.WaitUntil(() => _isCompleted);
            return _result;
        }
        
        private async void ApplyRemoteSettings(ConfigResponse configResponse)
        {
            if (configResponse.status == ConfigRequestStatus.Success)
            {
                OverrideLocalDataBaseConfigs();

                _result = IService.Result.Success;
            }
            else
            {
                _result = IService.Result.Fail;
            }

            _isCompleted = true;
        }

        private void OverrideLocalDataBaseConfigs()
        {
            var runtimeConfig = RemoteConfigService.Instance.appConfig;
            var remoteKeys = runtimeConfig.GetKeys();

            foreach (var key in remoteKeys)
            {
                var json = runtimeConfig.GetJson(key);
                if (json == string.Empty || json == "{}") continue;
                
                var isSuccess = _dataBase.OverrideConfigWithRemoteConfig(key, json);

                string log = isSuccess ? $"{key.AddColor("#E0B4DE")} - {"SUCCESS".AddColor(Color.green)}" : $"{key.AddColor("#E0B4DE")} - {"FAIL".AddColor(Color.red)}";
                
                Debug.Log($"{"[Override]".AddColor("#E29075")} {log}");
            }
        }
    }
}