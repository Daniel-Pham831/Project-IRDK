using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Maniac.DataBaseSystem
{
    public class Environment
    {
        public const string Production = "production";
        public const string Staging = "staging";
        public const string Develop = "develop";
    }
    
    public class BuildSettingConfig : DataBaseConfig
    {
        [SerializeField]
        [ValueDropdown("FetchAllIds")]
        private string TargetEnvironmentName;
        
        [SerializeField]
        private List<BuildEnvironment> BuildEnvironments;

        public string GetTargetEnvironmentID()
        {
            return BuildEnvironments.FirstOrDefault(x => x.EnvironmentName == TargetEnvironmentName)
                ?.EnvironmentId;
        }

        [JsonIgnore]
        public string GetTargetEnvironmentName => TargetEnvironmentName;
        
        [JsonIgnore]
        public bool IsInProduction => TargetEnvironmentName == Environment.Production;

#if UNITY_EDITOR
        public List<string> FetchAllIds()
        {
            return BuildEnvironments.Select(x => x.EnvironmentName).ToList();
        }
#endif
    }

    [Serializable]
    public class BuildEnvironment
    {
        public string EnvironmentName;
        public string EnvironmentId;
    }
}
