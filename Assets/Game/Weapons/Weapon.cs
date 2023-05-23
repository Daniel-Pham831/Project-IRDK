using System.Collections.Generic;
using Maniac.CoolDownSystem;
using Maniac.DataBaseSystem;
using Maniac.DataBaseSystem.Weapon;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Weapons
{
    public class Weapon : MonoBehaviour
    {
        public string WeaponId => weaponId;
        
        [ValueDropdown("GetAllWeaponIds")]
        [SerializeField] private string weaponId;
        [SerializeField] private Transform rotator;
        [SerializeField] private SpriteRenderer graphics;
        [SerializeField] private Transform firePoint;

        public WeaponStats Stats { get; private set; } = null;

        private Cooldown _fireCoolDown;
        private Vector2 _oldInputDirection;

        public void SetStats(WeaponStats stats)
        {
            Stats = stats;
            SetupWeaponData();
        }

        protected virtual void SetupWeaponData()
        {
            _fireCoolDown ??= new Cooldown();
            _fireCoolDown.ChangeDuration(Stats.TimeBetweenShots);
        }

        public void Attack()
        {
            throw new System.NotImplementedException();
        }
        
        public void Rotate(Vector2 inputDirection)
        {
            if (inputDirection == Vector2.zero)
                return;
            
            var angle = Vector2.SignedAngle(rotator.right, inputDirection);
            rotator.Rotate(Vector3.forward,angle);
            
            var localScale = rotator.localScale;
            localScale = new Vector3( localScale.x,inputDirection.x > 0 ? 1 : inputDirection.x < 0 ? -1 : rotator.localScale.y, localScale.z);
            rotator.localScale = localScale;
        }

#if UNITY_EDITOR
        private List<string> GetAllWeaponIds()
        {
            return DataBase.ActiveDatabase.GetConfig<WeaponConfig>().GetAllWeaponIds();
        }
#endif
    }
}