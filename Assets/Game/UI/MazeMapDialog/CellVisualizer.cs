using Game.Enums;
using Game.Maze;
using Game.Trader;
using Maniac.DataBaseSystem;
using Maniac.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.MazeMapDialog
{
    public class CellVisualizer : MonoBehaviour
    {
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private MazeSystem _mazeSystem => Locator<MazeSystem>.Instance;
        private TraderSystem _traderSystem => Locator<TraderSystem>.Instance;
        private MazeConfig _mazeConfig => _dataBase.GetConfig<MazeConfig>();
        private Maze.Maze _maze => _mazeSystem.CurrentMaze;
        
        [SerializeField] private GameObject topPath;
        [SerializeField] private GameObject rightPath;
        [SerializeField] private GameObject botPath;
        [SerializeField] private GameObject leftPath;
        [SerializeField] private Image bodyImg;
        [SerializeField] private Image innerImg;
        [SerializeField] private LayoutElement layoutElement;

        private Cell _cellModel;
        public Cell CellModel => _cellModel;

        public void Setup(Cell cellModel)
        {
            _cellModel = cellModel;
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

            SetupColor();
            SetIgnoreLayout(false);
        }
        
        public void SetIgnoreLayout(bool ignore)
        {
            layoutElement.ignoreLayout = ignore;
        }

        private void SetupColor()
        {
            if (_cellModel.Position == _maze.CurrentObserveCell.Value.Position)
            {
                bodyImg.color = _mazeConfig.YouAreHereColor;
            }
            else if (_cellModel.Position == _maze.EndCell.Position)
            {
                bodyImg.color = _mazeConfig.EndCellColor;
            }
            else if (_cellModel.Position == _maze.StartCell.Position)
            {
                bodyImg.color = _mazeConfig.StartCellColor;
            }
            else
            {
                bodyImg.color = _mazeConfig.NormalCellColor;
            }

            var doesCellContainTrader = _traderSystem.DoesCellContainTrader(_cellModel);
            innerImg.sprite = _mazeConfig.TraderSmallSprite;
            innerImg.gameObject.SetActive(doesCellContainTrader);
        }
    }
}