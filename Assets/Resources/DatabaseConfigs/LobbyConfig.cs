using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using Range = Maniac.Utils.Range;

namespace Maniac.DataBaseSystem
{
    public class LobbyConfig : DataBaseConfig
    {
        public float HeartBeatIntervalInSeconds = 20f;
        public float LobbyUpdateIntervalInSeconds = 1.1f;
        public int MaxPlayersPerLobby = 4;
    }
}
