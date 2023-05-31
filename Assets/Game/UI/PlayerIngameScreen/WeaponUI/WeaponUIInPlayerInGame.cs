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
        [SerializeField] private Slider weaponReloadProgressSlider;

        private List<IDisposable> reactiveListeners = new List<IDisposable>();

        public async UniTask Init()
        {
            _weaponController = _netPlayer.NetPlayerWeaponController;
            _weaponController.CurrentWeapon.Subscribe(SetupWeapon).AddTo(this);
        }

        private void OnDestroy()
        {
            DisposeAmmoListeners();
        }

        public void SetupWeapon(Weapon weapon)
        {
            if (weapon == null || weapon.WeaponData == null) return;

            var weaponData = weapon.WeaponData;
            weaponImage.sprite = weaponData.Info.WeaponSprite;
            weaponNameTxt.text = weaponData.Info.WeaponName;

            SetupEvents(weapon);
        }

        private void SetupEvents(Weapon weapon)
        {
            DisposeAmmoListeners();
            // Ammo
            var ammoListener = weapon.Ammo.Subscribe(value =>
            {
                weaponAmmoTxt.text = string.Format(_ammoFormat, value,
                    weapon.IsWeaponHasInfinityAmmo ? _infinitySymbol : weapon.TotalAmmo.Value);
            });

            var totalAmmoListener = weapon.TotalAmmo.Subscribe(value =>
            {
                weaponAmmoTxt.text = string.Format(_ammoFormat, weapon.Ammo.Value,
                    weapon.IsWeaponHasInfinityAmmo ? _infinitySymbol : value);
            });

            reactiveListeners.Add(ammoListener);
            reactiveListeners.Add(totalAmmoListener);
            
            // Reload
            var isReloadProgressListener = weapon.ReloadCooldown.IsOnCooldown.Subscribe(value =>
            {
                weaponReloadProgressSlider.gameObject.SetActive(value);
            });
            var reloadProgressListener = weapon.ReloadCooldown.DurationLeftPercent.Subscribe(value =>
            {
                weaponReloadProgressSlider.value = value;
            });
            
            reactiveListeners.Add(isReloadProgressListener);
            reactiveListeners.Add(reloadProgressListener);
        }

        public void DisposeAmmoListeners()
        {
            reactiveListeners.ForEach(x => x.Dispose());
            reactiveListeners.Clear();
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