using HG.Engine;
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
                angle += 0.1f;
                Invalidate();
            };

            Shown += (object sender, EventArgs e) =>
            {
                //_core.Start();
            };

            FormClosed += (sender, e) =>
            {
                //_core.Stop();
            };

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

        float angle = 0;
        float scaleFactor = 50;

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Prevent background painting to avoid flickering
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            try
            {
                lock (this)
                {
                    _core.DirectX.StartDraw();

                    //_core.Actors.Render();
                    //e.Graphics.DrawImage(_core.Actors.Render(), 0, 0);

                    float x = _core.Display.TotalCanvasSize.Width / 2;
                    float y = _core.Display.TotalCanvasSize.Height / 2;

                    var bitmap = _core.DirectX.GetCachedBitmap("c:\\0.png", 32, 32);
                    var bitmapRect = _core.DirectX.DrawBitmapAt(bitmap, x, y, angle);

                    _core.DirectX.DrawRectangleAt(bitmapRect, angle, _core.DirectX.RawColorRed, 2, 1);
                    var textLocation = _core.DirectX.DrawTextAt(x, y, -angle, "Hello from the GPU!", _core.DirectX.LargeTextFormat, _core.DirectX.SolidColorBrushRed);
                    _core.DirectX.DrawRectangleAt(textLocation, -angle, _core.DirectX.RawColorGreen);

                    _core.DirectX.EndDraw(scaleFactor);
                }
            }
            catch
            {
            }
        }
    }
}
