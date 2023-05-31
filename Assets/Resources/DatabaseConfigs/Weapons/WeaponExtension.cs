using Game.Weapons;

namespace Resource.DatabaseConfigs.Weapons
{
    public static class WeaponExtension
    {
        public static BulletData ToBulletData(this WeaponStats weaponStats)
        {
            return new BulletData()
            {
                Damage = weaponStats.Damage,
                MoveSpeed = weaponStats.BulletSpeed,
                CriticalChance = weaponStats.CriticalChance,
                CriticalDamageMultiplier = weaponStats.CriticalDamageMultiplier,
                KnockBackDistance = weaponStats.KnockbackDistance
            };
        }
    }
}