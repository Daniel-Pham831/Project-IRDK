using System;
using System.Collections.Generic;
using System.Linq;
using Maniac.UISystem;
using Maniac.Utils;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Game.Scenes.WCFTest
{
    [CreateAssetMenu(fileName = "Tile", menuName = "Tile/TileConfig", order = 0)]
    public class TileConfig : ScriptableObject
    {
        public List<TileData> tileDatas;

        [Button]
        public void AddNewTileData(Sprite sprite)
        {
            var newTileData = new TileData(sprite);
            newTileData.CreateAllDirections();
            if(tileDatas.FirstOrDefault(x => x.Id == newTileData.Id) == null)
                tileDatas.Add(newTileData);
        }
        
        [Button]
        public void AddNewTilesData(List<Sprite> sprites)
        {
            foreach (var sprite in sprites)
            {
                AddNewTileData(sprite);
            }
        }
        
#if UNITY_EDITOR
        public static TileConfig ActiveTileConfig
        {
            get
            {
                if (Locator<TileConfig>.Instance == null)
                {
                    TileConfig tileConfig = AssetDatabase.FindAssets($"t: {typeof(TileConfig).Name}").ToList()
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .Select(AssetDatabase.LoadAssetAtPath<TileConfig>)
                        .FirstOrDefault();

                    Locator<TileConfig>.Set(tileConfig);
                }

                return Locator<TileConfig>.Instance;
            }
        }
#endif
    }

    [Serializable]
    public enum Direction
    {
        Top,
        Right,
        Bot,
        Left,
    }
    
    [Serializable]
    public class TileData
    {
        public string Id;
        public Sprite MainSprite;
        public List<AdjacentTileData> AdjacentTileDatas;

        public TileData(Sprite sprite)
        {
            Id = sprite.name;
            MainSprite = sprite;
        }

        public void CreateAllDirections()
        {
            AdjacentTileDatas = new List<AdjacentTileData>();
            AdjacentTileDatas.Add(new AdjacentTileData(Direction.Top));
            AdjacentTileDatas.Add(new AdjacentTileData(Direction.Right));
            AdjacentTileDatas.Add(new AdjacentTileData(Direction.Bot));
            AdjacentTileDatas.Add(new AdjacentTileData(Direction.Left));
        }
    }

    [Serializable]
    public class AdjacentTileData
    {
        [Sirenix.OdinInspector.ReadOnly]
        [ShowInInspector]
        private Direction _direction;
        
        [ValueDropdown("FetchAllTileData")]
        public List<string> PossibleSprites;
        public Direction Direction => _direction;
        
        public AdjacentTileData(Direction direction)
        {
            _direction = direction;
        }
        
        [Button]
        public void AddNewTilesData(List<Sprite> sprites)
        {
            foreach (var sprite in sprites)
            {
                if(!PossibleSprites.Contains(sprite.name))
                    PossibleSprites.Add(sprite.name);
            }
        }
        
#if UNITY_EDITOR
        public IEnumerable<string> FetchAllTileData()
        {
            return TileConfig.ActiveTileConfig.tileDatas.Select(x => x.Id);
        }
#endif
    }
}