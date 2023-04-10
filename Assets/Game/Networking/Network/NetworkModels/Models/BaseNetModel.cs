using System;
using System.Collections.Generic;
using MemoryPack;

namespace Game.Networking.Network.NetworkModels.Models
{
    [Serializable]
    [MemoryPackable()]
    public partial class BaseNetModel
    {
        public bool ShouldRemove { get; set; }
        public ulong ClientId { get; set; }
        public string PlayerId { get; set; }
    }
}