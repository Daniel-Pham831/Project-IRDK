using System.Collections.Generic;
using System.Linq;
using Maniac.DataBaseSystem;
using Maniac.Utils;
using UnityEngine;

namespace Maniac.DataBaseSystem.Weapon
{
    public class WeaponConfig : DataBaseConfig
    {
        public List<WeaponData> WeaponDatas;
        private Dictionary<string, WeaponData> _weaponDataCache = new Dictionary<string, WeaponData>();
        
        public WeaponData GetWeaponDataById(string id)
        {
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
    }
}
