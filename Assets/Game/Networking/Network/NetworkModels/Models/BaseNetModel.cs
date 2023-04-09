using System;
using BinaryPack.Attributes;
using BinaryPack.Enums;

namespace Game.Networking.Network.NetworkModels.Models
{
    [Serializable]
    [BinarySerialization]
    public class BaseNetModel
    {
        public bool ShouldRemove { get; set; }
        public ulong ClientId { get; set; }
        public string PlayerId { get; set; }
    }
}