using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using CharacterInfo = Maniac.DataBaseSystem.CharacterInfo;

namespace Game
{
    public class CharacterChooserItemInLobbyAccount : MonoBehaviour
    {
        [SerializeField] private Image mainImage;
        private Action<string> _onClickedItemCallback;
        private CharacterInfo _characterInfo;

        public async UniTask Setup(CharacterInfo characterInfo,Action<string> OnClickedItemCallback)
        {
            _characterInfo = characterInfo;
            _onClickedItemCallback = OnClickedItemCallback;
            mainImage.sprite = characterInfo.sprite;
        }

        public async void OnItemClicked()
        {
            _onClickedItemCallback?.Invoke(_characterInfo.Id);
        }
    }
}