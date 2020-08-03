using ASKON_TestTask.Persistence;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ASKON_TestTask
{
    public partial class RenameDetailForm : Form
    {
        private readonly TestTaskContext _context;
        private readonly int _maxNameDetailLength = 255;

        public string EnteredName { get; set; }

        public RenameDetailForm(TestTaskContext context)
        {
            _context = context;

            InitializeComponent();

            tbNewDetailName.Select();
        }

        private void btnRenameDetail_Click(object sender, EventArgs e)
        {
            EnteredName = tbNewDetailName.Text;

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

        private void tbNewDetailName_Enter(object sender, EventArgs e)
        {
            HideErrorMessage();
        }

        private void ShowErrorMessage(string messageText)
        {
            labelRenameDetailError.Text = messageText;
            labelRenameDetailError.Visible = true;
        }

        private void HideErrorMessage()
        {
            labelRenameDetailError.Visible = false;
        }
    }
}
