using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.Utils;
using Newtonsoft.Json;
using Unity.Services.CloudSave;
using UnityEngine;

namespace Game.Commands
{
    public class FetchCSDataCommand<T> : ResultCommand<T> where T : class
    {
        private readonly string _key;

        public FetchCSDataCommand(string key = "")
        {
            _key = key;
        }

        public override async UniTask Execute()
        {
            var savedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string>{_key});
            
            if (savedData.Count == 0 || !savedData.ContainsKey(_key))
            {
                return;
            }

            var json = savedData[_key];
            
            try
            {
                _result = JsonConvert.DeserializeObject<T>(json,Helper.SerializerSettings);
            }
            catch (Exception e)
            {
                _result = json as T;
            }
        }
    }
}