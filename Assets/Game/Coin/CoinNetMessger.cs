using Game.Networking.NetMessengerSystem;
using Game.Networking.NetMessengerSystem.NetMessages;
using MemoryPack;
using System;

[Serializable]
[MemoryPackable]
public partial class CoinNetMessger : NetMessage, INetMessageListener
{
    public int Amout;

    public void OnNetMessageReceived(NetMessage message)
    {
        throw new NotImplementedException();
    }
}