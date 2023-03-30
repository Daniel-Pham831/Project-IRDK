using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Maniac.UISystem;

namespace Game
{
    public class ConnectToServerLoadingScreen : BaseUI
    {
        [SerializeField] private Transform loadingImage;
        
        // This method will be call first, at this point, UI hasn't show up on canvas yet.
        // Use this to init your UI
        public override void OnSetup(object parameter = null) //first
        {
            loadingImage.DORotate(loadingImage.forward * 540, 1f, RotateMode.FastBeyond360).SetEase(Ease.Linear)
                .SetLoops(-1);
            
            base.OnSetup(parameter);
        }
    }
}
