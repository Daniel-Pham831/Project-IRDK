using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Game.Networking.NetDataTransmitterComponents;
using Game.Networking.Network;
using Game.Players.Scripts;
using Maniac;
using Unity.Multiplayer.Tools.NetStatsMonitor;
using Unity.Netcode;

namespace Maniac.DataBaseSystem
{
    public class NetConfig : DataBaseConfig
    {
        public float SendPingInterval = 0.5f;
        public float NetworkTimeOut = 15f;
        
        public NetDataTransmitter NetDataTransmitter;
        public List<NetworkBehaviour> NetworkPrefabs = new List<NetworkBehaviour>();
        
        public NetStatsMonitorConfiguration NetStatsMonitorConfiguration;
        
        private Dictionary<string,NetworkBehaviour> _networkPrefabCache = new Dictionary<string,NetworkBehaviour>();

        public NetworkBehaviour GetNetPrefab(string typeFullName)
        {
            var key = typeFullName;
            if (!_networkPrefabCache.ContainsKey(key))
            {
                var netPrefab = GetNetPrefabHelper(key);
                if (netPrefab != null)
                    _networkPrefabCache[key] = netPrefab;
                else
                    throw new Exception($"There is no {key} in NetworkPrefabs - NetConfig. Please Check!");
            }

            return _networkPrefabCache[key];
        }
        
        public T GetNetPrefab<T>() where T : NetworkBehaviour
        {
            var key = typeof(T).FullName;
            if (!_networkPrefabCache.ContainsKey(key))
            {
                var netPrefab = GetNetPrefabHelper<T>(key);
                if (netPrefab != null)
                    _networkPrefabCache[key] = netPrefab;
                else
                    throw new Exception($"There is no {key} in NetworkPrefabs - NetConfig. Please Check!");
            }

            return _networkPrefabCache[key] as T;
        }

        private T GetNetPrefabHelper<T>(string fullName) where T : NetworkBehaviour
        {
            var type = Type.GetType(fullName);
            foreach (var netPrefab in NetworkPrefabs)
            {
                if (netPrefab.TryGetComponent(type,out var component))
                {
                    return component as T;
                }
            }
            
            return default(T);
        }
        
        private NetworkBehaviour GetNetPrefabHelper(string fullName)
        {
            var type = Type.GetType(fullName);
            foreach (var netPrefab in NetworkPrefabs)
            {
                if (netPrefab.TryGetComponent(type,out var component))
                {
                    return component as NetworkBehaviour;
                }
            }
            
            return null;
        }
    }
}
