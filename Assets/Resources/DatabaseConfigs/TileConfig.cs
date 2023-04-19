using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEditor;

namespace Maniac.DataBaseSystem
{
    public class TileConfig : DataBaseConfig
    {
        [ListDrawerSettings(Expanded = true)]
        public List<TileData> tileDatas;
        private Dictionary<string, TileData> _tileDatasCache = new Dictionary<string, TileData>();
        
        [ListDrawerSettings(DraggableItems = false)]
        public List<Sprite> usableSprites;

        public TileData Find(string id)
        {
            if (!_tileDatasCache.ContainsKey(id))
            {
                var tileData = tileDatas.FirstOrDefault(x=>x.Id == id);
                if (tileData != null)
                {
                    _tileDatasCache.Add(id, tileData);
                }
                else
                {
                    return null;
                }
            }
            
            return _tileDatasCache[id];
        }

        public bool Contains(string id)
        {
            return Find(id)?.Id == id;
        }

        [Button]
        public void ResetDirection()
        {
            foreach (var tileData in tileDatas)
            {
                tileData.AdjacentTileDatas[0].SetDirection(Direction.Top);
                tileData.AdjacentTileDatas[1].SetDirection(Direction.Right);
                tileData.AdjacentTileDatas[2].SetDirection(Direction.Bot);
                tileData.AdjacentTileDatas[3].SetDirection(Direction.Left);
            }
            
            //set this ScriptableObject as dirty
            EditorUtility.SetDirty(this);
        }
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
        [Sirenix.OdinInspector.ReadOnly]
        [ShowInInspector]
        public string Id;
        
        [PreviewField(40,ObjectFieldAlignment.Left)]
        public Sprite MainSprite;
        public List<AdjacentTileData> AdjacentTileDatas;

        public TileData(Sprite sprite)
        {
            Id = sprite.name;
            MainSprite = sprite;
            CreateAllDirections();
        }

        public void CreateAllDirections()
        {
            AdjacentTileDatas = new List<AdjacentTileData>();
            AdjacentTileDatas.Add(new AdjacentTileData(Direction.Top));
            AdjacentTileDatas.Add(new AdjacentTileData(Direction.Right));
            AdjacentTileDatas.Add(new AdjacentTileData(Direction.Bot));
            AdjacentTileDatas.Add(new AdjacentTileData(Direction.Left));
        }

        public void AddAdjacentTileData(Direction direction, List<string> possibleSpriteNames)
        {
            var adjacentTileData = AdjacentTileDatas.FirstOrDefault(x => x.Direction == direction);
            if (adjacentTileData != null)
            {
                adjacentTileData.PossibleSprites = possibleSpriteNames;
            }
        }
    }

    [Serializable]
    public class AdjacentTileData
    {
        [Sirenix.OdinInspector.ReadOnly]
        [SerializeField]
        private Direction _direction;
        
        public List<string> PossibleSprites;
        public Direction Direction => _direction;
        
        public AdjacentTileData(Direction direction)
        {
            _direction = direction;
        }
        
        public void SetDirection(Direction direction)
        {
            _direction = direction;
        }
    }
}
