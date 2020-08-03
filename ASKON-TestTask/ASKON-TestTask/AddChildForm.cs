using ASKON_TestTask.Persistence;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ASKON_TestTask
{
    public partial class AddChildForm : Form
    {
        private readonly int _maxNameDetailLength = 255;

        public string EnteredName { get; set; }
        public int CountToAdd { get; set; }

        public AddChildForm()
        {
            InitializeComponent();

            numericUpDownCountToAdd.Value = numericUpDownCountToAdd.Minimum = 1;

            tbDetailName.Select();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            EnteredName = tbDetailName.Text;
            CountToAdd = (int)numericUpDownCountToAdd.Value;

            if (IsValidDetailName(EnteredName))
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private bool IsValidDetailName(string detailName)
        {
            if (string.IsNullOrEmpty(detailName))
            {
                ShowErrorMessage("Type smthng!");

                return false;
            }

            if (detailName.Length > _maxNameDetailLength)
            {
                ShowErrorMessage("Name is too long.");

                return false;
            }

            return true;
        }

        private void tbDetailName_Enter(object sender, EventArgs e)
        {
            HideErrorMessage();
        }

        private void ShowErrorMessage(string messageText)
        {
            labelAddChildError.Text = messageText;
            labelAddChildError.Visible = true;
        }

        private void HideErrorMessage()
        {
            labelAddChildError.Visible = false;
        }
    }
}
