using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using AYellowpaper.SerializedCollections;
using Maniac.Utils;
using Game.Enums;

namespace Maniac.DataBaseSystem
{
    public class MonsterConfig : DataBaseConfig
    {
        /// <summary>
        /// ID quái
        /// </summary>
        public string ID;
        /// <summary>
        /// Thông tin chỉ số
        /// </summary>
        public SerializedDictionary<string, MonsterStats> DicMonsterStats;
        /// <summary>
        /// Tùy theo độ khó để sinh quái
        /// </summary>
        public SerializedDictionary<Difficulty, MonsterSwapInfo> DificultyMonster;
    }
    
  

}
