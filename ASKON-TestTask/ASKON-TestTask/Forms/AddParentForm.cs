using System;
using System.Windows.Forms;

namespace ASKON_TestTask.Forms
{
    public partial class AddParentForm : Form
    {
        private readonly FormsHelper _formsHelper;

        public string EnteredName { get; set; }

        public AddParentForm(FormsHelper formsHelper)
        {
            _formsHelper = formsHelper;

            InitializeComponent();

            tbAddParentName.Select();
        }

        private async void btnAddParent_ClickAsync(object sender, EventArgs e)
        {
            EnteredName = tbAddParentName.Text;

            var isValidDetailName = await _formsHelper.IsValidDetailNameAsync(
                labelAddParentError,
                EnteredName);

            if (isValidDetailName)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void tbAddParentName_Enter(object sender, EventArgs e)
        {
            FormsHelper.HideErrorMessage(labelAddParentError);
        }
    }
}