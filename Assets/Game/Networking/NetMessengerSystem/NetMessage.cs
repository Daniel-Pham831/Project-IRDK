using System;
using Maniac.Utils;
using MemoryPack;
using Unity.Collections;

namespace Game.Networking.NetMessengerSystem
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

    [Serializable]
    [MemoryPackable]
    public partial class TestNetMessage : NetMessage
    {
        public string TestString { get; set; }
    }
    
    
    [Serializable]
    [MemoryPackable]
    public partial class TestNet3Message : NetMessage
    {
        public string TestString { get; set; }
    }
    
    
    [Serializable]
    [MemoryPackable]
    public partial class TestNet4Message : NetMessage
    {
        public string TestString { get; set; }
    }
    
    
    [Serializable]
    [MemoryPackable]
    public partial class TestNet5Message : NetMessage
    {
        public string TestString { get; set; }
    }
}