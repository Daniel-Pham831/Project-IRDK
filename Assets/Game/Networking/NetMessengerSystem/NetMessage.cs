using System;
using Maniac.Utils;
using MemoryPack;

namespace Game.Networking.NetMessengerSystem
{
    public interface INetMessage
    {
        public string Type { get; }
    }
    
    [Serializable]
    [MemoryPackable]
    public partial class NetMessage : INetMessage
    {
        public string Type => GetType().Name;
    }
    
    public static class NetMessageExtensions
    {
        public static byte[] ToBytes<T>(this T message) where T : NetMessage
        {
            return Helper.Serialize(message);
        }

        public static T ToNetMessage<T>(this byte[] bytes) where  T : NetMessage,new()
        {
            return Helper.Deserialize<T>(bytes);
        }
    }

    [Serializable]
    [MemoryPackable]
    public partial class TestNetMessage : NetMessage
    {
        public string TestString { get; set; }
    }
}