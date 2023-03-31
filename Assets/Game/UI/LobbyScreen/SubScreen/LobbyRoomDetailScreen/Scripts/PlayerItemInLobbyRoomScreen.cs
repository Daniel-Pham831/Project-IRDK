using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Game.Scripts
{
    public class PlayerItemInLobbyRoomScreen : MonoBehaviour
    {
        [SerializeField] private GameObject isHostIcon;
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private TMP_Text ping;
        [SerializeField] private GameObject isReadyIcon;
        [SerializeField] private GameObject kickButton;

        public async UniTask Setup(Player playerInfo)
        {
            
        }
    }
}