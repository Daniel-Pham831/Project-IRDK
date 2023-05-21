using System;
using System.Collections.Generic;
using System.Linq;
using Maniac.LanguageTableSystem;
using UnityEngine;

namespace Maniac.DataBaseSystem.Weapon
{
    public enum WeaponTier
    {
        Standard,
        Enhanced,
        Advanced
    }

    [Serializable]
    public class WeaponData
    {
        public string Id;
        public WeaponInfo Info;
        public List<WeaponStats> AllLevelStats;
        
        public WeaponStats GetStatsByTier(WeaponTier tier)
        {
            return AllLevelStats.FirstOrDefault(x => x.Tier == tier);
        }
    }

    [Serializable]
    public class WeaponInfo
    {
        public LanguageItem WeaponNameLangItem;
        public string WeaponName => WeaponNameLangItem != null ? WeaponNameLangItem.GetCurrentLanguageText() : "";

        public LanguageItem WeaponDescriptionLangItem;

        public string WeaponDescription =>
            WeaponDescriptionLangItem != null ? WeaponDescriptionLangItem.GetCurrentLanguageText() : "";

        public Sprite WeaponSprite;
    }

    [Serializable]
    public class WeaponStats
    {
        public WeaponTier Tier;
        
        public float Damage;
        public int MagCapacity; // Aka num of bullets per mag
        public int NumOfMags;
        public float ReloadTimeInSeconds;
        public float FireRate; // How many bullets per second
        public float BulletSpeed;
        public float FireRange;
        public float Accuracy; // 0-1
        public float CriticalChance; // 0-1
        public float CriticalDamageMultiplier; 
        public float KnockbackDistance; // 0-1
    }
}