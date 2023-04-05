using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Commands;
using Maniac.ProfileSystem;
using Maniac.Utils;
using Maniac.Utils.Extension;
using Newtonsoft.Json;
using Unity.Services.CloudSave;
using UnityEngine;

namespace Game.CloudProfileSystem
{
    public class CloudProfileManager
    {
        private ProfileManager _profileManager => Locator<ProfileManager>.Instance;
        
        private readonly Dictionary<string, CloudProfileRecord> _recordsCache = new Dictionary<string, CloudProfileRecord>();
        private readonly HashSet<string> _hasChangedKey = new HashSet<string>();
        
        public async UniTask Init()
        {
            await PreLoadAllCloudProfileRecordsIntoCache();
        }
        
        private async UniTask PreLoadAllCloudProfileRecordsIntoCache()
        {
            Dictionary<string, Type> allCloudProfileTypes = GetListTypeBaseOnProfile();

            foreach (var profileType in allCloudProfileTypes)
            {
                await Load(profileType.Value);
            }
        }

        private Dictionary<string, Type> GetListTypeBaseOnProfile()
        {
            Dictionary<string, Type> objects = new Dictionary<string, Type>();
            IEnumerable<Type> types = typeof(CloudProfileRecord).GetAllSubclasses();
            foreach (Type type in types)
            {
                objects.Add(type.Name, type);
            }

            return objects;
        }

        public async UniTask<T> Get<T>() where T : CloudProfileRecord
        {
            var key = typeof(T).Name;
            if (_recordsCache.TryGetValue(key, out CloudProfileRecord value) && !_hasChangedKey.Contains(key))
                return value as T;

            return await Load<T>();
        }

        public void CheckHasChangedKey(CloudProfileRecord record)
        {
            if (!_hasChangedKey.Contains(record.Key))
                _hasChangedKey.Add(record.Key);
        }
        
        public async UniTask<T> Load<T>() where T : CloudProfileRecord
        {
            var key = typeof(T).Name;
            var savedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string>{key});
            var hasCloudData = savedData.Count != 0 & savedData.ContainsKey(key);

            T result = null;
            if (hasCloudData)
            {
                var json = savedData[key];
                
                try
                {
                    result = JsonConvert.DeserializeObject<T>(json,Helper.SerializerSettings);
                }
                catch
                {
                    // ignored
                }
            }
            
            // in case there is no cloud data, create one, and up to cloud
            if(result == null)
            {
                result = Activator.CreateInstance<T>();
                await Save(result);
            }

            SaveCache(result);
            return result;
        }
        
        public async UniTask<CloudProfileRecord> Load(Type type)
        {
            var key = type.Name;
            var savedData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string>{key});
            var hasCloudData = savedData.Count != 0 & savedData.ContainsKey(key);

            CloudProfileRecord result = null;
            if (hasCloudData)
            {
                var json = savedData[key];
                
                try
                {
                    result = JsonUtility.FromJson(json,type) as CloudProfileRecord;
                }
                catch
                {
                    // ignored
                }
            }
            
            // in case there is no cloud data, create one, and up to cloud
            if(result == null)
            {
                result = Activator.CreateInstance(type) as CloudProfileRecord;
                await Save(result);
            }

            SaveCache(result);
            return result;
        }
        
        public async UniTask<bool> Save(CloudProfileRecord record)
        {
            bool result = false;
            try
            {
                var data = new Dictionary<string, object>{{record.Key, record}};
                await CloudSaveService.Instance.Data.ForceSaveAsync(data);

                ClearHasChangedKey(record);
                SaveCache(record);
                LocalSave(record);
                result = true;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return result;
        }

        private void LocalSave(CloudProfileRecord record)
        {
            _profileManager.Save(record);
        }

        private void ClearHasChangedKey(CloudProfileRecord record)
        {
            if (_hasChangedKey.Contains(record.Key))
                _hasChangedKey.Remove(record.Key);
        }

        private void SaveCache(CloudProfileRecord record)
        {
            string key = record.Key;
            if (!_recordsCache.ContainsKey(key))
            {
                _recordsCache.Add(key, record);
            }

            _recordsCache[key] = record;
        }
    }
}