using System.Drawing;

namespace HG.Engine
{
    internal class HgQuadrant
    {
        public Point Key { get; private set; }
        public Rectangle Bounds { get; private set; }

        public HgQuadrant(Point key, Rectangle bounds)
        {
            Key = key;
            Bounds = bounds;
        }
    }
}