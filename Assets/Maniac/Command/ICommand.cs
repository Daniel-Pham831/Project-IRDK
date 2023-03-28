using Cysharp.Threading.Tasks;

namespace Maniac.Command
{
    public interface ICommand
    {
        UniTask Execute();
    }

    public abstract class Command : ICommand
    {
        public abstract UniTask Execute();
    }

    public abstract class ResultCommand : Command
    {
        protected object _result = null;
        public async UniTask<object> ExecuteAndGetResult()
        {
            await Execute();
            return HasValidResult() ? _result : default;
        }

        private bool HasValidResult()
        {
            return _result != null;
        }
    }

    public abstract class ResultCommand<T> : Command
    {
        protected T _result = default;
        public async UniTask<T> ExecuteAndGetResult()
        {
            await Execute();
            return HasValidResult() ? _result : default;
        }

        private bool HasValidResult()
        {
            return _result != null;
        }
    }
}