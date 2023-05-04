using System;
using Cysharp.Threading.Tasks;
using Maniac.Utils;
using UnityEngine;

namespace Maniac.UISystem.Command
{
    public class ShowScreenCommand<T> : Maniac.Command.Command where T : BaseUI
    {
        private UIManager uiManager => Locator<UIManager>.Instance;
        private object _parameter;
        private object _result;
        private BaseUI ui;
        public BaseUI ShowedUI => ui;

        public ShowScreenCommand(object parameter = null)
        {
            _parameter = parameter;
        }
        
        public override async UniTask Execute()
        {
            ui = await uiManager.Show<T>(_parameter);
            if (ui == null)
            {
                Debug.Log($"Something wrong with {typeof(T)} UI.");
                return;
            }
            ui.OnClose += (param) => _result = param;
        }

        public async UniTask<object> ExecuteAndReturnResult()
        {
            await Execute();
            await WaitCompletion();
            if (HasValidResult())
                return _result;
            else
                return default;
        }

        private bool HasValidResult()
        {
            return _result != null;
        }

        private async UniTask WaitCompletion()
        {
            await ui.WaitCompletion();
        }
    }
}