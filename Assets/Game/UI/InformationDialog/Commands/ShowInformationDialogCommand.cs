using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.LanguageTableSystem;
using Maniac.UISystem.Command;
using Maniac.Utils;

namespace Game.Commands
{
    public class ShowInformationDialogCommand : Command
    {
        private LanguageTable _languageTable => Locator<LanguageTable>.Instance;

        private string _header;
        private string _body;
        private string _confirm;

        public ShowInformationDialogCommand(string header, string body, string confirm = "")
        {
            _header = header;
            _body = body;

            if (confirm == string.Empty)
                confirm = LanguageTable.General_Confirm;

            _confirm = confirm;
        }

        public override async UniTask Execute()
        {
            var headerLanguageItem = _languageTable.Get(_header);
            var bodyLanguageItem = _languageTable.Get(_body);
            var confirmLanguageItem = _languageTable.Get(_confirm);

            var confirmParam = new InformationDialog.Param()
            {
                header = headerLanguageItem != null ? headerLanguageItem.GetCurrentLanguageText() : _header,
                body = bodyLanguageItem != null ? bodyLanguageItem.GetCurrentLanguageText() : _body,
                confirm = confirmLanguageItem != null ? confirmLanguageItem.GetCurrentLanguageText() : _confirm,
            };

            await new ShowScreenCommand<InformationDialog>(confirmParam).ExecuteAndReturnResult();
            await UniTask.CompletedTask;
        }
    }
}