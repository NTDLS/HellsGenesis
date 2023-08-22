using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AI2D.Engine.Managers
{
    public class EngineDrawingCacheManager
    {
        public enum DrawingCacheType
        {
            Scaling,
            Screen,
            Radar
        }

        private readonly Dictionary<DrawingCacheType, DrawingCacheItem> _graphicsCache = new();

        public class DrawingCacheItem
        {
            public Graphics Graphics { get; private set; }
            public Bitmap Bitmap { get; private set; }

            public DrawingCacheItem(Size size)
            {
                Bitmap = new Bitmap(size.Width, size.Height);
                Graphics = Graphics.FromImage(Bitmap);

                Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Graphics.CompositingMode = CompositingMode.SourceOver;
                Graphics.CompositingQuality = CompositingQuality.HighQuality;
                Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            }
        }

        public bool Exists(DrawingCacheType key)
        {
            return _graphicsCache.ContainsKey(key);
        }

        public DrawingCacheItem Get(DrawingCacheType key, Size size)
        {
            if (_graphicsCache.TryGetValue(key, out var item))
            {
                if (item.Bitmap.Width != size.Width || item.Bitmap.Height != size.Height)
                {
                    throw new System.Exception("Graphics cache bitmap size can not be changed.");
                }
                return item;
            }
            else
            {
                var newInstance = new DrawingCacheItem(size);
                _graphicsCache.Add(key, newInstance);
                return newInstance;
            }
        }
        public DrawingCacheItem Create(DrawingCacheType key, Size size)
        {
            if (_graphicsCache.ContainsKey(key))
            {
                throw new System.Exception("Graphics cache item already exists and can not be recreated.");
            }
            var newInstance = new DrawingCacheItem(size);
            _graphicsCache.Add(key, newInstance);
            return newInstance;
        }

        public DrawingCacheItem Get(DrawingCacheType key)
        {
            if (_graphicsCache.TryGetValue(key, out var item))
            {
                return item;
            }
            else
            {
                throw new System.Exception("Graphics cache item does not exist and not be created without a size.");
            }
        }
    }
}
