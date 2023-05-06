using Cysharp.Threading.Tasks;
using Game.Interfaces;
using Game.Maze;
using Game.Players.Scripts;
using Maniac.UISystem.Command;
using Maniac.Utils;
using UniRx;
using UnityEngine;

namespace Game.Trader
{
    public class TraderController : MonoLocator<TraderController>, IInteractableMono
    {
        private TraderSystem _traderSystem => Locator<TraderSystem>.Instance;
        private Maze.MazeSystem _mazeSystem => Locator<Maze.MazeSystem>.Instance;
        private NetPlayer _netPlayer => Locator<NetPlayer>.Instance;
        
        [SerializeField] private GameObject trader;
        [SerializeField] private GameObject traderHouse;
        [SerializeField] private Collider2D traderInteractCollider2D;
        
        private Cell _currentCell;
        private Maze.Maze _currentMaze;

        public MonoBehaviour Mono => this;
        public int InteractPriority => 1;
        
        public override void Awake()
        {
            base.Awake();
            
            _currentMaze = _mazeSystem.CurrentMaze;
            ObserveMaze();
        }

        private void ObserveMaze()
        {
            _currentMaze.CurrentObserveCell.Subscribe(SetupEnvironment).AddTo(this);
            _currentMaze.NotifyCellChanged();
        }

        private void SetupEnvironment(Cell cell)
        {
            _currentCell = cell;
            SetupTrader();
        }

        private void SetupTrader()
        {
            var hasTrader = _traderSystem.DoesCellContainTrader(_currentCell);
            
            trader.SetActive(hasTrader);
            traderHouse.SetActive(hasTrader);
            traderInteractCollider2D.enabled = hasTrader;
        }
        

        public async UniTask Interact(object interactor = null)
        {
            await new ShowScreenCommand<TraderScreen>().Execute();
        }
    }
}