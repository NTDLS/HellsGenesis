using System.Windows.Forms;

namespace NebulaSiege.Game
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
