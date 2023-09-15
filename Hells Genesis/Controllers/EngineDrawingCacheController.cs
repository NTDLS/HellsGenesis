using HG.Engine;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace HG.Controllers
{
    internal class EngineDrawingCacheController
    {
        private readonly Dictionary<HgDrawingCacheType, DrawingCacheItem> _graphicsCache = new();
        private readonly EngineCore _core;

        public EngineDrawingCacheController(EngineCore core)
        {
            _core = core;
        }

        internal class DrawingCacheItem
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

        public bool Exists(HgDrawingCacheType key)
        {
            return _graphicsCache.ContainsKey(key);
        }

        public DrawingCacheItem Get(HgDrawingCacheType key, Size size)
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
                newInstance.Graphics.InterpolationMode = _core.Settings.GraphicsScalingMode;
                _graphicsCache.Add(key, newInstance);
                return newInstance;
            }
        }
        public DrawingCacheItem Create(HgDrawingCacheType key, Size size)
        {
            if (_graphicsCache.ContainsKey(key))
            {
                throw new System.Exception("Graphics cache item already exists and can not be recreated.");
            }
            var newInstance = new DrawingCacheItem(size);
            newInstance.Graphics.InterpolationMode = _core.Settings.GraphicsScalingMode;
            _graphicsCache.Add(key, newInstance);
            return newInstance;
        }

        public DrawingCacheItem Get(HgDrawingCacheType key)
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
