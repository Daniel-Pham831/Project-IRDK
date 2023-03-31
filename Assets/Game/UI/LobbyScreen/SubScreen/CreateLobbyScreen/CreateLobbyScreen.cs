using UnityEngine;
using Cysharp.Threading.Tasks;
using Maniac;
using DG.Tweening;
using Maniac.UISystem;
using TMPro;

namespace Game
{
    public class CreateLobbyScreen : BaseUI
    {
        [SerializeField] private TMP_InputField lobbyNameInput;

        public async void OnCreateClicked()
        {
            var trimmedName = lobbyNameInput.text.Trim();
            if (string.IsNullOrEmpty(trimmedName) || string.IsNullOrWhiteSpace(trimmedName))
            {
                ShowErrorInInputField();
            }
            else
            {
                Close(trimmedName);
            }
        }

        private void ShowErrorInInputField()
        {
            lobbyNameInput.transform.DOPunchScale(Vector3.one * 0.05f, 0.1f);
        }
    }
}
