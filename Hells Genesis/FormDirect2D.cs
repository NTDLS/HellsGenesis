using HG.Engine;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
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

            _core = new Core(this);

            this.Shown += (object sender, EventArgs e) =>
            {
                //_core.Start();
            };

            FormClosed += (sender, e) =>
            {
                //_core.Stop();
            };

            this.MouseWheel += FormDirect2D_MouseWheel;

        }

        private void FormDirect2D_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                scaleFactor += 0.1f;
            }
            else if (e.Delta < 0)
            {
                scaleFactor -= 0.1f;
            }
        }

        float angle = 0;
        float scaleFactor = 1;

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
                    _core.D2DX.ScreenRenderTarget.BeginDraw();

                    //_core.Actors.Render();
                    //e.Graphics.DrawImage(_core.Actors.Render(), 0, 0);

                    _core.D2DX.ScreenRenderTarget.Clear(_core.D2DX.RawColorGray);

                    var intermediateSize = new Size2F(_core.Display.TotalCanvasSize.Width, _core.Display.TotalCanvasSize.Height);

                    using (var intermediateRenderTarget = new BitmapRenderTarget(_core.D2DX.ScreenRenderTarget, CompatibleRenderTargetOptions.None, intermediateSize))
                    {
                        float x = _core.Display.NatrualScreenSize.Width / 2;
                        float y = _core.Display.NatrualScreenSize.Height / 2;

                        var bitmap = _core.D2DX.GetCachedBitmap("c:\\test.bmp");
                        var bitmapRect = _core.D2DX.DrawBitmapAt(_core.D2DX.ScreenRenderTarget, bitmap, x, y, angle);

                        _core.D2DX.DrawRectangleAt(_core.D2DX.ScreenRenderTarget, bitmapRect, angle, _core.D2DX.RawColorRed, 2, 1);

                        var textLocation = _core.D2DX.DrawTextAt(_core.D2DX.ScreenRenderTarget, x, y, -angle, "Hello from the GPU!", _core.D2DX.LargeTextFormat, _core.D2DX.SolidColorBrushRed);
                        _core.D2DX.DrawRectangleAt(_core.D2DX.ScreenRenderTarget, textLocation, -angle, _core.D2DX.RawColorGreen);

                        _core.D2DX.ScreenRenderTarget.EndDraw();
                    }
                }
            }
            catch
            {
            }
        }
    }
}
