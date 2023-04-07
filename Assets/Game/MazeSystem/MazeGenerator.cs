using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
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
        public async void GenerateNewMaze()
        {
            var maze = new Maze(new Vector2Int(3, 5));
            var visitedCells = new List<Vector2Int>();
            await GenerateMazeRecursively(new Vector2Int(0,0),maze,visitedCells);
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
                var randomNeighbor = unvisitedNeighbors.TakeRandom();
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
        public void ExportToJSON(Maze maze)
        {
            string path = UnityEditor.EditorUtility.SaveFilePanel("Choose JSON File", Application.dataPath, "Maze", "json");
            if (!string.IsNullOrEmpty(path))
            {
                var json = JsonConvert.SerializeObject(maze, Formatting.Indented);
                File.WriteAllText(path, json);
                Debug.Log("Exported!");
            }
        }
#endif
    }
}