using System;
using Maniac.Utils;
using MemoryPack;
using Unity.Collections;

namespace Game.Networking.Network.NetworkModels
{
    [Serializable]
    [MemoryPackable]
    public partial class HubModel
    {
        public FixedString64Bytes HandlerKey;
        public byte[] Data;
    }

    public static class HubModelExtension
    {
        public static byte[] ToBytes(this HubModel hubModel)
        {
            return Helper.Serialize(hubModel);
        }
    }
}