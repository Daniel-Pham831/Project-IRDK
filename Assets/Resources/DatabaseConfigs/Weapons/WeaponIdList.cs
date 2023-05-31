using System;
using System.Collections;
using AYellowpaper.SerializedCollections.KeysGenerators;
using Maniac.DataBaseSystem;

namespace Resource.DatabaseConfigs.Weapons
{
    [KeyListGenerator("Weapon Id",typeof(string))]
    public class WeaponIdList : KeyListGenerator
    {
        private DataBase _database;
        private WeaponConfig _weaponConfig;

        public override IEnumerable GetKeys(Type type)
        {
#if UNITY_EDITOR
            _database ??= DataBase.ActiveDatabase;
            _weaponConfig ??= _database.GetConfig<WeaponConfig>();

            var ids = _weaponConfig.GetAllWeaponIds();
            foreach (var id in ids)
            {
                yield return id;
            }
#endif
        }
    }
}