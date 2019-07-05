using AI2D.Engine;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AI2D
{
    public partial class FormMain : Form
    {
        private Game _game;
        private bool _fullScreen = false;

        protected override CreateParams CreateParams
        {
            get
            {
                /*
                    Paints all descendants of a window in bottom-to-top painting order using double-buffering.
                    For more information, see Remarks. This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC. 
                */
                CreateParams handleParam = base.CreateParams;
                handleParam.ExStyle |= 0x02000000; //WS_EX_COMPOSITED       
                return handleParam;
            }
        }

        public FormMain()
        {
            InitializeComponent();

            if (_fullScreen)
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.Width = Screen.PrimaryScreen.Bounds.Width;
                this.Height = Screen.PrimaryScreen.Bounds.Height;
                this.ShowInTaskbar = true;
                this.TopMost = true;
                this.WindowState = FormWindowState.Maximized;
            }

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            _game = new Game(pictureBoxScene, new Size(this.Width, this.Height));
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            _game.Start();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _game.Stop();
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) _game.Input.KeyStateChanged(Types.PlayerKey.Forward, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.A) _game.Input.KeyStateChanged(Types.PlayerKey.RotateCounterClockwise, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.S) _game.Input.KeyStateChanged(Types.PlayerKey.Reverse, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.D) _game.Input.KeyStateChanged(Types.PlayerKey.RotateClockwise, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.Space) _game.Input.KeyStateChanged(Types.PlayerKey.Fire, Types.KeyPressState.Down);
            if (e.KeyCode == Keys.Escape) _game.Input.KeyStateChanged(Types.PlayerKey.Escape, Types.KeyPressState.Down);

            _game.Input.DebugKeyPress(e.KeyCode);
        }

        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W) _game.Input.KeyStateChanged(Types.PlayerKey.Forward, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.A) _game.Input.KeyStateChanged(Types.PlayerKey.RotateCounterClockwise, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.S) _game.Input.KeyStateChanged(Types.PlayerKey.Reverse, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.D) _game.Input.KeyStateChanged(Types.PlayerKey.RotateClockwise, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.Space) _game.Input.KeyStateChanged(Types.PlayerKey.Fire, Types.KeyPressState.Up);
            if (e.KeyCode == Keys.Escape) _game.Input.KeyStateChanged(Types.PlayerKey.Escape, Types.KeyPressState.Up);
        }

        private void PictureBoxScene_Paint(object sender, PaintEventArgs e)
        {
            /*
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            */

            _game.Actors.Render(e.Graphics);
            base.OnPaint(e);
        }

        private void PictureBoxScene_MouseEnter(object sender, EventArgs e)
        {
            Cursor.Hide();
        }

        private void PictureBoxScene_MouseLeave(object sender, EventArgs e)
        {
            Cursor.Show();
        }
    }
}
