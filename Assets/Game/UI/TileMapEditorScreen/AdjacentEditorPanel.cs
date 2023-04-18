using System;
using System.Collections.Generic;
using Maniac.DataBaseSystem;
using Maniac.Utils;
using Maniac.Utils.Extension;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class AdjacentEditorPanel : MonoBehaviour
    {
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private TileConfig _tileConfig;

        [SerializeField] private CanvasGroup _canvasGroup;
        
        [SerializeField] private Transform _tileHolder;
        [SerializeField] private TileInfoItem _tileInfoItemPrefab;
        private List<TileInfoItem> _tilesInConfig = new List<TileInfoItem>();

        [SerializeField] private Transform _adjacentPickerHolder;
        private List<TileInfoItem> _tilesForAdjacentSelection = new List<TileInfoItem>();

        [SerializeField] private Image mainImage;
        [SerializeField] private Transform _topAdjacentHolder;
        [SerializeField] private Transform _rightAdjacentHolder;
        [SerializeField] private Transform _botAdjacentHolder;
        [SerializeField] private Transform _leftAdjacentHolder;
        [SerializeField] private Image chosenAdjacentItemPrefab;

        private void Awake()
        {
            _tileConfig = _dataBase.GetConfig<TileConfig>();
        }

        public void ShowAndSetup()
        {
            Setup();
            Show();
        }

        public void Show()
        {
            SetCanvasGroup(true);
        }

        public void Hide()
        {
            SetCanvasGroup(false);
        }

        private void SetCanvasGroup(bool shouldShow)
        {
            _canvasGroup.alpha = shouldShow ? 1 : 0;
            _canvasGroup.interactable = shouldShow;
            _canvasGroup.blocksRaycasts = shouldShow;
        }
        
        private void Setup()
        {
            _tileHolder.ClearAllChildren();
            _adjacentPickerHolder.ClearAllChildren();
            _tilesInConfig.Clear();
            _tilesForAdjacentSelection.Clear();
            
            foreach (var tileData in _tileConfig.tileDatas)
            {
                var newTile = Instantiate(_tileInfoItemPrefab, _tileHolder);
                newTile.Setup(tileData.MainSprite,OnTileInConfigChosen);
                _tilesInConfig.Add(newTile);
                
                var newTileForAdjacent = Instantiate(_tileInfoItemPrefab, _adjacentPickerHolder);
                newTileForAdjacent.Setup(tileData.MainSprite,OnTileForAdjacentChosen);
                _tilesForAdjacentSelection.Add(newTileForAdjacent);
            }
        }

        private void OnTileForAdjacentChosen(Sprite sprite, bool isSelected)
        {
            if (isSelected)
            {
                
                // _tilesForAdjacentSelection.Add(sprite);
            }
            else
                // _tilesForAdjacentSelection.Remove(sprite);
        }

        private void OnTileInConfigChosen(Sprite sprite, bool isSelected)
        {
            foreach (var item in _tilesInConfig)
            {
                item.SetIsSelected(item.name == sprite.name);
            }

            if(isSelected)
                SetupTileAdjacent(sprite);
        }

        private void SetupTileAdjacent(Sprite sprite)
        {
            var tileData = _tileConfig.Find(sprite.name);
            if (tileData != null)
            {
                mainImage.sprite = tileData.MainSprite;
                mainImage.name = tileData.Id;
                foreach (var adjacentTileData in tileData.AdjacentTileDatas)
                {
                    SetupAdjacentTile(adjacentTileData);
                }
            }
        }

        private void SetupAdjacentTile(AdjacentTileData adjacentTileData)
        {
            Transform holder;
            switch(adjacentTileData.Direction)
            {
                case Direction.Top:
                    holder = _topAdjacentHolder;
                    break;
                case Direction.Right:
                    holder = _rightAdjacentHolder;
                    break;
                case Direction.Bot:
                    holder = _botAdjacentHolder;
                    break;
                case Direction.Left:
                    holder = _leftAdjacentHolder;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            holder.ClearAllChildren();

            foreach (var possibleSpriteName in adjacentTileData.PossibleSprites)
            {
                var newImage = Instantiate(chosenAdjacentItemPrefab, holder);
                newImage.gameObject.name = possibleSpriteName;
                newImage.sprite = _tileConfig.Find(possibleSpriteName).MainSprite;
            }
        }

        public void OnSaveAdjacentToTilesClicked()
        {
            var tileToSave = _tileConfig.Find(mainImage.name);
            if (tileToSave != null)
            {
                var topPossibleNames = GetAllChildNamesOfHolder(_topAdjacentHolder);
                var rightPossibleNames = GetAllChildNamesOfHolder(_rightAdjacentHolder);
                var botPossibleNames = GetAllChildNamesOfHolder(_botAdjacentHolder);
                var leftPossibleNames = GetAllChildNamesOfHolder(_leftAdjacentHolder);
                
                tileToSave.AddAdjacentTileData(Direction.Top,topPossibleNames);
                tileToSave.AddAdjacentTileData(Direction.Right,rightPossibleNames);
                tileToSave.AddAdjacentTileData(Direction.Bot,botPossibleNames);
                tileToSave.AddAdjacentTileData(Direction.Left,leftPossibleNames);
            }
        }

        private List<string> GetAllChildNamesOfHolder(Transform topAdjacentHolder)
        {
            var result = new List<string>();
            if (transform.childCount == 0) return result;

            foreach (Transform child in transform)
            {
                result.Add(child.gameObject.name);
            }
            return result;
        }

        public void OnAdjacentTypeClicked(Direction direction)
        {
            foreach (var tileSprite in _tilesForAdjacentSelection)
            {
            }
        }
    }
}