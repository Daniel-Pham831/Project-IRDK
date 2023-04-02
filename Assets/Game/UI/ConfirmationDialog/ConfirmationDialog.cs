using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Maniac.UISystem;
using TMPro;

namespace Game
{
    public class ConfirmationDialog : BaseUI
    {
        public enum Result
        {
            Confirm,
            Cancel
        }

        public class Param
        {
            public string header;
            public string body;
            public string confirm;
            public string cancel;
        }
        
        [SerializeField] private TMP_Text headerText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private TMP_Text confirmText;
        [SerializeField] private TMP_Text cancelText;

        private Param _param;
        
        public override void OnSetup(object parameter = null) //first
        {
            base.OnSetup(parameter);
            
            _param = parameter as Param;
            
            headerText.text = _param.header;
            bodyText.text = _param.body;
            confirmText.text = _param.confirm;
            cancelText.text = _param.cancel;
        }

        public async void OnConfirmClicked()
        {
            await Close(Result.Confirm);
        }

        public async void OnCancelClicked()
        {
            await Close(Result.Cancel);
        }
    }
}
