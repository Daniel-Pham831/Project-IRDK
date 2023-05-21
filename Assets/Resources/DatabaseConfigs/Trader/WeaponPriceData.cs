using System;
using System.Collections.Generic;
using Maniac.DataBaseSystem.Weapon;
using Sirenix.OdinInspector;

namespace Maniac.DataBaseSystem.Trader
{
    [Serializable]
    public class WeaponPriceData
    {
        [ValueDropdown("GetAllWeaponIds")]
        public string WeaponId;
        public WeaponTier Tier;
        public int Price;

#if UNITY_EDITOR
        private List<string> GetAllWeaponIds()
        {
            return DataBase.ActiveDatabase.GetConfig<WeaponConfig>().GetAllWeaponIds();
        }
#endif
    }
}