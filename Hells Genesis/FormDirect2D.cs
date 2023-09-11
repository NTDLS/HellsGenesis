using HG.Engine;
using HG.Types;
using System;
using System.Windows.Forms;

namespace HG
{
    public partial class FormDirect2D : Form
    {
        private readonly Core _core;
        private readonly bool _fullScreen = false;

        public FormDirect2D()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);

            if (_fullScreen)
            {
                FormBorderStyle = FormBorderStyle.None;
                Width = Screen.PrimaryScreen.Bounds.Width;
                Height = Screen.PrimaryScreen.Bounds.Height;
                ShowInTaskbar = true;
                //TopMost = true;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                ClientSize = new System.Drawing.Size((int)(Screen.PrimaryScreen.Bounds.Width * 0.75), (int)(Screen.PrimaryScreen.Bounds.Height * 0.75));
                StartPosition = FormStartPosition.CenterScreen;
            }

            _core = new Core(this);

            var timer = new Timer()
            {
                Enabled = true,
                Interval = 1
            };

            timer.Tick += (object sender, EventArgs e) =>
            {
                Invalidate();
            };

            Shown += (object sender, EventArgs e) =>
            {
                _core.Start();
            };

            FormClosed += (sender, e) =>
            {
                _core.Stop();
            };

            KeyDown += FormMain_KeyDown;
            KeyUp += FormMain_KeyUp;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left) _core.Input.KeyStateChanged(HgPlayerKey.Left, HgKeyPressState.Down);
            else if (keyData == Keys.Right) _core.Input.KeyStateChanged(HgPlayerKey.Right, HgKeyPressState.Down);
            else if (keyData == Keys.Up) _core.Input.KeyStateChanged(HgPlayerKey.Up, HgKeyPressState.Down);
            else if (keyData == Keys.Down) _core.Input.KeyStateChanged(HgPlayerKey.Down, HgKeyPressState.Down);
            else return base.ProcessCmdKey(ref msg, keyData);

            _core.Input.HandleSingleKeyPress(keyData);

            return true; // Mark the key as handled
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey) _core.Input.KeyStateChanged(HgPlayerKey.SpeedBoost, HgKeyPressState.Down);
            if (e.KeyCode == Keys.W) _core.Input.KeyStateChanged(HgPlayerKey.Forward, HgKeyPressState.Down);
            if (e.KeyCode == Keys.A) _core.Input.KeyStateChanged(HgPlayerKey.RotateCounterClockwise, HgKeyPressState.Down);
            if (e.KeyCode == Keys.S) _core.Input.KeyStateChanged(HgPlayerKey.Reverse, HgKeyPressState.Down);
            if (e.KeyCode == Keys.D) _core.Input.KeyStateChanged(HgPlayerKey.RotateClockwise, HgKeyPressState.Down);
            if (e.KeyCode == Keys.Space) _core.Input.KeyStateChanged(HgPlayerKey.PrimaryFire, HgKeyPressState.Down);
            if (e.KeyCode == Keys.ControlKey) _core.Input.KeyStateChanged(HgPlayerKey.SecondaryFire, HgKeyPressState.Down);
            if (e.KeyCode == Keys.Escape) _core.Input.KeyStateChanged(HgPlayerKey.Escape, HgKeyPressState.Down);
            if (e.KeyCode == Keys.Left) _core.Input.KeyStateChanged(HgPlayerKey.Left, HgKeyPressState.Down);
            if (e.KeyCode == Keys.Right) _core.Input.KeyStateChanged(HgPlayerKey.Right, HgKeyPressState.Down);
            if (e.KeyCode == Keys.Up) _core.Input.KeyStateChanged(HgPlayerKey.Up, HgKeyPressState.Down);
            if (e.KeyCode == Keys.Down) _core.Input.KeyStateChanged(HgPlayerKey.Down, HgKeyPressState.Down);
            if (e.KeyCode == Keys.Enter) _core.Input.KeyStateChanged(HgPlayerKey.Enter, HgKeyPressState.Down);

            _core.Input.HandleSingleKeyPress(e.KeyCode);
        }

        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _core.Pause();

                if (MessageBox.Show("Are you sure you want to quit?", "Afraid to go on?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    Close();
                }
                else
                {
                    _core.Resume();
                }
            }

            if (e.KeyCode == Keys.ShiftKey) _core.Input.KeyStateChanged(HgPlayerKey.SpeedBoost, HgKeyPressState.Up);
            if (e.KeyCode == Keys.W) _core.Input.KeyStateChanged(HgPlayerKey.Forward, HgKeyPressState.Up);
            if (e.KeyCode == Keys.A) _core.Input.KeyStateChanged(HgPlayerKey.RotateCounterClockwise, HgKeyPressState.Up);
            if (e.KeyCode == Keys.S) _core.Input.KeyStateChanged(HgPlayerKey.Reverse, HgKeyPressState.Up);
            if (e.KeyCode == Keys.D) _core.Input.KeyStateChanged(HgPlayerKey.RotateClockwise, HgKeyPressState.Up);
            if (e.KeyCode == Keys.Space) _core.Input.KeyStateChanged(HgPlayerKey.PrimaryFire, HgKeyPressState.Up);
            if (e.KeyCode == Keys.ControlKey) _core.Input.KeyStateChanged(HgPlayerKey.SecondaryFire, HgKeyPressState.Up);
            if (e.KeyCode == Keys.Escape) _core.Input.KeyStateChanged(HgPlayerKey.Escape, HgKeyPressState.Up);
            if (e.KeyCode == Keys.Left) _core.Input.KeyStateChanged(HgPlayerKey.Left, HgKeyPressState.Up);
            if (e.KeyCode == Keys.Right) _core.Input.KeyStateChanged(HgPlayerKey.Right, HgKeyPressState.Up);
            if (e.KeyCode == Keys.Up) _core.Input.KeyStateChanged(HgPlayerKey.Up, HgKeyPressState.Up);
            if (e.KeyCode == Keys.Down) _core.Input.KeyStateChanged(HgPlayerKey.Down, HgKeyPressState.Up);
            if (e.KeyCode == Keys.Enter) _core.Input.KeyStateChanged(HgPlayerKey.Enter, HgKeyPressState.Up);
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Prevent background painting to avoid flickering
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Prevent painting to avoid flickering.
        }
    }
}
