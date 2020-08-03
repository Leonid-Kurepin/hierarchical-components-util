using ASKON_TestTask.Persistence;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ASKON_TestTask
{
    public partial class AddParentForm : Form
    {
        private readonly TestTaskContext _context;
        private readonly int _maxNameDetailLength = 255;

        public string EnteredName { get; set; }

        public AddParentForm(TestTaskContext context)
        {
            _context = context;

            InitializeComponent();

            tbAddParentName.Select();
        }

        private void btnAddParent_Click(object sender, EventArgs e)
        {
            EnteredName = tbAddParentName.Text;

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

            var isUniqueName = !_context.Details.Any(x => x.Name == detailName);

            if (!isUniqueName)
            {
                ShowErrorMessage("Not unique!");

                return false;
            }

            return true;
        }

        private void tbAddParentName_Enter(object sender, EventArgs e)
        {
            HideErrorMessage();
        }

        private void ShowErrorMessage(string messageText)
        {
            labelAddParentError.Text = messageText;
            labelAddParentError.Visible = true;
        }

        private void HideErrorMessage()
        {
            labelAddParentError.Visible = false;
        }
    }
}
