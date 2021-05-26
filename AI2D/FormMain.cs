using AI2D.Engine;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AI2D
{
    public partial class FormMain : Form
    {
        private Core _core;
        private bool _fullScreen = false;

        //This really shouldn't be necessary! :(
        protected override CreateParams CreateParams
        {
            get
            {
                //Paints all descendants of a window in bottom-to-top painting order using double-buffering.
                // For more information, see Remarks. This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC. 
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000; //WS_EX_COMPOSITED       
                return handleParam;
            }
        }

        public FormMain()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            if (_fullScreen)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.Width = Screen.PrimaryScreen.Bounds.Width;
                this.Height = Screen.PrimaryScreen.Bounds.Height;
                this.ShowInTaskbar = true;
                //this.TopMost = true;
                this.WindowState = FormWindowState.Maximized;
            }

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);

            this.BackColor = Color.FromArgb(1, 1, 10);

            _core = new Core(this, new Size(this.Width, this.Height));

            _core.OnStop += _core_OnStop;
        }

        private void _core_OnStop(Core sender)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.Close();
            });
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            _core.Start();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _core.Stop();
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey) _core.Input.KeyStateChanged(Types.PlayerKey.SpeedBoost, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.W) _core.Input.KeyStateChanged(Types.PlayerKey.Forward, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.A) _core.Input.KeyStateChanged(Types.PlayerKey.RotateCounterClockwise, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.S) _core.Input.KeyStateChanged(Types.PlayerKey.Reverse, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.D) _core.Input.KeyStateChanged(Types.PlayerKey.RotateClockwise, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.Space) _core.Input.KeyStateChanged(Types.PlayerKey.PrimaryFire, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.ControlKey) _core.Input.KeyStateChanged(Types.PlayerKey.SecondaryFire, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.Escape) _core.Input.KeyStateChanged(Types.PlayerKey.Escape, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.Left) _core.Input.KeyStateChanged(Types.PlayerKey.Left, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.Right) _core.Input.KeyStateChanged(Types.PlayerKey.Right, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.Up) _core.Input.KeyStateChanged(Types.PlayerKey.Up, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.Down) _core.Input.KeyStateChanged(Types.PlayerKey.Down, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.Enter) _core.Input.KeyStateChanged(Types.PlayerKey.Enter, Types.KeyPressState.Down);

            _core.Input.HandleSingleKeyPress(e.KeyCode);
        }

        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                _core.Pause();

                if (MessageBox.Show("Are you sure you want to quit?", "Afraid to go on?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    this.Close();
                }
                else
                {
                    _core.Resume();
                }
            }

            if (e.KeyCode == Keys.ShiftKey) _core.Input.KeyStateChanged(Types.PlayerKey.SpeedBoost, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.W) _core.Input.KeyStateChanged(Types.PlayerKey.Forward, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.A) _core.Input.KeyStateChanged(Types.PlayerKey.RotateCounterClockwise, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.S) _core.Input.KeyStateChanged(Types.PlayerKey.Reverse, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.D) _core.Input.KeyStateChanged(Types.PlayerKey.RotateClockwise, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.Space) _core.Input.KeyStateChanged(Types.PlayerKey.PrimaryFire, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.ControlKey) _core.Input.KeyStateChanged(Types.PlayerKey.SecondaryFire, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.Escape) _core.Input.KeyStateChanged(Types.PlayerKey.Escape, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.Left) _core.Input.KeyStateChanged(Types.PlayerKey.Left, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.Right) _core.Input.KeyStateChanged(Types.PlayerKey.Right, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.Up) _core.Input.KeyStateChanged(Types.PlayerKey.Up, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.Down) _core.Input.KeyStateChanged(Types.PlayerKey.Down, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.Enter) _core.Input.KeyStateChanged(Types.PlayerKey.Enter, Types.KeyPressState.Up);
        }

        private void FormMain_MouseEnter(object sender, EventArgs e)
        {
            if (_fullScreen)
            {
                Cursor.Hide();
            }
        }

        private void FormMain_MouseLeave(object sender, EventArgs e)
        {
            if (_fullScreen)
            {
                Cursor.Show();
            }
        }

        private void FormMain_Paint(object sender, PaintEventArgs e)
        {
            /*
            Bitmap frame = _core.Actors.GetLatestFrame();
            if (frame != null)
            {
                e.Graphics.DrawImage(frame, 0, 0);
                frame.Dispose();
                frame = null;
            }
            */

            e.Graphics.DrawImage(_core.Actors.Render(), 0, 0);
        }
    }
}
