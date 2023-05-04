using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Maniac.UISystem.Command;
using UnityEngine;

namespace Game
{
    public class AboveNotificationSystem
    {
        private Queue<AboveNotificationDialog.Param> _aboveNotiQueue = new Queue<AboveNotificationDialog.Param>();
        
        public async UniTask Init()
        {
            await UniTask.CompletedTask;
            Run();
        }

        private async UniTask Run()
        {
            await ExecuteAboveNotiQueue();
        }
        
        private async UniTask ExecuteAboveNotiQueue()
        {
            if (_aboveNotiQueue.Count > 0)
            {
                var aboveNotiParam = _aboveNotiQueue.Dequeue();
                var isFinished = false;
                
                var showCommand = new ShowScreenCommand<AboveNotificationDialog>(aboveNotiParam);
                await showCommand.Execute();
                var notiUI = showCommand.ShowedUI;
                notiUI.OnClose += (_) => isFinished = true;

                var durationTask = UniTask.Delay((int)(aboveNotiParam.Duration * 1000));
                var actionTask = UniTask.WaitUntil(() => isFinished);

                Debug.Log("Run");
                await UniTask.WhenAny(durationTask, actionTask);
                Debug.Log("Finish");
            }

            await UniTask.DelayFrame(1);
            await ExecuteAboveNotiQueue();
        }

        public async UniTask AddToQueue(AboveNotificationDialog.Param aboveNotiParam)
        {
            _aboveNotiQueue.Enqueue(aboveNotiParam);
        }
    }
}