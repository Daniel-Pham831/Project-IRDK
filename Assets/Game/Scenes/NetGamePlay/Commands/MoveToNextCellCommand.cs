﻿using Cysharp.Threading.Tasks;
using Game.Enums;
using Game.Maze;
using Game.Players.Commands;
using Maniac.Command;

namespace Game.Scenes.NetGamePlay.Commands
{
    public class MoveToNextCellCommand : Command
    {
        private readonly Direction _moveDirection;

        public MoveToNextCellCommand(Direction moveDirection)
        {
            _moveDirection = moveDirection;
        }
        
        public override async UniTask Execute()
        {
            await new HidePlayerInGameScreenCommand().Execute();
            await new DisablePlayerInputCommand().Execute();
            await new ShowTransitionCommand().Execute();
            
            await new UpdatePlayerPositionInGamePlayCommand(_moveDirection.GetOppositeDirection()).Execute();
            await new MoveToDirectionInMazeCommand(_moveDirection).Execute();
            
            await new HideTransitionCommand().Execute();
            await new EnablePlayerInputCommand().Execute();
            await new ShowPlayerInGameScreenCommand().Execute();
        }
    }
}