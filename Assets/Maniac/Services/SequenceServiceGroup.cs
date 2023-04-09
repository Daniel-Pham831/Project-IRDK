using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Maniac.Services
{
    public class SequenceServiceGroup : Service , IDisposable
    {
        private readonly string _serviceGroupName;
        private readonly Queue<Service> _serviceQueue;
        private int _totalServices;
        protected override string Name => _serviceGroupName;
        public FloatReactiveProperty Progress { get; private set; } = new FloatReactiveProperty();

        public SequenceServiceGroup(string serviceGroupName)
        {
            _serviceGroupName = serviceGroupName;
            _serviceQueue = new Queue<Service>();
        }

        public void Add(Service newService)
        {
            _serviceQueue.Enqueue(newService);
        }

        public override async UniTask<IService.Result> Execute()
        {
            Progress.Value = 0;
            _totalServices = _serviceQueue.Count;
            while (_serviceQueue.Count != 0)
            {
                var service = _serviceQueue.Dequeue();
                await service.Run();
                Progress.Value = 1 - ((float)_serviceQueue.Count / _totalServices);
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