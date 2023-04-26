using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Game.Networking.Network.NetworkModels;
using Game.Networking.Network.NetworkModels.Handlers;
using Game.Networking.Network.NetworkModels.Handlers.NetPlayerModel;
using Maniac.DataBaseSystem;
using Maniac.UISystem;
using Maniac.Utils;
using UniRx;
using UnityEngine.UI;

namespace Game
{
    public class LobbyAccountDetailScreen : BaseUI
    {
        private NetModelHub _netModelHub => Locator<NetModelHub>.Instance;
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private CharacterConfig _characterConfig;

        [SerializeField] private Image mainCharacterImage;
        [SerializeField] private CharacterChooserItemInLobbyAccount chooserItemPrefab;
        [SerializeField] private Transform content;
        private readonly StringReactiveProperty chosenCharacterId = new StringReactiveProperty();
        private string _localClientCharacter;
        private NetPlayerModelHandler _netPlayerModelHandler;

        public override async void OnSetup(object parameter = null)
        {
            _characterConfig = _dataBase.GetConfig<CharacterConfig>();
            _netPlayerModelHandler = _netModelHub.GetHandler<NetPlayerModelHandler>();
            chosenCharacterId.Subscribe(UpdateMainCharacterImage).AddTo(this);
            _localClientCharacter = _netPlayerModelHandler.LocalClientModel.Value.CharacterGraphicsId;
            chosenCharacterId.Value = _localClientCharacter;
            await SetupAllCharacterChoosers();
            base.OnSetup(parameter);
        }

        private async UniTask SetupAllCharacterChoosers()
        {
            foreach (var characterInfo in _characterConfig.CharacterInfos)
            {
                var newItem = Instantiate(chooserItemPrefab, content);
                newItem.Setup(characterInfo, UpdateMainCharacterImage);
            }
        }

        private void UpdateMainCharacterImage(string characterId)
        {
            var characterInfo = _characterConfig.GetCharacterInfo(characterId);
            mainCharacterImage.sprite = characterInfo.sprite;
            _localClientCharacter = characterId;
        }

        public async void OnChooseClicked()
        {
            Close(_localClientCharacter);
        }

        public override async void Back()
        {
            await Close(_localClientCharacter);
        }
    }
}
