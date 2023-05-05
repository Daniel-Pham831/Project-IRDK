using System;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Maniac.DataBaseSystem;
using Maniac.UISystem;
using Maniac.Utils;
using UnityEngine.UI;

namespace Game
{
    public class TransitionScreen : BaseUI
    {
        [SerializeField] private Image _mainImg;
        
        private DataBase _dataBase => Locator<DataBase>.Instance;
        private TransitionConfig _transitionConfig => _dataBase.GetConfig<TransitionConfig>();
        
        private TransitionInfo _transitionEnterInfo;
        private TransitionInfo _transitionExitInfo;

        public override void OnSetup(object parameter = null)
        {
            _transitionEnterInfo = _transitionConfig.GetRandomTransition();
            _transitionExitInfo = _transitionConfig.GetRandomTransition();
        }

        public override async UniTask OnTransitionEnter(object parameter = null)
        {
            await base.OnTransitionEnter(parameter);
            var timeBetweenFrames = _transitionConfig.DefaultTransitionEnterDuration / _transitionEnterInfo.SpriteFrames.Count;

            foreach (var spriteFrame in _transitionEnterInfo.SpriteFrames)
            {
                _mainImg.sprite = spriteFrame;
                await UniTask.Delay(TimeSpan.FromSeconds(timeBetweenFrames));
            }
        }

        public override async UniTask OnTransitionExit()
        {
            var timeBetweenFrames = _transitionConfig.DefaultTransitionExitDuration / _transitionExitInfo.SpriteFrames.Count;
            for (int i = _transitionExitInfo.SpriteFrames.Count - 1; i >= 0; i--)
            {
                _mainImg.sprite = _transitionExitInfo.SpriteFrames[i];
                
                await UniTask.Delay(TimeSpan.FromSeconds(timeBetweenFrames));
            }
            
            await base.OnTransitionExit();
        }
    }
}
