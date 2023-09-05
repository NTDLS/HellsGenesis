using System.Drawing;

namespace HG.Engine.Managers
{
    internal class EngineImageManager
    {
        private readonly Core _core;

        public EngineImageManager(Core core)
        {
            _core = core;
        }

        public Bitmap Get(string path)
        {
            return _core.Assets.GetBitmap(path);
        }
    }
}
