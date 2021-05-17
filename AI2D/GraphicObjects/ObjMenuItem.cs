using AI2D.Engine;
using AI2D.Types;
using System.Drawing;

namespace AI2D.GraphicObjects
{
    public class ObjMenuItem : ObjTextBlock
    {
        public bool Selected { get; set; }
        public enum MenuItemType
        {
            Title,
            Text,
            Item
        }

        public string Name { get; set; }

        public MenuItemType ItemType { get; set; }

        public ObjMenuItem(Core core, string font, Brush color, double size, PointD location)
            : base(core, font, color, size, location, true)
        {
            Visable = true;
            Velocity = new Types.VelocityD();
        }
    }
}
