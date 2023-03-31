using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Commands;
using Game.Networking;
using Maniac.Services;
using Maniac.Utils;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using UnityEngine;

namespace Game.Services.UnityServices
{
    public class InitUpdatePlayerNameService : Service
    {
        private LocalData LocalData => Locator<LocalData>.Instance;

        public override async UniTask<IService.Result> Execute()
        {
            var userName = await new FetchCSDataCommand<string>(CloudSaveKey.UserName).ExecuteAndGetResult();

            if (!string.IsNullOrEmpty(userName))
            {
                LocalData.LocalPlayer.DisplayName = userName;
            }

            return IService.Result.Success;
        }
    }
}