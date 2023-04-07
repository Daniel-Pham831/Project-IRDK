using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Game.Networking.Network;
using Game.Networking.Scripts;
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
