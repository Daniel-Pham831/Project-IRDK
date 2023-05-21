using Cysharp.Threading.Tasks;
using Maniac.DataBaseSystem;
using Maniac.DataBaseSystem.Weapon;
using Maniac.Utils;

namespace Game.Weapon
{
    public class WeaponSystem
    {
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private WeaponConfig _weaponConfig => _dataBase.GetConfig<WeaponConfig>();

        public async UniTask Init()
        {
            await UniTask.CompletedTask;
        }
        
        public void GiveWeaponToLocalPlayer(string weaponId)
        {  
            
        }
    }
}