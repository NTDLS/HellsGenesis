using HG.Engine;
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
                Width = (int)(Screen.PrimaryScreen.Bounds.Width * 0.75);
                Height = (int)(Screen.PrimaryScreen.Bounds.Height * 0.75);
            }

            var timer = new Timer()
            {
                Enabled = true,
                Interval = 1
            };

            timer.Tick += (object sender, EventArgs e) =>
            {
                angle += 0.01f;
                //Invalidate();
            };

            _core = new Core(this);

            this.Shown += (object sender, EventArgs e) =>
            {
                _core.Start();
            };

            FormClosed += (sender, e) =>
            {
                _core.Stop();
            };
        }


        float angle = 0.45f;

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Prevent background painting to avoid flickering
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);

            try
            {
                lock (this)
                {
                    _core.D2DX.RenderTarget.BeginDraw();

                    _core.Actors.Render();

                    //e.Graphics.DrawImage(_core.Actors.Render(), 0, 0);

                    _core.D2DX.RenderTarget.Clear(_core.D2DX.RawColorGray);

                    float x = 200;
                    float y = 200;

                    var bitmap = _core.D2DX.GetCachedBitmap("c:\\test.bmp");
                    var bitmapRect = _core.D2DX.DrawBitmapAt(bitmap, x, y, angle);

                    _core.D2DX.DrawRectangleAt(bitmapRect, angle, _core.D2DX.RawColorRed, 2, 1);

                    var textLocation = _core.D2DX.DrawTextAt(400, 400, -angle, "Hello from the GPU!", _core.D2DX.LargeTextFormat, _core.D2DX.SolidColorBrushRed);
                    _core.D2DX.DrawRectangleAt(textLocation, -angle, _core.D2DX.RawColorGreen);

                    _core.D2DX.RenderTarget.EndDraw();
                }
            }
            catch
            {
            }
        }
    }
}
