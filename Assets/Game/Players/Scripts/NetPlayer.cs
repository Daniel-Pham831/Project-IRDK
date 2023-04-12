using System;
using Game.Networking.Network.NetworkModels;
using Game.Networking.Network.NetworkModels.Handlers;
using Maniac.DataBaseSystem;
using Maniac.Utils;
using TMPro;
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace Game.Players.Scripts
{
    public class NetPlayer : NetworkBehaviour
    {
        private NetModelHub _hub => Locator<NetModelHub>.Instance;
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private CharacterConfig _characterConfig;

        [SerializeField] private TMP_Text playerName;
        [SerializeField] private SpriteRenderer playerSpriteRenderer;

        private string oldName = "";
        private string oldCharacterId = "";
        private NetPlayerModelHandler _netPlayerModelHandler;

        private void Awake()
        {
            _characterConfig = _dataBase.GetConfig<CharacterConfig>();
            _netPlayerModelHandler = _hub.GetHandler<NetPlayerModelHandler>();
        }

        public override void OnNetworkSpawn()
        {
            var thisClientNetPlayerModel = _netPlayerModelHandler.GetReactiveModelByClientId(OwnerClientId);

            thisClientNetPlayerModel.Subscribe(UpdateNetPlayer).AddTo(this);
        }

        private void UpdateNetPlayer(NetPlayerModel value)
        {
            if (value.Name != null && oldName != value.Name)
            {
                oldName = value.Name;
                playerName.text = oldName;
            }

            if (value.CharacterGraphicsId != null && oldCharacterId != value.CharacterGraphicsId)
            {
                oldCharacterId = value.CharacterGraphicsId;
                var characterInfo = _characterConfig.GetCharacterInfo(oldCharacterId);
                playerSpriteRenderer.sprite = characterInfo.sprite;
            }
        }
    }
}