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
using Game.Coin;
using TMPro;
using Maniac.LanguageTableSystem;

namespace Game
{
    public class PlayerInGameScreen : BaseUI
    {
        private NetPlayer _netPlayer  => Locator<NetPlayer>.Instance;
        private CoinSystem _coinSystem => Locator<CoinSystem>.Instance;
        private LanguageTable _languageTable => Locator<LanguageTable>.Instance;

        [SerializeField] private TextMeshProUGUI txtSharecoin;
        [SerializeField] private TextMeshProUGUI txtPrivatecoin;
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
            //update sharecoin to UI
            _coinSystem.SharedCoin.Subscribe(value => {

                var CoinShare = _languageTable.Get(LanguageTable.Coin_Shared);
                txtSharecoin.text = CoinShare.Format(value);
            }).AddTo(this);
            //Update Privatecoin to UI
            _coinSystem.PrivateCoin.Subscribe(value =>
            {
                var privateCoinShare = _languageTable.Get(LanguageTable.Coin_Private);
                txtPrivatecoin.text = privateCoinShare.Format(value);// string.Format($" Private Coin :{value.ToString()}");
            }).AddTo(this);

            interactButton.onClick.AddListener(OnInteractClicked);
        }

        public async void OnInteractClicked()
        {
            await new InteractWithInteractableCommand().Execute();
        }
    }
}
