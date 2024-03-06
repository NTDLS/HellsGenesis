using System.Windows.Forms;

namespace Si.Client
{
    public partial class FormViewBrain : Form
    {
        public FormViewBrain()
        {
            InitializeComponent();
        }
        public FormViewBrain(string text)
        {
            InitializeComponent();

            textBoxText.Text = text;
        }
    }
}
