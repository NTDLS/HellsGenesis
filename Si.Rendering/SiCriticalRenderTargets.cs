using SharpDX.Direct2D1;

namespace Si.Rendering
{
    public class SiCriticalRenderTargets
    {
        public BitmapRenderTarget? IntermediateRenderTarget { get; set; }
        public WindowRenderTarget? ScreenRenderTarget { get; set; }
    }
}
