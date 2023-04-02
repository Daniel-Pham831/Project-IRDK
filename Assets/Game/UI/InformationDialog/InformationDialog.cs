using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Maniac.UISystem;
using TMPro;

namespace Game
{
    public class InformationDialog : BaseUI
    {
        public class Param
        {
            public string header;
            public string body;
            public string confirm;
        }
        
        [SerializeField] private TMP_Text headerText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private TMP_Text confirmText;
        
        public override void OnSetup(object parameter = null) //first
        {
            base.OnSetup(parameter);
            
            var param = parameter as Param;
            
            headerText.text = param.header;
            bodyText.text = param.body;
            confirmText.text = param.confirm;
        }
    }
}
