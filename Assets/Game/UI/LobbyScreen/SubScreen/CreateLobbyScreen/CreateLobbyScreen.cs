using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Game.Networking.Lobby.Models;
using Maniac.DataBaseSystem;
using Maniac.UISystem;
using Maniac.Utils;
using TMPro;
using UnityEngine.UI;

namespace Game
{
    public class CreateLobbyScreen : BaseUI
    {
        [SerializeField] private TMP_InputField lobbyNameInput;
        [SerializeField] private Toggle isPrivateToggle;
        [SerializeField] private Slider totalPlayersSlider;
        [SerializeField] private TMP_Text numOfPlayerOnSlider;
        
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private LobbyConfig _lobbyConfig;
        private LobbyModel _model;

        public override void OnSetup(object parameter = null)
        {
            _lobbyConfig = _dataBase.Get<LobbyConfig>();
            _model = new LobbyModel();
            isPrivateToggle.onValueChanged.AddListener(value =>
            {
                _model.IsPrivateLobby = value;
            });
            isPrivateToggle.isOn = false;

            totalPlayersSlider.minValue = 1;
            totalPlayersSlider.maxValue = _lobbyConfig.MaxPlayersPerLobby;
            totalPlayersSlider.wholeNumbers = true;
            totalPlayersSlider.onValueChanged.AddListener(value =>
            {
                _model.MaxPlayers = (int)value;
                numOfPlayerOnSlider.text = value.ToString();
            });
            totalPlayersSlider.value = 1;
            totalPlayersSlider.value = _lobbyConfig.MaxPlayersPerLobby;
        }

        public async void OnCreateClicked()
        {
            var trimmedName = lobbyNameInput.text.Trim();
            _model.LobbyName = trimmedName;
            Close(_model);
        }
    }
}
