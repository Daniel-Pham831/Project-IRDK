using System;
using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Networking.LobbySystem.Commands
{
    public class UpdatePlayerDataCommand : Command
    {
        private readonly string _lobbyId;
        private readonly string _playerId;
        private readonly string _key;
        private readonly string _data = "{}";
        
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;

        public UpdatePlayerDataCommand(string lobbyId, string playerId, string key, object data)
        {
            _lobbyId = lobbyId;
            _playerId = playerId;
            _key = key;
            try
            {
                _data = JsonUtility.ToJson(data);
            }
            catch
            {
                // ignored
            }
        }

        public override async UniTask Execute()
        {
            await _lobbySystem.UpdatePlayerData(_lobbyId, _playerId, _key, _data);
        }
    }
}