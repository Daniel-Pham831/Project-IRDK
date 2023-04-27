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

            foreach (var wall in cellModel.Walls)
            {
                switch (wall)
                {
                    case Wall.Top:
                        topPath.SetActive(false);
                        break;
                    case Wall.Right:
                        rightPath.SetActive(false);
                        break;
                    case Wall.Bot:
                        botPath.SetActive(false);
                        break;
                    case Wall.Left:
                        leftPath.SetActive(false);
                        break;
                }
            }
        }
    }
}