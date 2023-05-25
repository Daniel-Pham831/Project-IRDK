using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Players.Scripts;
using Game.Weapons;
using Maniac.Utils;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.WeaponUI
{
    public class WeaponUIInPlayerInGame : MonoBehaviour
    {
        private readonly string _ammoFormat = "<b><size=60>{0}</size></b>/{1}";
        private string _infinitySymbol => Helper.InfinitySymbol;

        private NetPlayer _netPlayer => Locator<NetPlayer>.Instance;
        private NetPlayerWeaponController _weaponController;

        [SerializeField] private Image weaponImage;
        [SerializeField] private TMP_Text weaponNameTxt;
        [SerializeField] private TMP_Text weaponAmmoTxt;

        private List<IDisposable> _ammoListeners = new List<IDisposable>();

        public async UniTask Init()
        {
            _weaponController = _netPlayer.NetPlayerWeaponController;
            _weaponController.CurrentWeapon.Subscribe(SetupWeapon).AddTo(this);
        }

        public void SetupWeapon(Weapon weapon)
        {
            if(weapon == null || weapon.WeaponData == null) return;
            
            var weaponData = weapon.WeaponData;
            weaponImage.sprite = weaponData.Info.WeaponSprite;
            weaponNameTxt.text = weaponData.Info.WeaponName;

            DisposeAmmoListeners();
            if (weapon.IsWeaponHasInfinityAmmo)
            {
                weaponAmmoTxt.text = string.Format(_ammoFormat, _infinitySymbol, _infinitySymbol);
            }
            else
            {
                var ammoListener = weapon.Ammo.Subscribe(value =>
                {
                    weaponAmmoTxt.text = string.Format(_ammoFormat, value, weapon.TotalAmmo.Value);
                });
            
                var totalAmmoListener = weapon.TotalAmmo.Subscribe(value =>
                {
                    weaponAmmoTxt.text = string.Format(_ammoFormat, weapon.Ammo.Value, value);
                });
            
                _ammoListeners.Add(ammoListener);
                _ammoListeners.Add(totalAmmoListener);
            }
        }

        public void DisposeAmmoListeners()
        {
            _ammoListeners.ForEach(x => x.Dispose());
            _ammoListeners.Clear();
        }
        
        public void OnNextWeapon()
        {
            _weaponController.NextWeapon();
        }
        
        public void OnPreviousWeapon()
        {
            _weaponController.PreviousWeapon();
        }
    }
}