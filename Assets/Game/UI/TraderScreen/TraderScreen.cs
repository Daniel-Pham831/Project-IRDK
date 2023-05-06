using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Maniac.UISystem;
using Maniac.UISystem.Command;

namespace Game
{
    public class TraderScreen : BaseUI
    {
        public async void OnViewMapClicked()
        {
            await new ShowScreenCommand<MazeMapDialog>().Execute();
        }
    }
}
