namespace HG.Engine.Controllers
{
    internal class EngineImageController
    {
        private readonly Core _core;

        public EngineImageController(Core core)
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
