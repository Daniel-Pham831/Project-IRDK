using System;
using Game.Networking.NetMessengerSystem.NetMessages;
using MemoryPack;

namespace Game.Coin
{
    [Serializable]
    [MemoryPackable]
    public partial class UpdatePrivateCoinToClientNetMessage : NetMessage
    {
       public int privateCoin;
    }
}
