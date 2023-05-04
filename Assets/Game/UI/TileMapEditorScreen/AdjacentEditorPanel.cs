using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Enums;
using Maniac.DataBaseSystem;
using Maniac.Utils;
using Maniac.Utils.Extension;
using TMPro;
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

        [SerializeField] private Image topIndicator;
        [SerializeField] private Image rightIndicator;
        [SerializeField] private Image botIndicator;
        [SerializeField] private Image leftIndicator;

        [SerializeField] private Image chosenAdjacentItemPrefab;
        [SerializeField] private TMP_Dropdown directionDropdown;

        private void Awake()
        {
            _tileConfig = _dataBase.GetConfig<TileConfig>();
            directionDropdown.options = Enum.GetNames(typeof(Direction)).Select(x=>new TMP_Dropdown.OptionData(x)).ToList();
            
            directionDropdown.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(int directionAsInt)
        {
            var currentDirection = (Direction)directionAsInt;
            var correctHolder = GetCorrectHolderForDirection(currentDirection);
            var allChildNames = GetAllChildNamesOfHolder(correctHolder);
            var correctIndicator = GetCorrectIndicatorForDirection(currentDirection);
            TurnOffAllIndicators();
            correctIndicator.gameObject.SetActive(true);
            foreach (var tiles in _tilesForAdjacentSelection)
            {
                var shouldSelected = allChildNames.Contains(tiles.name);
                tiles.SetIsSelected(shouldSelected);
            }
        }

        private void TurnOffAllIndicators()
        {
            topIndicator.gameObject.SetActive(false);
            rightIndicator.gameObject.SetActive(false);
            botIndicator.gameObject.SetActive(false);
            leftIndicator.gameObject.SetActive(false);
        }

        private Image GetCorrectIndicatorForDirection(Direction currentDirection)
        {
            switch (currentDirection)
            {
                case Direction.Top:
                    return topIndicator;
                case Direction.Right:
                    return rightIndicator;
                case Direction.Bottom:
                    return botIndicator;
                case Direction.Left:
                    return leftIndicator;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currentDirection), currentDirection, null);
            }
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
            var currentDirection = (Direction)directionDropdown.value;
            var correctHolder = GetCorrectHolderForDirection(currentDirection);
            if (isSelected)
            {
                var newImage = Instantiate(chosenAdjacentItemPrefab, correctHolder);
                newImage.gameObject.name = sprite.name;
                newImage.sprite = sprite;
            }
            else
            {
                var childWithName = GetChildWithName(correctHolder,sprite.name);
                if(childWithName != null)
                    Destroy(childWithName.gameObject);
            }
        }

        // get a child transform in correctHolder with name spriteName
        private Transform GetChildWithName(Transform parent, string name)
        {
            if (parent.childCount == 0) return null;

            foreach (Transform child in parent)
            {
                if (child.name == name)
                    return child;
            }

            return null;
        }

        private Transform GetCorrectHolderForDirection(Direction currentDirection)
        {
            switch (currentDirection)
            {
                case Direction.Top:
                    return _topAdjacentHolder;
                case Direction.Right:
                    return _rightAdjacentHolder;
                case Direction.Bottom:
                    return _botAdjacentHolder;
                case Direction.Left:
                    return _leftAdjacentHolder;
                default:
                    throw new ArgumentOutOfRangeException(nameof(currentDirection), currentDirection, null);
            }
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
            Transform holder = GetCorrectHolderForDirection(adjacentTileData.Direction);
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
                tileToSave.AddAdjacentTileData(Direction.Bottom,botPossibleNames);
                tileToSave.AddAdjacentTileData(Direction.Left,leftPossibleNames);
            }
        }

        private List<string> GetAllChildNamesOfHolder(Transform topAdjacentHolder)
        {
            var result = new List<string>();
            if (topAdjacentHolder.childCount == 0) return result;

            foreach (Transform child in topAdjacentHolder)
            {
                result.Add(child.gameObject.name);
            }
            return result;
        }
    }
}