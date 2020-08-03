using System.Windows.Forms;

namespace ASKON_TestTask
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
