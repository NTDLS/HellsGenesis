using HG.Engine;

namespace HG.Controllers
{
    internal class EngineImageController
    {
        private readonly EngineCore _core;

        public EngineImageController(EngineCore core)
        {
            _core = core;
        }

        public SharpDX.Direct2D1.Bitmap Get(string path)
        {
            return _core.Assets.GetBitmap(path);
        }

        public SharpDX.Direct2D1.Bitmap Get(string path, int newWidth, int newHeight)
        {
            return _core.Assets.GetBitmap(path, newWidth, newHeight);
        }
    }
}
