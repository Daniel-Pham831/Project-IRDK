using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Game.MazeSystem;
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
        
        private Stack<TileModel> _collapsedTiles = new Stack<TileModel>();

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
            await DoWaveFunctionCollapseRecursively();
        }

        private async UniTask DoWaveFunctionCollapseRecursively()
        {
            if (!_tileQueue.TryDequeue(out var tileModel, out var priority))
            {
                Debug.Log("Done!");
                return;
            }

            var collapsedSpriteName = tileModel.Collapse();
            if (collapsedSpriteName == string.Empty)
            {
                tileModel.PossibleTiles = tileConfig.GetAllSpriteNames();
                _tileQueue.Enqueue(tileModel,tileModel.Priority);
                if(_collapsedTiles.TryPop(out var lastTile))
                {
                    lastTile.ReCollapse();
                    _tileQueue.Enqueue(lastTile, lastTile.Priority);
                }
                DoWaveFunctionCollapseRecursively();
                return;
            }

            ReduceEntropyOfNeighbors(tileModel, collapsedSpriteName);
            UpdatePriorities(_tileQueue, tile => tile.Priority);
            _collapsedTiles.Push(tileModel);
            await UniTask.Delay(1);
            DoWaveFunctionCollapseRecursively();
        }

        private void ReduceEntropyOfNeighbors(TileModel tileModel, string collapsedSpriteName)
        {
            var topPosition = new Vector2(tileModel.Position.x, tileModel.Position.y + 1);
            var bottomPosition = new Vector2(tileModel.Position.x, tileModel.Position.y - 1);
            var leftPosition = new Vector2(tileModel.Position.x - 1, tileModel.Position.y);
            var rightPosition = new Vector2(tileModel.Position.x + 1, tileModel.Position.y);

            var topPossibleSpriteNames =
                tileConfig.GetPossibleSpritesOfATilesAtDirection(collapsedSpriteName, Direction.Top);
            var bottomPossibleSpriteNames =
                tileConfig.GetPossibleSpritesOfATilesAtDirection(collapsedSpriteName, Direction.Bottom);
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
        
        public List<TileModel> GetAllCollapsedNeighbors(Vector2 Position)
        {
            var topPosition = new Vector2(Position.x, Position.y + 1);
            var bottomPosition = new Vector2(Position.x, Position.y - 1);
            var leftPosition = new Vector2(Position.x - 1, Position.y);
            var rightPosition = new Vector2(Position.x + 1, Position.y);

            var allCollapsedNeighbors = new List<TileModel>();
            foreach (var model in Tiles)
            {
                if (model.Position == topPosition && model.HasCollapsed)
                {
                    allCollapsedNeighbors.Add(model);
                }
                else if (model.Position == bottomPosition && model.HasCollapsed)
                {
                    allCollapsedNeighbors.Add(model);
                }
                else if (model.Position == leftPosition && model.HasCollapsed)
                {
                    allCollapsedNeighbors.Add(model);
                }
                else if (model.Position == rightPosition && model.HasCollapsed)
                {
                    allCollapsedNeighbors.Add(model);
                }
            }

            return allCollapsedNeighbors;
        }
    }

    [Serializable]
    public class TileModel
    {
        public Vector2 Position;
        public List<string> PossibleTiles;
        public List<string> CollapsedTiles = new List<string>();
        public GameObject TileGameObject;
        public bool HasCollapsed => PossibleTiles.Count == 1;
        
        [NonSerialized]
        public Grid Grid;
        private readonly SpriteRenderer _spriteRenderer;
        private TileConfig _tileConfig;
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
            var allCollapsedNeighbors = Grid.GetAllCollapsedNeighbors(Position);
            FilterPossibleTiles(allCollapsedNeighbors);
            if (PossibleTiles.Count != 0)
            {
                return CollapseToASingleTile();
            }

            return string.Empty;
        }

        private void FilterPossibleTiles(List<TileModel> allCollapsedNeighbors)
        {
            foreach (var tileModel in allCollapsedNeighbors)
            {
                var topPossibleSpriteNames =
                    _tileConfig.GetPossibleSpritesOfATilesAtDirection(tileModel.PossibleTiles[0], Direction.Top);
                var bottomPossibleSpriteNames =
                    _tileConfig.GetPossibleSpritesOfATilesAtDirection(tileModel.PossibleTiles[0], Direction.Bottom);
                var leftPossibleSpriteNames =
                    _tileConfig.GetPossibleSpritesOfATilesAtDirection(tileModel.PossibleTiles[0], Direction.Left);
                var rightPossibleSpriteNames =
                    _tileConfig.GetPossibleSpritesOfATilesAtDirection(tileModel.PossibleTiles[0], Direction.Right);

                if (Position == new Vector2(tileModel.Position.x, tileModel.Position.y + 1))
                {
                    PossibleTiles = PossibleTiles.Where(topPossibleSpriteNames.Contains).ToList();
                }
                else if (Position == new Vector2(tileModel.Position.x, tileModel.Position.y - 1))
                {
                    PossibleTiles = PossibleTiles.Where(bottomPossibleSpriteNames.Contains).ToList();
                }
                else if (Position == new Vector2(tileModel.Position.x - 1, tileModel.Position.y))
                {
                    PossibleTiles = PossibleTiles.Where(leftPossibleSpriteNames.Contains).ToList();
                }
                else if (Position == new Vector2(tileModel.Position.x + 1, tileModel.Position.y))
                {
                    PossibleTiles = PossibleTiles.Where(rightPossibleSpriteNames.Contains).ToList();
                }
            }
        }

        private string CollapseToASingleTile()
        {
            var randomTile = PossibleTiles[Random.Range(0, PossibleTiles.Count)];
            PossibleTiles.Remove(randomTile);

            CollapsedTiles.Clear();
            CollapsedTiles.AddRange(PossibleTiles);

            PossibleTiles.Clear();
            PossibleTiles.Add(randomTile);
            _spriteRenderer.sprite = _tileConfig.Find(randomTile).MainSprite;
            return randomTile;
        }

        public void ReCollapse()
        {
            PossibleTiles.Clear();
            PossibleTiles.AddRange(CollapsedTiles);
            CollapsedTiles.Clear();
        }

        public void ClearTilesWhichNotIn(List<string> possibleSpriteNames)
        {
            try
            {
                PossibleTiles = PossibleTiles.Where(possibleSpriteNames.Contains).ToList();
            }
            catch
            {
            }
        }
    }
}