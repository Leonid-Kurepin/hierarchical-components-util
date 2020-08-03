using System.Windows.Forms;

namespace ASKON_TestTask.Forms
{
    public partial class MessageForm : Form
    {
        public MessageForm(string messageText)
        {
            InitializeComponent();

            labelSuccessMessage.Text = messageText;
        }
    }
}
