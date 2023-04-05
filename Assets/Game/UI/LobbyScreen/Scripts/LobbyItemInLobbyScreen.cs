﻿using System;
using Cysharp.Threading.Tasks;
using Game.Networking.Lobby;
using Maniac.LanguageTableSystem;
using Maniac.Utils.Extension;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Game.Scripts
{
    public class LobbyItemInLobbyScreen : MonoBehaviour
    {
        [SerializeField] private TMP_Text roomName;
        [SerializeField] private TMP_Text roomData;
        [SerializeField] private LanguageItem isPlayingLangItem;
        [SerializeField] private LanguageItem lobbyRoomLangItem;
        
        private readonly string _roomStateFormat = "{0}/{1} {2} {3}";
        private Action<Lobby> _joinLobbyCallBack;
        private Lobby _lobby;

        public async UniTask Setup(Lobby lobby, Action<Lobby> joinLobbyCallBack)
        {
            _joinLobbyCallBack = joinLobbyCallBack;
            _lobby = lobby;
            
            roomName.text = lobby.Name;
            var roomState = lobby.Data[LobbyDataKey.IsPlaying]?.Value == "true"
                ? isPlayingLangItem.GetCurrentLanguageText().AddColor(Color.red)
                : lobbyRoomLangItem.GetCurrentLanguageText().AddColor(Color.cyan);

            var regionText = lobby.Data[LobbyDataKey.LobbyRegion]?.Value.AddColor(Color.yellow);
            roomData.text = string.Format(_roomStateFormat, lobby.MaxPlayers - lobby.AvailableSlots, lobby.MaxPlayers,
                roomState,regionText);
        }

        public async void OnJoinClicked()
        {
            if (_lobby != null)
            {
                _joinLobbyCallBack?.Invoke(_lobby);
            }
        }
    }
}