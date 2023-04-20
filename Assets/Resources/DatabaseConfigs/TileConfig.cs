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
using Random = UnityEngine.Random;

namespace Maniac.DataBaseSystem
{
    public class TileConfig : DataBaseConfig
    {
        [ListDrawerSettings(Expanded = true)]
        public List<TileData> tileDatas;
        private Dictionary<string, TileData> _tileDatasCache = new Dictionary<string, TileData>();
        private Dictionary<(string,Direction),List<string>> _possibleSpritesOfATilesAtDirection = new Dictionary<(string, Direction), List<string>>();

        [ListDrawerSettings(DraggableItems = false)]
        public List<Sprite> usableSprites;

        private List<string> usableSpriteNames = new List<string>();
        
        public string GetRandomSpriteName()
        {
            return tileDatas[Random.Range(0, usableSpriteNames.Count)].Id;
        }

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

        // we should cache it
        public AdjacentTileData GetAdjacentTileDataOfDirection(string tileName, Direction direction)
        {
            var tileData = Find(tileName);
            if (tileData == null)
            {
                return null;
            }

            return tileData.AdjacentTileDatas.FirstOrDefault(x => x.Direction == direction);
        }
        
        // Get the list of possibleSprite of a AdjacentTileData at the direction
        // We should cache it so that we don't need to search it every time
        public List<string> GetPossibleSpritesOfATilesAtDirection(string tileName, Direction direction)
        {
            var key = (tileName, direction);
            if (!_possibleSpritesOfATilesAtDirection.ContainsKey(key))
            {
                var adjacentTileData = GetAdjacentTileDataOfDirection(tileName, direction);
                if (adjacentTileData == null)
                {
                    return null;
                }

                _possibleSpritesOfATilesAtDirection.Add(key, adjacentTileData.PossibleSprites);
            }

            return _possibleSpritesOfATilesAtDirection[key];
        }
       


        public List<string> GetAllSpriteNames()
        {
            if (usableSpriteNames == null || usableSpriteNames.Count != tileDatas.Count)
            {
                usableSpriteNames ??= new List<string>();
                usableSpriteNames.Clear();
                foreach (var tileData in tileDatas)
                {
                    usableSpriteNames.Add(tileData.Id);
                }
            }

            return usableSpriteNames;
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

        [Button]
        public void FilterUnusableSpriteFromTileData()
        {
            usableSpriteNames.Clear();
            foreach (var tileData in tileDatas)
            {
                usableSpriteNames.Add(tileData.Id);
            }

            foreach (var tileData in tileDatas)
            {
                foreach (var adjacentTileData in tileData.AdjacentTileDatas)
                {
                    // remove all the sprite in adjacentTileData.PossibleSprites that is not in usableSpriteNames
                    adjacentTileData.PossibleSprites = adjacentTileData.PossibleSprites.Where(x =>
                    {
                        var contains = usableSpriteNames.Contains(x);
                        if (!contains)
                        {
                            Debug.Log($"Remove {x} from {tileData.Id} {adjacentTileData.Direction}");
                        }
                        return contains;
                    }).ToList();
                }
            }
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
