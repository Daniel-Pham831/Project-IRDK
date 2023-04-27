using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Maniac.RandomSystem;
using Maniac.RunnerSystem;
using Maniac.Utils;
using Maniac.Utils.Extension;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.MazeSystem
{
    
    public enum Wall
    {
        Top,
        Bot,
        Left,
        Right
    }

    public class MazeGenerator
    {
        public Maze CurrentMaze { get; private set; }
        
        public void Init()
        {
            Locator<MazeGenerator>.Set(this);
        }
        
        public async UniTask GenerateNewMaze(Vector2Int mazeDimension)
        {
            CurrentMaze = new Maze(mazeDimension);
            var visitedCells = new List<Vector2Int>();
            await GenerateMazeRecursively(new Vector2Int(0,0),CurrentMaze,visitedCells);
        }
        
        private async UniTask GenerateMazeRecursively(Vector2Int currentPosition, Maze maze, List<Vector2Int> visitedCells)
        {
            visitedCells.Add(currentPosition);
            List<Vector2Int> unvisitedNeighbors = new List<Vector2Int>()
            {
                new(currentPosition.x, currentPosition.y + 1), 
                new(currentPosition.x, currentPosition.y - 1), 
                new(currentPosition.x - 1, currentPosition.y), 
                new(currentPosition.x + 1, currentPosition.y),
            };

            while (unvisitedNeighbors.Count > 0)
            {
                var randomNeighbor = unvisitedNeighbors.TakeRandomWithSeed(Locator<Randomer>.Instance.Seed);
                unvisitedNeighbors.Remove(randomNeighbor);
                
                if(!maze.IsCellValid(randomNeighbor) || visitedCells.Contains(randomNeighbor))
                    continue;
                    
                maze.BreakWallsBetween(currentPosition,randomNeighbor);
                await GenerateMazeRecursively(randomNeighbor, maze, visitedCells);
            }
        }
        
#if  UNITY_EDITOR
        [Sirenix.OdinInspector.Button("Export JSON", ButtonSizes.Medium)]
        [PropertyOrder(-1)]
        [HorizontalGroup("Buttons-JSON")]
        public void ExportToJSON()
        {
            string path = UnityEditor.EditorUtility.SaveFilePanel("Choose JSON File", Application.dataPath, "Maze", "json");
            if (!string.IsNullOrEmpty(path))
            {
                var json = JsonConvert.SerializeObject(CurrentMaze, Formatting.Indented);
                File.WriteAllText(path, json);
                Debug.Log("Exported!");
            }
        }
#endif
    }
}