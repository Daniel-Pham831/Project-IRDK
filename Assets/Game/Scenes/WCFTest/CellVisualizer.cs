using Game.MazeSystem;
using UnityEngine;

namespace Game.Scenes.WCFTest
{
    public class CellVisualizer : MonoBehaviour
    {
        [SerializeField] private GameObject topPath;
        [SerializeField] private GameObject rightPath;
        [SerializeField] private GameObject botPath;
        [SerializeField] private GameObject leftPath;
        
        public void Setup(Cell cellModel)
        {
            transform.position = (Vector2)cellModel.Position;
            name = $"Cell {cellModel.Position.x} {cellModel.Position.y}";

            foreach (var wall in cellModel.Walls)
            {
                switch (wall)
                {
                    case Direction.Top:
                        topPath.SetActive(false);
                        break;
                    case Direction.Right:
                        rightPath.SetActive(false);
                        break;
                    case Direction.Bottom:
                        botPath.SetActive(false);
                        break;
                    case Direction.Left:
                        leftPath.SetActive(false);
                        break;
                }
            }
        }
    }
}