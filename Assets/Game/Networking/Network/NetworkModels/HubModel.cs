using System;
using System.Collections.Generic;
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
        public byte[] Data = null;
        public List<ulong> ToClientIds = null;
    }

    public static class HubModelExtension
    {
        public static byte[] ToBytes(this HubModel hubModel)
        {
            return Helper.Serialize(hubModel);
        }
    }
}