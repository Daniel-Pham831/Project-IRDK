using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;

namespace Maniac.DataBaseSystem
{
    public class CharacterConfig : DataBaseConfig
    {
        public List<CharacterInfo> CharacterInfos;
    }

    [Serializable]
    public class CharacterInfo
    {
        public string Id;
        public Sprite sprite;
    }
}
