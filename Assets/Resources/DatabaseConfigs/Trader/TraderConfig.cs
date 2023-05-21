using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using Maniac.Utils;

namespace Maniac.DataBaseSystem.Trader
{
    public class TraderConfig : DataBaseConfig
    {
        public List<WeaponPriceData> WeaponPriceDatas;
        
        private Dictionary<string, WeaponPriceData> _weaponPriceDataCache = new Dictionary<string, WeaponPriceData>();
        
        public WeaponPriceData GetWeaponPriceDataByWeaponId(string weaponId)
        {
            if (!_weaponPriceDataCache.ContainsKey(weaponId))
            {
                var data = WeaponPriceDatas.FirstOrDefault(x => x.WeaponId == weaponId);
                if (data != null)
                {
                    _weaponPriceDataCache.Add(weaponId, data);
                }
                else
                {
                    Debug.LogError("WeaponPriceData with id: " + weaponId + " not found in TraderConfig");
                    return null;
                }
            }
            
            return _weaponPriceDataCache[weaponId].CloneByExpressionTree();
        }
    }
}
