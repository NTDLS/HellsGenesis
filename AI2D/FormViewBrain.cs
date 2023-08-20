using System.Windows.Forms;

namespace AI2D
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
