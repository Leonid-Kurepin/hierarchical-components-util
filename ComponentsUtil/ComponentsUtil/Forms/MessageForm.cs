using System.Windows.Forms;

namespace ComponentsUtil.Forms
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
