using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using Sirenix.OdinInspector;

namespace Maniac.DataBaseSystem
{
    public class TileConfig : DataBaseConfig
    {
        public List<TileData> tileDatas;
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
        
        public List<string> PossibleSprites;
        public Direction Direction => _direction;
        
        public AdjacentTileData(Direction direction)
        {
            _direction = direction;
        }
    }
}
