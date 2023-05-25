using UnityEngine;
using Maniac;
using DG.Tweening;
using Game.Players.Commands;
using Game.Players.Scripts;
using Game.WeaponUI;
using Maniac.UISystem;
using Maniac.Utils;
using UniRx;
using UnityEngine.UI;

namespace Game
{
    public class PlayerInGameScreen : BaseUI
    {
        private NetPlayer _netPlayer  => Locator<NetPlayer>.Instance;
        
        [SerializeField] private Button interactButton;
        [SerializeField] private WeaponUIInPlayerInGame weaponUIInPlayerInGame;

        public override async void OnSetup(object parameter)
        {
            base.OnSetup(parameter);
            SetupInteractButton();
            await weaponUIInPlayerInGame.Init();
        }

        private void SetupInteractButton()
        {
            _netPlayer.NetPlayerInput.IsInteractable.Subscribe(value =>
                {
                    interactButton.gameObject.SetActive(value);
                }
            ).AddTo(this);

            interactButton.onClick.AddListener(OnInteractClicked);
        }

        public async void OnInteractClicked()
        {
            await new InteractWithInteractableCommand().Execute();
        }
    }
}
