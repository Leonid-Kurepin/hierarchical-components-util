using System;
using System.Windows.Forms;

namespace ASKON_TestTask.Forms
{
    public partial class RenameDetailForm : Form
    {
        private readonly FormsHelper _formsHelper;

        public string EnteredName { get; set; }

        public RenameDetailForm(FormsHelper formsHelper)
        {
            _formsHelper = formsHelper;

            InitializeComponent();

            tbNewDetailName.Select();
        }

        private async void btnRenameDetail_ClickAsync(object sender, EventArgs e)
        {
            EnteredName = tbNewDetailName.Text;

            var isValidDetailName = await _formsHelper.IsValidDetailNameAsync(
                labelRenameDetailError,
                EnteredName);

            if (isValidDetailName)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void tbNewDetailName_Enter(object sender, EventArgs e)
        {
            FormsHelper.HideErrorMessage(labelRenameDetailError);
        }
    }
}
