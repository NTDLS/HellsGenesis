using HG.Engine;
using HG.Types;
using System;
using System.Diagnostics;
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
                //angle += 0.1f;
                Invalidate();
            };

            Shown += (object sender, EventArgs e) =>
            {
                _core.Start();
            };

            FormClosed += (sender, e) =>
            {
                //_core.Stop();
            };

            this.KeyDown += FormMain_KeyDown;
            this.KeyUp += FormMain_KeyUp;


            MouseWheel += FormDirect2D_MouseWheel;
        }

        private void FormDirect2D_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                scaleFactor += 0.5f;
            }
            else if (e.Delta < 0)
            {
                scaleFactor -= 0.5f;
            }

            Debug.Print($"{scaleFactor:n1}");
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left) _core.Input.KeyStateChanged(HgPlayerKey.Left, HgKeyPressState.Down);
            else if (keyData == Keys.Right) _core.Input.KeyStateChanged(HgPlayerKey.Right, HgKeyPressState.Down);
            else if (keyData == Keys.Up) _core.Input.KeyStateChanged(HgPlayerKey.Up, HgKeyPressState.Down);
            else if (keyData == Keys.Down) _core.Input.KeyStateChanged(HgPlayerKey.Down, HgKeyPressState.Down);
            else return base.ProcessCmdKey(ref msg, keyData);

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

        float angle = 0;
        float scaleFactor = 50;

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                lock (this)
                {
                    _core.DirectX.ScreenRenderTarget.BeginDraw();
                    _core.DirectX.IntermediateRenderTarget.BeginDraw();

                    _core.DirectX.ScreenRenderTarget.Clear(_core.DirectX.RawColorBlack);
                    _core.DirectX.IntermediateRenderTarget.Clear(_core.DirectX.RawColorBlack);

                    _core.Actors.RenderPreScaling(_core.DirectX.IntermediateRenderTarget);

                    _core.DirectX.IntermediateRenderTarget.EndDraw();

                    _core.DirectX.ApplyScaling(scaleFactor);
                    _core.Actors.RenderPostScaling(_core.DirectX.ScreenRenderTarget);

                    /*
                    float x = _core.Display.NatrualScreenSize.Width / 2;
                    float y = _core.Display.NatrualScreenSize.Height / 2;

                    var bitmap = _core.DirectX.GetCachedBitmap("c:\\0.png", 32, 32);
                    var bitmapRect = _core.DirectX.DrawBitmapAt(_core.DirectX.ScreenRenderTarget, bitmap, x, y, angle);

                    _core.DirectX.DrawRectangleAt(_core.DirectX.ScreenRenderTarget, bitmapRect, angle, _core.DirectX.RawColorRed, 2, 1);
                    var textLocation = _core.DirectX.DrawTextAt(_core.DirectX.ScreenRenderTarget, x, y, -angle, "Hello from the GPU!", _core.DirectX.LargeTextFormat, _core.DirectX.SolidColorBrushRed);
                    _core.DirectX.DrawRectangleAt(_core.DirectX.ScreenRenderTarget, textLocation, -angle, _core.DirectX.RawColorGreen);
                    */

                    _core.DirectX.ScreenRenderTarget.EndDraw();
                }
            }
            catch
            {
            }
        }
    }
}
