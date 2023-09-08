using System;
using System.Drawing;
using System.Windows.Forms;

namespace HG
{
    public partial class FormDirect2D : Form
    {
        HgDirectX hgDx;

        public FormDirect2D()
        {
            InitializeComponent();
        }

        private void FormDirect2D_Load(object sender, EventArgs e)
        {
            this.ClientSize = new Size(800, 600);
            this.Text = "SharpDX 2D Drawing";

            FormClosing += (sender, e) =>
            {
                //renderTarget?.Dispose();
            };

            InitializeDirectX();

            hgDx = new HgDirectX(this);
        }

        private void InitializeDirectX()
        {
            this.ClientSize = new Size(800, 600);

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);

            var timer = new Timer()
            {
                Enabled = true,
                Interval = 1
            };

            timer.Tick += (object sender, EventArgs e) =>
            {
                angle += 0.01f;
                Invalidate();
            };
        }

        float angle = 0.45f;

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Prevent background painting to avoid flickering
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            hgDx.RenderTarget.BeginDraw();

            hgDx.RenderTarget.Clear(hgDx.RawColorLightGray);

            float x = 200;
            float y = 200;

            var bitmap = hgDx.GetCachedBitmap("c:\\test.bmp");
            var bitmapRect = hgDx.DrawBitmapAt(bitmap, x, y, angle);

            hgDx.DrawRectangleAt(bitmapRect, angle, hgDx.RawColorRed, 2, 1);


            var textLocation = hgDx.DrawTextAt(400, 400, -angle, "Hello from the GPU!", hgDx.LargeTextFormat, hgDx.SolidColorBrushRed);
            hgDx.DrawRectangleAt(textLocation, -angle, hgDx.RawColorGreen);

            hgDx.RenderTarget.EndDraw();
        }
    }
}
