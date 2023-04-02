using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;

namespace Maniac.DataBaseSystem
{
    public class LobbyConfig : DataBaseConfig
    {
        public float HeartBeatIntervalInSeconds = 20f;
        public float LobbyUpdateIntervalInSeconds = 1.1f;
    }
}
