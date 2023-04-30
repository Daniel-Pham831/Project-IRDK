using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.DataBaseSystem;
using Maniac.Utils;
using Unity.Multiplayer.Tools.NetStatsMonitor;
using Unity.Netcode;
using UnityEngine;

namespace Game.Networking.Scripts
{
    public class ShowRunTimeNetworkStatsCommand : Command
    {
        private DataBase _dataBase = Locator<DataBase>.Instance;
        private BuildSettingConfig _buildSettingConfig;
        private NetConfig _netConfig;

        public override async UniTask Execute()
        {
            if (_buildSettingConfig.IsInProduction) return;
            
            _buildSettingConfig = _dataBase.GetConfig<BuildSettingConfig>();
            _netConfig = _dataBase.GetConfig<NetConfig>();

            var go = new GameObject("RunTimeNetworkStats");
            var runtimeNetStatsMonitor = go.AddComponent<RuntimeNetStatsMonitor>();
            runtimeNetStatsMonitor.Configuration = _netConfig.NetStatsMonitorConfiguration;
        }
    }
}