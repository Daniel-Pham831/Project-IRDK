using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Game.Networking.NetPlayerComponents;
using Game.Networking.Network;
using Maniac;
using Unity.Netcode;

namespace Maniac.DataBaseSystem
{
    public class NetConfig : DataBaseConfig
    {
        public float SendPingInterval = 0.5f;
        public NetPlayer NetPlayer;
        
        public List<NetworkBehaviour> NetworkPrefabs = new List<NetworkBehaviour>();
    }
}
