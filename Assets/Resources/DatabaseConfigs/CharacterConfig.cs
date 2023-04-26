using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using UnityEngine.U2D.Animation;

namespace Maniac.DataBaseSystem
{
    public class CharacterConfig : DataBaseConfig
    {
        private string _defaultCharacterId = "default";
        public List<CharacterInfo> CharacterInfos;

        public string GetDefaultCharacterId()
        {
            var firstCharacter = CharacterInfos.FirstOrDefault();
            if(firstCharacter != null && _defaultCharacterId != firstCharacter.Id)
                _defaultCharacterId = firstCharacter.Id;

            return _defaultCharacterId;
        }

        public CharacterInfo GetCharacterInfo(string oldCharacterId)
        {
            var result = CharacterInfos.FirstOrDefault(c => c.Id == oldCharacterId) ?? CharacterInfos[0];
            return result;
        }
    }

    [Serializable]
    public class CharacterInfo
    {
        public string Id;
        public Sprite sprite;
        public SpriteLibraryAsset spriteLibraryAsset;
    }
}
