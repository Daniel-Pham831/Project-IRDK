using Cysharp.Threading.Tasks;
using Maniac.ProfileSystem;
using Maniac.Utils;
using Maniac.Utils.Extension;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.CloudProfileSystem
{
    public abstract class CloudProfileRecord : ProfileRecord
    {
        [JsonIgnore]
        public string Key => GetType().Name;
        private CloudProfileManager _cloudProfileManager => Locator<CloudProfileManager>.Instance;
        
        public override async UniTask Save()
        {
            var isSuccess = await _cloudProfileManager.Save(this);
            
            if (isSuccess)
            {
                Debug.Log($"CloudSave:{GetType().Name.AddColor("#Cb9ce0")} - {"Success".AddColor(Color.yellow)}");
            }
            else
            {
                Debug.Log($"CloudSave:{GetType().Name.AddColor("#9025be")} - {"Fail".AddColor(Color.red)}-Please Check!");
            }
        }

        public virtual void HasChanged()
        {
            _cloudProfileManager.CheckHasChangedKey(this);
        }
    }
}