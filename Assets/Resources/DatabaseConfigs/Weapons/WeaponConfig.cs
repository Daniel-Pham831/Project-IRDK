using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Game.Weapons;
using Maniac.DataBaseSystem;
using Maniac.Utils;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Resource.DatabaseConfigs.Weapons
{
    public class WeaponConfig : DataBaseConfig
    {
        [ValueDropdown("GetAllWeaponIds")]
        public string DefaultWeaponId;
        public List<WeaponData> WeaponDatas;
        [JsonIgnore]
        public SerializedDictionary<string, Weapon> WeaponPrefabs;

        [JsonIgnore]
        private Dictionary<string, WeaponData> _weaponDataCache = new Dictionary<string, WeaponData>();
        
        public WeaponData GetWeaponDataById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return GetWeaponDataById(DefaultWeaponId);
            
            if (!_weaponDataCache.ContainsKey(id))
            {
                var data = WeaponDatas.FirstOrDefault(x => x.Id == id);
                if (data != null)
                {
                    _weaponDataCache.Add(id, data);
                }
                else
                {
                    Debug.LogError("WeaponData with id: " + id + " not found in WeaponConfig");
                    return null;
                }
            }
            
            return _weaponDataCache[id].CloneByExpressionTree();
        }
        
        public List<string> GetAllWeaponIds()
        {
            return WeaponDatas.Select(x => x.Id).ToList();
        }

        public Weapon GetWeaponPrefab(string weaponId)
        {
            if (string.IsNullOrEmpty(weaponId))
                return GetWeaponPrefab(DefaultWeaponId);
            
            if (!WeaponPrefabs.ContainsKey(weaponId))
            {
                Debug.LogError("WeaponPrefab with id: " + weaponId + " not found in WeaponConfig");
                return null;
            }

            return WeaponPrefabs[weaponId];
        }
    }
}
