using System;
using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.LanguageTableSystem;
using Maniac.Utils;
using UnityEngine;

namespace Game
{
    public class ShowAboveNotificationCommand : Command
    {
        private readonly string _content;
        private readonly Action _onClicked;
        private readonly Sprite _icon;
        private readonly float _duration;
        
        private LanguageTable _languageTable => Locator<LanguageTable>.Instance;
        private AboveNotificationSystem _aboveNotificationSystem => Locator<AboveNotificationSystem>.Instance;

        public ShowAboveNotificationCommand(string content, Action onClicked = null, Sprite icon = null,
            float duration = 3f)
        {
            _content = content;
            _onClicked = onClicked;
            _icon = icon;
            _duration = duration;
        }

        public override async UniTask Execute()
        {
            var contentLanguageItem = _languageTable.Get(_content);

            var aboveNotiParam = new AboveNotificationDialog.Param()
            {
                Content = contentLanguageItem != null ? contentLanguageItem.GetCurrentLanguageText() : _content,
                OnClicked = _onClicked,
                Icon = _icon,
                Duration = _duration,
            };
            
            await _aboveNotificationSystem.AddToQueue(aboveNotiParam);
        }
    }
}