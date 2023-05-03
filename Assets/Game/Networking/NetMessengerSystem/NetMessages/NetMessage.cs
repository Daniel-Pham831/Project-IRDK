using System;
using Maniac.Utils;
using MemoryPack;
using Unity.Collections;

namespace Game.Networking.NetMessengerSystem.NetMessages
{
    [Serializable]
    [MemoryPackable]
    public partial class NetMessage
    {
        public ulong SenderID;
    }
    
    public static class NetMessageExtensions
    {
        public static byte[] ToBytes<T>(this T message) where T : NetMessage
        {
            return Helper.Serialize(message);
        }
    }
}