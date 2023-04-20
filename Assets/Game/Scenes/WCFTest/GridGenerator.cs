using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Maniac.DataBaseSystem;
using Maniac.Utils;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scenes.WCFTest
{
    public class GridGenerator : MonoBehaviour
    {
        public int Columns = 1;
        public int Rows = 1;
        public float CellSize = 1f;

        private Vector2 Offset;
        private Vector2 _mousePosition;
        public GameObject tilesPrefab;
        public TileConfig tileConfig;

        private Grid _grid;
        private PriorityQueue<TileModel,int> _tileQueue = new PriorityQueue<TileModel,int>();
        private List<GameObject> _allTileObjects = new List<GameObject>();

        private void OnValidate()
        {
            Rows = Mathf.Clamp(Rows, 0, Rows);
            Columns = Mathf.Clamp(Columns, 0, Columns);
            CellSize = Mathf.Clamp(CellSize, 0f, CellSize);

            Offset.x = (float)((Columns / 2f - 0.5) * -1f);
            Offset.y = (float)((Rows / 2f - 0.5) * -1f);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            for (int column = 0; column < Columns; column++)
            {
                for (int row = 0; row < Rows; row++)
                {
                    Gizmos.DrawWireCube(new Vector3(column + Offset.x, row + Offset.y),
                        new Vector3(CellSize, CellSize, 0));
                }
            }
        }

        private async UniTask Start()
        {
            Locator<TileConfig>.Set(tileConfig, true);
            await StartAgain();
        }

        private async UniTask StartAgain()
        {
            _grid = new Grid();
            GenerateAllTiles();
            var shouldRestart = await DoWaveFunctionCollapseRecursively();
            if(shouldRestart)
                await StartAgain();
        }

        private async UniTask<bool> DoWaveFunctionCollapseRecursively()
        {
            if (!_tileQueue.TryDequeue(out var tileModel, out var priority))
            {
                Debug.Log("Done!");
                return false;
            }

            var collapsedSpriteName = tileModel.Collapse();
            if (collapsedSpriteName == string.Empty)
            {
                return true;
            }

            var topPosition = new Vector2(tileModel.Position.x, tileModel.Position.y + 1);
            var bottomPosition = new Vector2(tileModel.Position.x, tileModel.Position.y - 1);
            var leftPosition = new Vector2(tileModel.Position.x - 1, tileModel.Position.y);
            var rightPosition = new Vector2(tileModel.Position.x + 1, tileModel.Position.y);

            var topPossibleSpriteNames =
                tileConfig.GetPossibleSpritesOfATilesAtDirection(collapsedSpriteName, Direction.Top);
            var bottomPossibleSpriteNames =
                tileConfig.GetPossibleSpritesOfATilesAtDirection(collapsedSpriteName, Direction.Bot);
            var leftPossibleSpriteNames =
                tileConfig.GetPossibleSpritesOfATilesAtDirection(collapsedSpriteName, Direction.Left);
            var rightPossibleSpriteNames =
                tileConfig.GetPossibleSpritesOfATilesAtDirection(collapsedSpriteName, Direction.Right);

            foreach (var model in _grid.Tiles)
            {
                if (model.Position == topPosition)
                {
                    model.ClearTilesWhichNotIn(topPossibleSpriteNames);
                }
                else if (model.Position == bottomPosition)
                {
                    model.ClearTilesWhichNotIn(bottomPossibleSpriteNames);
                }
                else if (model.Position == leftPosition)
                {
                    model.ClearTilesWhichNotIn(leftPossibleSpriteNames);
                }
                else if (model.Position == rightPosition)
                {
                    model.ClearTilesWhichNotIn(rightPossibleSpriteNames);
                }
            }

            UpdatePriorities(_tileQueue, tile => tile.Priority);
            await UniTask.Delay(1);
            DoWaveFunctionCollapseRecursively();

            return false;
        }

        private void GenerateAllTiles()
        {
            foreach (var gameObj in _allTileObjects)
            {
                Destroy(gameObj);
            }
            
            _allTileObjects.Clear();
            _tileQueue.Clear();
            for (int column = 0; column < Columns; column++)
            {
                for (int row = 0; row < Rows; row++)
                {
                    var newTiles = Instantiate(tilesPrefab, new Vector3(column + Offset.x, row + Offset.y),
                        Quaternion.identity);
                    newTiles.name = $"Tiles {column} - {row}";
                    _allTileObjects.Add(newTiles);
                    var tileModel = new TileModel(column, row, newTiles, tileConfig.GetAllSpriteNames());
                    _grid.AddTile(tileModel);
                    _tileQueue.Enqueue(tileModel,tileModel.Priority);
                }
            }

            var random = Random.Range(0, _grid.Tiles.Count);
            Debug.Log(random);
            for (int i = 0; i < random; i++)
            {
                var tileModel = _tileQueue.Dequeue();
                _tileQueue.Enqueue(tileModel,tileModel.Priority);
            }
        }
        
        private void UpdatePriorities<T,V>(PriorityQueue<T,V> queue, Func<T, V> prioritySelector) {
            // Create a temporary list to store the elements in the queue
            List<T> tempList = new List<T>(queue.Count);
            while (queue.Count > 0) {
                tempList.Add(queue.Dequeue());
            }
    
            // Update the priority of each element in the list
            foreach (T item in tempList) {
                V priority = prioritySelector(item);
                queue.Enqueue(item, priority);
            }
        }
    }

    [Serializable]
    public class Grid
    {
        public List<TileModel> Tiles = new List<TileModel>();
        
        public void AddTile(TileModel tile)
        {
            Tiles.Add(tile);
            tile.SetGrid(this);
        }
    }

    [Serializable]
    public class TileModel
    {
        public Vector2 Position;
        public List<string> PossibleTiles;
        public GameObject TileGameObject;
        
        [NonSerialized]
        public Grid Grid;
        private readonly SpriteRenderer _spriteRenderer;
        private TileConfig _tileConfig;
        public bool HasCollapsed = false;
        public int Priority => PossibleTiles.Count;

        public TileModel(int x,int y,GameObject tileGameObject ,List<string> possibleTiles)
        {
            Position = new Vector2(x,y);
            TileGameObject = tileGameObject;
            _spriteRenderer = TileGameObject.GetComponent<SpriteRenderer>();
            PossibleTiles = possibleTiles.CloneByExpressionTree();
            _tileConfig = Locator<TileConfig>.Instance;
        }
        
        public void SetGrid(Grid grid)
        {
            Grid = grid;
        }

        public string Collapse()
        {
            if (PossibleTiles.Count != 0)
            {
                var randomTile = PossibleTiles[Random.Range(0, PossibleTiles.Count)];
                PossibleTiles.Clear();
                PossibleTiles.Add(randomTile);
                _spriteRenderer.sprite = _tileConfig.Find(randomTile).MainSprite;
                HasCollapsed = true;

                return randomTile;
            }

            return string.Empty;
        }

        public void ClearTilesWhichNotIn(List<string> possibleSpriteNames)
        {
            PossibleTiles = PossibleTiles.Where(possibleSpriteNames.Contains).ToList();
        }
    }
}