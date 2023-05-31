using System;
using AYellowpaper.SerializedCollections;
using Maniac.LanguageTableSystem;
using Newtonsoft.Json;
using UnityEngine;

namespace Resource.DatabaseConfigs.Weapons
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
        [JsonIgnore]
        public WeaponInfo Info;
        public SerializedDictionary<WeaponTier,WeaponStats> AllLevelStats;
        public int MagCapacity;
        public int NumOfMags;
        
        public WeaponStats GetStatsByTier(WeaponTier tier)
        {
            if (!AllLevelStats.ContainsKey(tier)) return null;
                
            return AllLevelStats[tier];
        }
    }

    [Serializable]
    public class WeaponInfo
    {
        public LanguageItem WeaponNameLangItem;
        public string WeaponName => WeaponNameLangItem != null ? WeaponNameLangItem.GetCurrentLanguageText() : "";

        public Sprite WeaponSprite;
    }

    [Serializable]
    public class WeaponStats
    {
        public float Damage;
        public float ReloadTimeInSeconds;
        [Tooltip("// How many bullets per second")]
        public float FireRate; 
        public float BulletSpeed;
        public float FireRange;
        public float Accuracy; // 0-1
        public float CriticalChance; // 0-1
        public float CriticalDamageMultiplier; 
        public float KnockbackDistance; 
        
        [JsonIgnore]
        public float TimeBetweenShotsInSeconds
        {
            get
            {
                if (FireRate != 0)
                    return 1 / FireRate;
                else
                {
                    Debug.LogError("FireRate is 0");
                    return 0;
                }
            }
        }
    }
}