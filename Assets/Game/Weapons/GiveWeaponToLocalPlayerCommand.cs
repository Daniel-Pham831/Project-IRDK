using Cysharp.Threading.Tasks;
using Game.Players.Scripts;
using Maniac.Command;
using Maniac.Utils;
using Resource.DatabaseConfigs.Weapons;

namespace Game.Weapons
{
    public class GiveWeaponToLocalPlayerCommand : Command
    {
        private WeaponSystem _weaponSystem => Locator<WeaponSystem>.Instance;
        private readonly string _weaponId;
        private readonly WeaponTier _tier;
        private NetPlayer _netPlayer => Locator<NetPlayer>.Instance;

        // weapon id == "" means give the default weapon
        public GiveWeaponToLocalPlayerCommand(string weaponId = "",WeaponTier tier = WeaponTier.Standard)
        {
            _weaponId = weaponId;
            _tier = tier;
        }
        
        public override async UniTask Execute()
        {
            var weapon = _weaponSystem.GetNewWeapon(_weaponId, _tier);
            var weaponController = _netPlayer.GetComponent<NetPlayerWeaponController>();
            weaponController.AddWeapon(weapon);
        }
    }
}