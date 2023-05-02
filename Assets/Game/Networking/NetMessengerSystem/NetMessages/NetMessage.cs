using System;
using Maniac.Utils;
using MemoryPack;
using Unity.Collections;

namespace Game.Networking.NetMessengerSystem.NetMessages
{
    public interface INetMessage
    {
        public FixedString32Bytes Type { get; }
    }
    
    [Serializable]
    [MemoryPackable]
    public partial class NetMessage : INetMessage
    {
        public FixedString32Bytes Type => GetType().Name;
    }
    
    public static class NetMessageExtensions
    {
        public static byte[] ToBytes<T>(this T message) where T : NetMessage
        {
            return Helper.Serialize(message);
        }
    }
}