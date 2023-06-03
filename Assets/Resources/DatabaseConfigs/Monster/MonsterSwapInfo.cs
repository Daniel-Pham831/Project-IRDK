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
    [Serializable]
    public class MonsterSwapInfo
    {
        public FloatRange Range;
        public SerializedDictionary<string, int> DicMonsterInfor;
    }
}
