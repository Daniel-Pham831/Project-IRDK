using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Maniac.Services
{
    public class SequenceCommandServiceGroup : Service , IDisposable
    {
        private readonly string _groupName;
        private readonly Queue<Command.Command> _commandQueue;
        private int _totalCommands;
        protected override string Name => _groupName;
        public FloatReactiveProperty Progress { get; private set; } = new FloatReactiveProperty();

        public SequenceCommandServiceGroup(string groupName)
        {
            _groupName = groupName;
            _commandQueue = new Queue<Command.Command>();
        }

        public void Add(Command.Command newCommand)
        {
            _commandQueue.Enqueue(newCommand);
        }

        public override async UniTask<IService.Result> Execute()
        {
            Progress.Value = 0;
            _totalCommands = _commandQueue.Count;
            while (_commandQueue.Count != 0)
            {
                var command = _commandQueue.Dequeue();
                await command.Execute();
                Progress.Value = 1 - ((float)_commandQueue.Count / _totalCommands);
            }

            Progress.Value = 1;
            return IService.Result.Success;
        }

        public void Dispose()
        {
            Progress?.Dispose();
        }
    }
}