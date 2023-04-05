using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Networking.Lobby;
using Maniac.Utils;
using UniRx;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Scripts
{
    public class LobbyItemControllerInLobbyScreen  : MonoBehaviour
    {
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;

        [SerializeField] private Transform holder;
        [SerializeField] private LobbyItemInLobbyScreen itemPrefab;

        private readonly List<LobbyItemInLobbyScreen> _availableLobbyItems = new List<LobbyItemInLobbyScreen>();
        private Action<Lobby> _joinLobbyCallBack;

        public async UniTask Init(Action<Lobby> joinLobbyCallBack)
        {
            _joinLobbyCallBack = joinLobbyCallBack;
            SubscribeEvents();
            await _lobbySystem.StartQuery();
        }

        private async void OnDestroy()
        {
            await _lobbySystem.StopQuery();
        }

        private void SubscribeEvents()
        {
            _lobbySystem.QueryResponse.Subscribe(async (value) =>
            {
                await Setup(value?.Results);
            }).AddTo(this);
        }

        private async UniTask Setup(List<Lobby> lobbies)
        {
            lobbies ??= new List<Lobby>();
            
            if (_availableLobbyItems.Count < lobbies.Count)
            {
                var diff = lobbies.Count - _availableLobbyItems.Count;
                for (int i = 0; i < diff; i++)
                {
                    _availableLobbyItems.Add(Instantiate(itemPrefab,holder));
                }
            }

            for (int i = 0; i < _availableLobbyItems.Count; i++)
            {
                var lobbyItem = _availableLobbyItems[i];

                var shouldSetup = i <= lobbies.Count - 1;
                if (shouldSetup)
                {
                    lobbyItem.Setup(lobbies[i],_joinLobbyCallBack);
                }
                
                lobbyItem.gameObject.SetActive(shouldSetup);
            }
        }
    }
}