using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Game.Commands;
using Game.Networking.Lobby;
using Game.Networking.Lobby.Commands;
using Game.Networking.NetDataTransmitterComponents;
using Game.Networking.NetMessengerSystem;
using Game.Networking.NetMessengerSystem.NetMessages;
using Game.Networking.Network.NetworkModels;
using Game.Networking.Network.NetworkModels.Handlers.NetLobbyModel;
using Game.Networking.Network.NetworkModels.Handlers.NetPlayerModel;
using Game.Networking.Relay;
using Game.Scenes.NetGamePlay.Commands;
using Maniac.DataBaseSystem;
using Maniac.LanguageTableSystem;
using Maniac.TimeSystem;
using Maniac.UISystem;
using Maniac.UISystem.Command;
using Maniac.Utils;
using TMPro;
using UniRx;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;

namespace Game
{
    public class LobbyRoomDetailScreen : BaseUI, INetMessageListener
    {
        private RelaySystem _relaySystem => Locator<RelaySystem>.Instance;
        private LobbySystem _lobbySystem => Locator<LobbySystem>.Instance;
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private PingHandler _pingHandler => Locator<PingHandler>.Instance;
        private TimeManager _timeManager => Locator<TimeManager>.Instance;
        private NetMessageTransmitter _netMessageTransmitter => Locator<NetMessageTransmitter>.Instance;

        private NetModelHub _hub => Locator<NetModelHub>.Instance;
        private NetLobbyModelHandler _netLobbyModelHandler;
        private NetPlayerModelHandler _NetPlayerModelHandler;
        private LobbyConfig _lobbyConfig;
        private NetworkTimeSystem _networkTimeSystem;
        private GeneralConfig _generalConfig;

        [SerializeField] private TMP_Text lobbyNameTxt;
        [SerializeField] private TMP_Text lobbyStateTxt;
        [SerializeField] private TMP_Text lobbyPlayerCountTxt;
        [SerializeField] private TMP_Text lobbyRegionTxt;
        [SerializeField] private TMP_Text lobbyCodeTxt;
        [SerializeField] private TMP_Text lobbyPingTxt;
        [SerializeField] private TMP_Text gameStartCountDownTxt;

        [SerializeField] private LanguageItem privateLangItem;
        [SerializeField] private LanguageItem publicLangItem;
        [SerializeField] private LanguageItem gameStartCountDownLangItem;

        [SerializeField] private Button startGameBtn;

        private readonly string _playerCountFormat = "{0}/{1}";
        private Lobby _joinedLobby;

        protected override async void Awake()
        {
            base.Awake();

            _netMessageTransmitter.Register<LobbyStartNetMessage>(this);
        }

        private void OnDestroy()
        {
            _netMessageTransmitter.UnregisterAll(this);
        }

        public override async void OnSetup(object parameter = null)
        {
            _lobbyConfig = _dataBase.GetConfig<LobbyConfig>();
            _generalConfig = _dataBase.GetConfig<GeneralConfig>();
            _netLobbyModelHandler = _hub.GetHandler<NetLobbyModelHandler>();
            _NetPlayerModelHandler = _hub.GetHandler<NetPlayerModelHandler>();
            startGameBtn.gameObject.SetActive(NetworkManager.Singleton.IsServer);
            gameStartCountDownTxt.gameObject.SetActive(false);

            SubscribeEvents();

            base.OnSetup(parameter);
        }

        private void SubscribeEvents()
        {
            _lobbySystem.JoinedLobby.Subscribe(value =>
            {
                if (_lobbySystem.JoinedLobby.Value == null) return;

                _joinedLobby = _lobbySystem.JoinedLobby.Value;
                UpdateLobbyRoom();
            }).AddTo(this);

            _pingHandler.PingInMilliSeconds.Subscribe(value => { lobbyPingTxt.text = $"{value} ms"; }).AddTo(this);

            _netLobbyModelHandler.AllClientReactiveModels.Subscribe(UpdatePlayersReadyState).AddTo(this);
        }

        private async void UpdatePlayersReadyState(Dictionary<ulong, ReactiveProperty<NetLobbyModel>> lobbyModelDict)
        {
            foreach (var lobbyModel in lobbyModelDict)
            {
                var netPlayerModel = await _NetPlayerModelHandler.GetModelByPlayerId(lobbyModel.Value.Value.PlayerId);

                if (netPlayerModel == null) continue;
            }
        }

        private void UpdateLobbyRoom()
        {
            lobbyNameTxt.text = _joinedLobby.Name;
            lobbyPlayerCountTxt.text =
                string.Format(_playerCountFormat, _joinedLobby.Players.Count, _joinedLobby.MaxPlayers);
            lobbyCodeTxt.text = $"CODE\n{_joinedLobby.LobbyCode}";
            lobbyStateTxt.text = _joinedLobby.IsPrivate
                ? privateLangItem.GetCurrentLanguageText()
                : publicLangItem.GetCurrentLanguageText();

            var regionDescription = string.Empty;
            if (_joinedLobby.Data.TryGetValue(LobbyDataKey.LobbyRegion, out var dataObject))
            {
                regionDescription = _relaySystem.GetRegion(dataObject.Value)?.Description;
            }

            lobbyRegionTxt.text = regionDescription;
        }

        public async void OnStartClicked()
        {
            bool isHost = NetworkManager.Singleton.IsHost;
            if (!isHost) return;

            await new UpdateLobbyPlayingStateCommand().Execute();
            startGameBtn.gameObject.SetActive(false);
            _netMessageTransmitter.SendNetMessage(new LobbyStartNetMessage());
        }

        public override async void Back()
        {
            var yes = await new ShowConfirmationDialogCommand(LanguageTable.Confirmation_LeaveRoomHeader,
                LanguageTable.Confirmation_LeaveRoomBody).ExecuteAndGetResult();

            if (yes)
                await new LeaveLobbyCommand().Execute();
        }

        public async void OnSettingClicked()
        {

        }

        public async void OnAccountClicked()
        {
            await new ShowLobbyAccountDetailCommand().Execute();
        }

        public async void OnLobbyPlayersDetailsClicked()
        {
            await new ShowScreenCommand<LobbyPlayersDetailsScreen>().Execute();
        }

        public async void OnNetMessageReceived(NetMessage message)
        {
            switch (message)
            {
                case LobbyStartNetMessage:
                    await OnLobbyStartNetMessageReceived();
                    break;
            }
        }

        private async UniTask OnLobbyStartNetMessageReceived()
        {
            var countDownFormat = gameStartCountDownLangItem.GetCurrentLanguageText();
            int startTimeCountDown = _generalConfig.StartGameCounterInSeconds;
            startGameBtn.gameObject.SetActive(false);
            gameStartCountDownTxt.gameObject.SetActive(true);
            canvasGroup.interactable = false;

            while (startTimeCountDown > 0)
            {
                gameStartCountDownTxt.text = string.Format(countDownFormat, startTimeCountDown--);
                gameStartCountDownTxt.transform.DOPunchScale(Vector3.one * 0.15f, 0.2f);

                await UniTask.Delay(1000);
            }

            _uiManager.Close<LobbyRoomDetailScreen>();
            await new LoadNetGamePlaySceneCommand().Execute();
        }
    }
}

