using System.Drawing;

namespace HG.Engine.Controllers
{
    internal class EngineImageController
    {
        private readonly Core _core;

        public EngineImageController(Core core)
        {
            _core = core;
        }

        public Bitmap Get(string path)
        {
            return _core.Assets.GetBitmap(path);
        }
    }
}
