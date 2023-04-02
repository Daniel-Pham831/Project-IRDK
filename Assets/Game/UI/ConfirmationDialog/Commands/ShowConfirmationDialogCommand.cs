using Cysharp.Threading.Tasks;
using Maniac.Command;
using Maniac.LanguageTableSystem;
using Maniac.UISystem.Command;
using Maniac.Utils;

namespace Game.Commands
{
    public class ShowConfirmationDialogCommand : Command
    {
        protected LanguageTable _languageTable => Locator<LanguageTable>.Instance;

        protected string _header;
        protected string _body;
        protected string _confirm;
        protected string _cancel;

        protected ConfirmationDialog.Result _result = ConfirmationDialog.Result.Cancel;

        public ShowConfirmationDialogCommand(string header, string body, string confirm = "", string cancel = "")
        {
            _header = header;
            _body = body;

            if (confirm == string.Empty)
                confirm = LanguageTable.General_Confirm;

            if (cancel == string.Empty)
                cancel = LanguageTable.General_Cancel;

            _confirm = confirm;
            _cancel = cancel;
        }

        public override async UniTask Execute()
        {
            var headerLanguageItem = _languageTable.Get(_header);
            var bodyLanguageItem = _languageTable.Get(_body);
            var confirmLanguageItem = _languageTable.Get(_confirm);
            var cancelLanguageItem = _languageTable.Get(_cancel);

            var confirmParam = new ConfirmationDialog.Param()
            {
                header = headerLanguageItem != null ? headerLanguageItem.GetCurrentLanguageText() : _header,
                body = bodyLanguageItem != null ? bodyLanguageItem.GetCurrentLanguageText() : _body,
                confirm = confirmLanguageItem != null ? confirmLanguageItem.GetCurrentLanguageText() : _confirm,
                cancel = cancelLanguageItem != null ? cancelLanguageItem.GetCurrentLanguageText() : _cancel,
            };

            _result =(ConfirmationDialog.Result) await ShowScreenCommand.Create<ConfirmationDialog>(confirmParam).ExecuteAndReturnResult();

            await UniTask.CompletedTask;
        }

        public async UniTask<ConfirmationDialog.Result> ExecuteAndReturnResult()
        {
            await Execute();
            return _result;
        }
    }
}