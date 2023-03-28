using Cysharp.Threading.Tasks;
using Maniac.DataBaseSystem;
using Maniac.Services;
using Maniac.Utils;
using Maniac.Utils.Extension;
using Unity.Services.Authentication;
using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Game.Services.UnityServices
{
    public class InitRemoteConfigService : Service
    {
        private RemoteConfigService _remoteConfigService;
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private BuildSettingConfig _buildSettingConfig => _dataBase.Get<BuildSettingConfig>();
        public struct userAttributes {}
        public struct appAttributes {}

        private bool _isCompleted = false;
        private IService.Result _result;

        public override async UniTask<IService.Result> Execute()
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            _remoteConfigService = RemoteConfigService.Instance;
            Locator<RemoteConfigService>.Set(RemoteConfigService.Instance);

            _remoteConfigService.SetEnvironmentID(_buildSettingConfig.GetTargetEnvironmentID());
            _remoteConfigService.FetchConfigs(new userAttributes(), new appAttributes());

            _remoteConfigService.FetchCompleted += ApplyRemoteSettings;

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
            var runtimeConfig = _remoteConfigService.appConfig;
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