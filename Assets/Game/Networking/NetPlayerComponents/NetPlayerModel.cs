using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Game.Networking.NetPlayerComponents
{
    [Serializable]
    public struct NetPlayerModel : INetworkSerializable ,IEquatable<NetPlayerModel>
    {
        public ulong ClientId;
        public FixedString32Bytes Name;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref Name);
        }

        public bool Equals(NetPlayerModel other)
        {
            return ClientId == other.ClientId && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            return obj is NetPlayerModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ClientId, Name);
        }
    }
}