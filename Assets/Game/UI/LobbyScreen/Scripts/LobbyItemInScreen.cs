using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Game.Scripts
{
    public class LobbyItemInScreen : MonoBehaviour
    {
        [SerializeField] private TMP_Text name;
        [SerializeField] private TMP_Text ping;
        [SerializeField] private TMP_Text availableSlots;

        private readonly string _pingFormat = "{0}ms";
        private readonly string _availableSlotsFormat = "{0}/{1}";
        private Lobby _lobby;

        public async UniTask Setup(Lobby lobby)
        {
            _lobby = lobby;
            UpdateLobbyInformation();
        }

        private void UpdateLobbyInformation()
        {
            availableSlots.text = string.Format(_availableSlotsFormat, _lobby.AvailableSlots, _lobby.MaxPlayers);
        }
    }
}