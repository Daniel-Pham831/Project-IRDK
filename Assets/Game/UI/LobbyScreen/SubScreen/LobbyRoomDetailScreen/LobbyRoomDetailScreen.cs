using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Maniac.DataBaseSystem;
using Maniac.UISystem;
using Maniac.Utils;
using TMPro;
using UnityEngine.UI;

namespace Game
{
    public class LobbyRoomDetailScreen : BaseUI
    {
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private LobbyConfig _lobbyConfig;

        [SerializeField] private Button colorButtonPrefab;
        [SerializeField] private GameObject colorPickerHolder;
        [SerializeField] private TMP_Text roomName;
        [SerializeField] private TMP_Text roomCode;
        [SerializeField] private TMP_Text startOrReady;
        
        public override void OnSetup(object parameter = null)
        {
            _lobbyConfig = _dataBase.Get<LobbyConfig>();
            
            base.OnSetup(parameter);
        }

        public async void OnStartOrReadyClicked()
        {
            Debug.Log("OnStartOrReadyClicked");
        }

        public async void OnKickPlayerClicked()
        {
            
        }
    }
}
