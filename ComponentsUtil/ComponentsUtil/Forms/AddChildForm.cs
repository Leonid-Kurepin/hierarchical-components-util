using System;
using System.Windows.Forms;

namespace ComponentsUtil.Forms
{
    public partial class AddChildForm : Form
    {
        private readonly FormsHelper _formsHelper;

        public string EnteredName { get; set; }
        public int CountToAdd { get; set; }

        public AddChildForm(FormsHelper formsHelper)
        {
            _formsHelper = formsHelper;

            InitializeComponent();

            numericUpDownCountToAdd.Value = numericUpDownCountToAdd.Minimum = 1;

            tbDetailName.Select();
        }

        private async void btnAdd_ClickAsync(object sender, EventArgs e)
        {
            EnteredName = tbDetailName.Text;
            CountToAdd = (int)numericUpDownCountToAdd.Value;

            var isValidDetailName = await _formsHelper.IsValidDetailNameAsync(
                labelAddChildError,
                EnteredName,
                false);

            if (isValidDetailName)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void tbDetailName_Enter(object sender, EventArgs e)
        {
            FormsHelper.HideErrorMessage(labelAddChildError);
        }
    }
}
