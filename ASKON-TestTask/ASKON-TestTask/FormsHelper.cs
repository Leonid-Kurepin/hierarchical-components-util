using ASKON_TestTask.Forms;
using ASKON_TestTask.Services;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASKON_TestTask
{
    public class FormsHelper
    {
        private static readonly int _maxDetailNameLength = 255;
        private readonly IDetailService _detailService;

        public FormsHelper(IDetailService detailService)
        {
            _detailService = detailService;
        }

        internal async Task<bool> IsValidDetailNameAsync(
            Label errorHandlerLabel,
            string detailName,
            bool checkNameUniqueness = true)
        {
            if (string.IsNullOrEmpty(detailName))
            {
                ShowErrorMessage(errorHandlerLabel, "Type smthng!");

                return false;
            }

            if (detailName.Length > _maxDetailNameLength)
            {
                ShowErrorMessage(errorHandlerLabel, "Name is too long.");

                return false;
            }

            if (checkNameUniqueness)
            {
                var isUniqueName = await _detailService.IsUniqueDetailNameAsync(detailName);

                if (!isUniqueName)
                {
                    ShowErrorMessage(errorHandlerLabel, "Not unique!");

                    return false;
                }
            }

            return true;
        }


        private static void ShowErrorMessage(Label errorHandlerLabel, string messageText)
        {
            errorHandlerLabel.Text = messageText;
            errorHandlerLabel.Visible = true;
        }

        internal static void HideErrorMessage(Label errorHandlerLabel)
        {
            errorHandlerLabel.Visible = false;
        }

        internal static void ShowMessageDialog(string messageText)
        {
            using var messageDialogForm = new MessageForm(messageText);
            if (messageDialogForm.ShowDialog() == DialogResult.OK)
            {
                return;
            }
        }
    }
}
