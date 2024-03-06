using System;
using System.Drawing;
using System.Windows.Forms;

namespace Si.Client
{
    public partial class FormStartup : Form
    {
        public FormStartup()
        {
            InitializeComponent();
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
            TransparencyKey = Color.FromArgb(12, 10, 12);
            BackColor = TransparencyKey;

            buttonExit.Top = Height - (buttonExit.Height + 25);
            buttonSettings.Top = Height - (buttonSettings.Height + 25);
            buttonStart.Top = Height - (buttonStart.Height + 25);

            buttonSettings.Left = (Width / 2) - (buttonSettings.Width / 2);
            buttonExit.Left = buttonSettings.Left - (buttonExit.Width + 25);
            buttonStart.Left = buttonSettings.Left + (buttonStart.Width + 25);

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
                }
            };

            timer.Start();
        }

        private void FormStartup_Shown(object sender, EventArgs e)
        {

            try
            {
                /*
                using (var player = new SoundPlayer(@"..\..\..\Assets\Splash.wav"))
                {
                    player.Play();
                }
                */
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
