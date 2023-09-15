using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace HG
{
    public partial class FormStartup : Form
    {
        public FormStartup()
        {
            InitializeComponent();

            buttonExit.Visible = false;
            buttonSettings.Visible = false;
            buttonStart.Visible = false;
        }

        private void FormStartup_Load(object sender, EventArgs e)
        {
            AcceptButton = buttonStart;
            CancelButton = buttonExit;

            buttonStart.Focus();

            TopMost = false;
            Width = BackgroundImage.Width;
            Height = BackgroundImage.Height;
            StartPosition = FormStartPosition.CenterScreen;
            Opacity = 0;
            TransparencyKey = Color.FromArgb(1, 1, 1);
            BackColor = Color.FromArgb(1, 1, 1);


            Shown += FormStartup_Shown;

            var timer = new Timer()
            {
                Enabled = true,
                Interval = 1,
            };

            timer.Tick += (object sender, EventArgs e) =>
            {
                Opacity += 0.05;
                if (Opacity >= 1)
                {
                    timer.Stop();

                    buttonExit.Visible = true;
                    buttonSettings.Visible = true;
                    buttonStart.Visible = true;
                }
            };

            timer.Start();
        }

        private void FormStartup_Shown(object sender, EventArgs e)
        {

            try
            {
                using (var player = new SoundPlayer(@"..\..\..\Assets\Splash.wav"))
                {
                    player.Play();
                }
            }
            catch { }
        }


        private void buttonExit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            using (var form = new FormSettings())
            {
                form.ShowDialog();
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }
    }
}
