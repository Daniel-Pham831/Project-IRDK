using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Game.CloudProfileSystem;
using Game.Networking;
using Maniac.UISystem;
using Maniac.UISystem.Command;
using Maniac.Utils;
using TMPro;

namespace Game
{
    public class AccountDetailsScreen : BaseUI
    {
        private CloudProfileManager _cloudProfileManager => Locator<CloudProfileManager>.Instance;
        private UserProfile _userProfile;

        [SerializeField] private TMP_Text userName;
        // This method will be call first, at this point, UI hasn't show up on canvas yet.
        // Use this to init your UI
        public override async void OnSetup(object parameter = null) //first
        {
            _userProfile = await _cloudProfileManager.Get<UserProfile>();
            
            userName.text = _userProfile.DisplayName;
            base.OnSetup(parameter);
        }

        public async void ShowUpdateUserNameClicked()
        {
            await ShowScreenCommand.Create<UpdateUserNameDialog>().ExecuteAndReturnResult();
            
            userName.text = _userProfile.DisplayName;
        }
    }
}
