using System;
using System.Windows.Forms;

namespace HG
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            checkBoxFullscreen.CheckedChanged += checkBoxFullscreen_CheckedChanged;
            trackBarResolution.Scroll += trackBarResolution_Scroll;

            trackBarResolution.Minimum = 2;
            trackBarResolution.Maximum = Math.Max(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height) / 100;
            trackBarResolution.Value = trackBarResolution.Maximum;

            trackBarResolution_Scroll(this, new EventArgs());
        }

        private void checkBoxFullscreen_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxFullscreen.Checked)
            {
                labelResolution.Text = $"{Screen.PrimaryScreen.Bounds.Width} x {Screen.PrimaryScreen.Bounds.Height}";
            }
            else
            {
                trackBarResolution_Scroll(this, new EventArgs());
            }

            trackBarResolution.Enabled = !checkBoxFullscreen.Checked;
        }

        private void trackBarResolution_Scroll(object sender, EventArgs e)
        {
            int baseX = Screen.PrimaryScreen.Bounds.Width - (int)(Screen.PrimaryScreen.Bounds.Width * (1.0 / trackBarResolution.Value));
            int baseY = Screen.PrimaryScreen.Bounds.Height - (int)(Screen.PrimaryScreen.Bounds.Height * (1.0 / trackBarResolution.Value));

            if ((baseX % 2) != 0) baseX++;
            if ((baseY % 2) != 0) baseY++;

            labelResolution.Text = $"{baseX} x {baseY}";
        }

        private void tabPageDisplay_Click(object sender, EventArgs e)
        {

        }

        private void textBoxOverdrawScale_TextChanged(object sender, EventArgs e)
        {
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void textBoxFrameLimiter_TextChanged(object sender, EventArgs e)
        {
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void labelOverdrawScale_Click(object sender, EventArgs e)
        {
        }
    }
}
