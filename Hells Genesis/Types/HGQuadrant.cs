using System.Drawing;

namespace HG.Engine
{
    internal class HGQuadrant
    {
        public Point Key { get; private set; }
        public Rectangle Bounds { get; private set; }

        public HGQuadrant(Point key, Rectangle bounds)
        {
            Key = key;
            Bounds = bounds;
        }
    }
}