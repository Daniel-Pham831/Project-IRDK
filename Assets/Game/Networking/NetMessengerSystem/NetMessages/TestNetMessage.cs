using System;
using MemoryPack;

namespace Game.Networking.NetMessengerSystem.NetMessages
{
    [Serializable]
    [MemoryPackable]
    public partial class TestNetMessage : NetMessage
    {
        public string TestString { get; set; }
    }
}