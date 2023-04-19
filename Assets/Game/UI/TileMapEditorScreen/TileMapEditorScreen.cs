using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Maniac.DataBaseSystem;
using Maniac.UISystem;
using Maniac.Utils;
using UnityEngine.Tilemaps;
using TileData = Maniac.DataBaseSystem.TileData;

namespace Game
{
    public class TileMapEditorScreen : BaseUI
    {
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private TileConfig _tileConfig;
        [SerializeField] private AdjacentEditorPanel _adjacentEditorPanel;
        
        [SerializeField] private List<Sprite> _tilemap;

        [SerializeField] private Transform chooserHolder;
        [SerializeField] private TileInfoItem tileItemPrefab;

        
        [SerializeField] private Transform alreadyChosenHolder;
        private List<TileInfoItem> chosenTiles = new List<TileInfoItem>();
        
        private HashSet<string> alreadyChosenNames = new HashSet<string>();

        public override void OnSetup(object parameter = null)
        {
            _tileConfig = _dataBase.GetConfig<TileConfig>();
            foreach (var tileSprite in _tilemap)
            {
                var newTile = Instantiate(tileItemPrefab, chooserHolder);
                newTile.Setup(tileSprite,OnTileClicked);

                if (_tileConfig.Contains(newTile.name))
                {
                    newTile.SetIsSelected(true);
                    OnTileClicked(tileSprite, true);
                }
            }
        }
        
        private void OnTileClicked(Sprite spriteClicked, bool isSelected)
        {
            if(isSelected)
            {
                if (!alreadyChosenNames.Contains(spriteClicked.name))
                {
                    var newTile = Instantiate(tileItemPrefab, alreadyChosenHolder);
                    newTile.Setup(spriteClicked);
                    alreadyChosenNames.Add(spriteClicked.name);
                    chosenTiles.Add(newTile);
                }
            }
            else
            {
                var chosenTile = chosenTiles.FirstOrDefault(x => x.name == spriteClicked.name);
                if (chosenTile != null)
                {
                    chosenTiles.Remove(chosenTile);
                    alreadyChosenNames.Remove(chosenTile.name);
                    Destroy(chosenTile.gameObject);
                }
            }
        }

        public void OnSaveToConfigClicked()
        {
            _tileConfig.tileDatas.Clear();
            _tileConfig.usableSprites.Clear();
            foreach (var chosenTile in chosenTiles)
            {
                _tileConfig.tileDatas.Add(new TileData(chosenTile.MainSprite));
                _tileConfig.usableSprites.Add(chosenTile.MainSprite);
            }
        }

        public void OnOpenAdjacentEditorClicked()
        {
            _adjacentEditorPanel.ShowAndSetup();
        }
    }
}