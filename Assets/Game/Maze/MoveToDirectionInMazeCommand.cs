using Cysharp.Threading.Tasks;
using Game.Enums;
using Maniac.Command;
using Maniac.Utils;

namespace Game.Maze
{
    public class MoveToDirectionInMazeCommand : Command
    {
        private readonly Direction _direction;
        private MazeSystem _mazeSystem => Locator<MazeSystem>.Instance;
        private Maze _currentMaze => _mazeSystem.CurrentMaze;

        public MoveToDirectionInMazeCommand(Direction direction)
        {
            _direction = direction;
        }

        public override async UniTask Execute()
        {
            if (_currentMaze == null) return;

            if (_currentMaze.IsDirectionValid(_direction))
            {
                _currentMaze.MoveToDirection(_direction);
            }

            await UniTask.CompletedTask;
        }
    }
}