using System;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Maniac.TimeSystem;
using Maniac.UISystem;
using Maniac.Utils;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UI;

namespace Game
{
    public class AboveNotificationDialog : BaseUI
    {
        private TimeManager timeManager => Locator<TimeManager>.Instance;
        
        [SerializeField] private TMP_Text contentTxt;
        [SerializeField] private Image icon;
        private Param _param;

        private bool _shouldInvokeOnClicked = false;

        public override void OnSetup(object parameter = null)
        {
            base.OnSetup(parameter);
            
            _param = parameter as Param;
            contentTxt.text = _param.Content;
            if (_param.Icon != null)
            {
                icon.sprite = _param.Icon;
            }
            icon.gameObject.SetActive(_param.Icon != null);
            
            timeManager.OnTimeOut(() =>
            {
                if(this == null || this.gameObject == null)
                    return;
                
                Close();
            },_param.Duration);
        }

        public override async UniTask OnTransitionExit()
        {
            await base.OnTransitionExit();
            
            if(_shouldInvokeOnClicked)
                _param.OnClicked?.Invoke();
        }

        public async void OnClicked()
        {
            _shouldInvokeOnClicked = true;
            await Close();
        }

        #region Inner classes

        public class Param
        {
            public string Content { get; set; }
            public Sprite Icon { get; set; }
            public float Duration { get; set; }
            public Action OnClicked { get; set; }
        }

        #endregion
    }
}
