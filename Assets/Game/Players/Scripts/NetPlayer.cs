using System;
using Cysharp.Threading.Tasks;
using Game.Networking.Network.NetworkModels;
using Game.Networking.Network.NetworkModels.Handlers;
using Game.Networking.Network.NetworkModels.Handlers.NetPlayerModel;
using Game.Networking.NormalMessages;
using Maniac.DataBaseSystem;
using Maniac.MessengerSystem.Base;
using Maniac.MessengerSystem.Messages;
using Maniac.Utils;
using Maniac.Utils.Extension;
using TMPro;
using ToolBox.Tags;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.U2D.Animation;

namespace Game.Players.Scripts
{
    public class NetPlayer : NetworkBehaviour
    {
        private NetModelHub _hub => Locator<NetModelHub>.Instance;
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private CharacterConfig _characterConfig;

        [SerializeField] private Tag LocalPlayerTag;
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private SpriteRenderer playerSpriteRenderer;
        [SerializeField] private SpriteLibrary spriteLibrary;

        private string oldName = "";
        private string oldCharacterId = "";
        private NetPlayerModelHandler _netPlayerModelHandler;

        private void Awake()
        {
            _characterConfig = _dataBase.GetConfig<CharacterConfig>();
            _netPlayerModelHandler = _hub.GetHandler<NetPlayerModelHandler>();
        }

        public override async void OnNetworkSpawn()
        {
            var thisClientNetPlayerModel = await GetReactiveModel();
            thisClientNetPlayerModel.Subscribe(UpdateNetPlayer).AddTo(this);
            
            DontDestroyOnLoad(this.gameObject);
            NetworkObject.DontDestroyWithOwner = false;

            if (IsOwner)
            {
                this.gameObject.AddTag(LocalPlayerTag);
                Locator<NetPlayer>.Set(this,true);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner)
            {
                Locator<NetPlayer>.Remove(this);
            }
        }

        private async UniTask<ReactiveProperty<NetPlayerModel>> GetReactiveModel()
        {
            ReactiveProperty<NetPlayerModel> result = _netPlayerModelHandler.GetReactiveModelByClientId(OwnerClientId);
            
            while (result == null)
            {
                await UniTask.Delay(100);
                result = _netPlayerModelHandler.GetReactiveModelByClientId(OwnerClientId);
            }

            return result;
        }

        private void UpdateNetPlayer(NetPlayerModel value)
        {
            if (value.Name != null && oldName != value.Name)
            {
                oldName = value.Name;
                playerName.text = oldName.AddColor(OwnerClientId == NetworkManager.Singleton.LocalClientId ? Color.yellow : Color.white);
            }

            if (value.CharacterGraphicsId != null && oldCharacterId != value.CharacterGraphicsId)
            {
                oldCharacterId = value.CharacterGraphicsId;
                var characterInfo = _characterConfig.GetCharacterInfo(oldCharacterId);
                playerSpriteRenderer.sprite = characterInfo.sprite;
                spriteLibrary.spriteLibraryAsset = characterInfo.spriteLibraryAsset;
            }
        }
    }
}