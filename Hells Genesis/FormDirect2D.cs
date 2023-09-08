using HG.Engine;
using HG.Utility.ExtensionMethods;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
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

            interSize = new Size2F(_core.Display.TotalCanvasSize.Width, _core.Display.TotalCanvasSize.Height);
            intermediateRenderTarget = new BitmapRenderTarget(_core.D2DX.ScreenRenderTarget, CompatibleRenderTargetOptions.None, interSize);

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

        SharpDX.Size2F interSize;
        BitmapRenderTarget intermediateRenderTarget;

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);

            try
            {
                lock (this)
                {
                    _core.D2DX.ScreenRenderTarget.BeginDraw();

                    intermediateRenderTarget.BeginDraw();


                    //_core.Actors.Render();
                    //e.Graphics.DrawImage(_core.Actors.Render(), 0, 0);

                    _core.D2DX.ScreenRenderTarget.Clear(_core.D2DX.RawColorGray);

                    intermediateRenderTarget.Clear(_core.D2DX.RawColorGray);

                    float x = _core.Display.TotalCanvasSize.Width / 2;
                    float y = _core.Display.TotalCanvasSize.Height / 2;

                    var bitmap = _core.D2DX.GetCachedBitmap("c:\\0.png", 32, 32);
                    var bitmapRect = _core.D2DX.DrawBitmapAt(intermediateRenderTarget, bitmap, x, y, angle);

                    //_core.D2DX.DrawRectangleAt(intermediateRenderTarget, bitmapRect, angle, _core.D2DX.RawColorRed, 2, 1);

                    //var textLocation = _core.D2DX.DrawTextAt(intermediateRenderTarget, x, y, -angle, "Hello from the GPU!", _core.D2DX.LargeTextFormat, _core.D2DX.SolidColorBrushRed);
                    //_core.D2DX.DrawRectangleAt(intermediateRenderTarget, textLocation, -angle, _core.D2DX.RawColorGreen);

                    intermediateRenderTarget.EndDraw();

                    scaleFactor = scaleFactor.Box(0, 100);

                    float widthScale = _core.Display.OverdrawSize.Width * (scaleFactor / 100.0f);
                    float heightScale = _core.Display.OverdrawSize.Height * (scaleFactor / 100.0f);

                    // Define the source rectangle to crop from intermediateRenderTarget (assuming you want to crop from the center)
                    RawRectangleF sourceRect = new RawRectangleF(widthScale, heightScale,
                        intermediateRenderTarget.Size.Width - widthScale, intermediateRenderTarget.Size.Height - heightScale);

                    var destRect = new RawRectangleF(0, 0, _core.Display.NatrualScreenSize.Width, _core.Display.NatrualScreenSize.Height);
                    _core.D2DX.ScreenRenderTarget.DrawBitmap(intermediateRenderTarget.Bitmap, destRect, 1.0f, BitmapInterpolationMode.Linear, sourceRect);

                    _core.D2DX.ScreenRenderTarget.EndDraw();
                }
            }
            catch
            {
            }
        }
    }
}
