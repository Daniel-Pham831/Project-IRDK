using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Players.Scripts;
using Maniac.DataBaseSystem;
using Maniac.SpawnerSystem;
using Maniac.Utils;
using Resource.DatabaseConfigs.Weapons;
using Unity.Tutorials.Core.Editor;

namespace Game.Weapons
{
    public class WeaponSystem
    {
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private WeaponConfig _weaponConfig => _dataBase.GetConfig<WeaponConfig>();
        private SpawnerManager _spawnerManager => Locator<SpawnerManager>.Instance;

        public async UniTask Init()
        {
            Locator<WeaponSystem>.Set(this,true);
        }

        public Weapon GetNewWeapon(string weaponId = "", WeaponTier tier = WeaponTier.Standard)
        {
            if (weaponId.IsNullOrEmpty())
                weaponId = _weaponConfig.DefaultWeaponId;
            
            var weaponPrefab = _weaponConfig.GetWeaponPrefab(weaponId);
            if (weaponPrefab == null) return null;

            var spawnedWeapon = _spawnerManager.Get(weaponPrefab);
            var weaponData = _weaponConfig.GetWeaponDataById(weaponId);

            spawnedWeapon.SetWeaponData(weaponData);
            return spawnedWeapon;
        }

        public List<Weapon> GetAllWeapon()
        {
            var result = new List<Weapon>();
            foreach (var weaponId in _weaponConfig.GetAllWeaponIds())
            {
                result.Add(GetNewWeapon(weaponId));
            }

            return result;
        }

        public Bullet GetNewBullet(string weaponId = "")
        {
            var bulletPrefab = _weaponConfig.GetBulletPrefab(weaponId);
            if (bulletPrefab == null) return null;

            var spawnedBullet = _spawnerManager.Get(bulletPrefab);
            return spawnedBullet;
        }
    }
}