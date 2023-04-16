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

        private ReactiveProperty<bool> _isSelected = new ReactiveProperty<bool>();
        private Action<Sprite,bool> _onClickCallBack;
        private Sprite _sprite;
        public Sprite MainSprite => _sprite;

        private void Awake()
        {
            _isSelected.Subscribe(value =>
            {
                border.SetActive(value);
            }).AddTo(this);

            _isSelected.Value = false;
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
            
            Debug.Log($"Clicked: {this.name} - {_isSelected.Value}");
            _isSelected.Value = !_isSelected.Value;
            _onClickCallBack?.Invoke(_sprite,_isSelected.Value);
        }

        public void SetIsSelected(bool isSelected)
        {
            _isSelected.Value = isSelected;
        }
    }
}