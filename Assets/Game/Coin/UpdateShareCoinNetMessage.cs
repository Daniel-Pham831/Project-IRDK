using Game.Networking.NetMessengerSystem.NetMessages;
using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Coin
{
    [Serializable]
    [MemoryPackable]
    public partial class UpdateShareCoinNetMessage : NetMessage
    {
        public int amout;
    }
}
