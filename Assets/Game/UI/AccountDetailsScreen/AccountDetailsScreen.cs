using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Game.Networking;
using Maniac.UISystem;
using Maniac.UISystem.Command;
using Maniac.Utils;
using TMPro;

namespace Game
{
    public class AccountDetailsScreen : BaseUI
    {
        private LocalData LocalData => Locator<LocalData>.Instance;

        [SerializeField] private TMP_Text userName;
        // This method will be call first, at this point, UI hasn't show up on canvas yet.
        // Use this to init your UI
        public override void OnSetup(object parameter = null) //first
        {
            userName.text = LocalData.LocalPlayer.DisplayName;
            base.OnSetup(parameter);
        }

        public async void ShowUpdateUserNameClicked()
        {
            await ShowScreenCommand.Create<UpdateUserNameDialog>().ExecuteAndReturnResult();
            
            userName.text = LocalData.LocalPlayer.DisplayName;
        }
    }
}
