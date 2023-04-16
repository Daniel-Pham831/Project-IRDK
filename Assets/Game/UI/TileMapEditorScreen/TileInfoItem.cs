using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class TileInfoItem : MonoBehaviour
    {
        [SerializeField] private Image mainImage;
        [SerializeField] private GameObject border;

        private ReactiveProperty<bool> isSelected = new ReactiveProperty<bool>();
        private Action<Sprite,bool> _onClickCallBack;
        private Sprite _sprite;
        public Sprite MainSprite => _sprite;

        private void Awake()
        {
            isSelected.Subscribe(value =>
            {
                border.SetActive(value);
            }).AddTo(this);

            isSelected.Value = false;
        }

        public void Setup(Sprite sprite, Action<Sprite,bool> onClickCallBack = null)
        {
            this.name = sprite.name;
            mainImage.sprite = sprite;
            _sprite = sprite;
            _onClickCallBack = onClickCallBack;
        }

        public void OnClicked()
        {
            if (_onClickCallBack == null) return;
            
            Debug.Log($"Clicked: {this.name} - {isSelected.Value}");
            isSelected.Value = !isSelected.Value;
            _onClickCallBack?.Invoke(_sprite,isSelected.Value);
        }
    }
}