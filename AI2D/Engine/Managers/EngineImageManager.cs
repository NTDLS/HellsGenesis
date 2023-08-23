using System.Collections.Generic;
using System.Drawing;

namespace AI2D.Engine.Managers
{
    internal class EngineImageManager
    {
        private Dictionary<string, Bitmap> _collection { get; set; } = new();

        private readonly Core _core;

        public EngineImageManager(Core core)
        {
            _core = core;

        }

        public Bitmap Get(string path)
        {
            Bitmap result = null;

            path = path.ToLower();

            lock (_collection)
            {
                if (_collection.ContainsKey(path))
                {
                    result = _collection[path].Clone() as Bitmap;
                }
                else
                {
                    using (var image = Image.FromFile(path))
                    using (var newbitmap = new Bitmap(image))
                    {
                        result = newbitmap.Clone() as Bitmap;
                        _collection.Add(path, result);
                    }
                }
            }

            return result;
        }
    }
}
