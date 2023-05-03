using Cysharp.Threading.Tasks;
using Game.Enums;
using Maniac.Command;

namespace Game.Scenes.NetGamePlay.Commands
{
    public class MovePlayersToCellAtDirectionCommand : Command
    {
        private readonly Direction _direction;

        public MovePlayersToCellAtDirectionCommand(Direction direction)
        {
            _direction = direction;
        }

        public override async UniTask Execute()
        {
            throw new System.NotImplementedException();
        }
    }
}