using System;
using Cysharp.Threading.Tasks;
using Game.Networking.NetMessengerSystem.NetMessages;
using Maniac.Command;
using Maniac.Utils;
using Maniac.Utils.Extension;
using Unity.Collections;

namespace Game.Networking.NetMessengerSystem
{
    public class PreInitNetMessageCodesCommand : Command
    {
        public override async UniTask Execute()
        {
            var allSubClasses = typeof(NetMessage).GetAllSubclasses();
            ushort ushortCounter = 0;
            foreach (var subClassType in allSubClasses)
            {
                FixedString32Bytes fixedString32Bytes = subClassType.Name;
                NetMessageCode.Add(ushortCounter++, fixedString32Bytes,subClassType);
            }
        }
    }
}