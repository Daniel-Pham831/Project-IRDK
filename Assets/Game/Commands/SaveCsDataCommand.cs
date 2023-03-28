using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Maniac.Command;
using Unity.Services.CloudSave;

namespace Game.Commands
{
    public class SaveCsDataCommand : Command
    {
        private readonly string _key;
        private readonly object _saveObj;

        public SaveCsDataCommand(string key,object saveObj)
        {
            _key = key;
            _saveObj = saveObj;
        }
        
        public override async UniTask Execute()
        {
            var data = new Dictionary<string, object>{{_key, _saveObj}};
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
        }
    }
}