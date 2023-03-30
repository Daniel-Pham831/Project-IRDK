using System;
using Maniac.UISystem;
using Maniac.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Scenes.MainMenu
{
    public class InitMainMenu : MonoBehaviour
    {
        private UIManager _uiManager => Locator<UIManager>.Instance;

        private async void Awake()
        {
            await _uiManager.Show<MainMenuScreen>();
        }

        private void Start()
        {
            Destroy(this.gameObject);
        }
    }
}