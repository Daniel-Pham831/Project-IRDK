﻿using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.CloudProfileSystem;
using Maniac.DataBaseSystem;
using Maniac.Utils;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

namespace Game.Networking.Relay
{
    public class RelaySystem
    {
        private CloudProfileManager _cloudProfileManager => Locator<CloudProfileManager>.Instance;
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private UserProfile _userProfile;
        private LobbyConfig _lobbyConfig;
        private IRelayService _relayService;
        public List<Region> Regions { get; private set; }

        public async UniTask Init()
        {
            _userProfile = await _cloudProfileManager.Get<UserProfile>();
            _lobbyConfig = _dataBase.GetConfig<LobbyConfig>();
            _relayService = RelayService.Instance;
            try
            {
                Regions = await _relayService.ListRegionsAsync();
            }
            catch
            {
                Regions = new List<Region>();
            }

            await UniTask.CompletedTask;
        }

        public async UniTask<(Allocation, string)> CreateRelay(int numOfAllocations)
        {
            (Allocation, string) result = (null, string.Empty);
            try
            {
                result.Item1 = await _relayService.CreateAllocationAsync(numOfAllocations);
                result.Item2 = await _relayService.GetJoinCodeAsync(result.Item1.AllocationId);
            }
            catch
            {
                // ignored
            }

            return result;
        }

        public async UniTask<JoinAllocation> JoinRelay(string relayJoinCode)
        {
            try
            {
                return await _relayService.JoinAllocationAsync(relayJoinCode);
            }
            catch
            {
                // ignored
            }

            return null;
        }

        public Region GetRegion(string id)
        {
            return Regions.FirstOrDefault(x => x.Id == id);
        }
    }
}