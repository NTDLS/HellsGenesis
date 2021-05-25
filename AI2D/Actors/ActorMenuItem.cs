using AI2D.Engine;
using AI2D.Engine.Menus;
using AI2D.Types;
using System.Drawing;

namespace AI2D.Actors
{
    public class ActorMenuItem : ActorTextBlock
    {
        public BaseMenu Menu { get; private set; }

        private bool _selected = false;

        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                if (_selected != value)
                {
                    Menu.SelectionChanged(this);
                }
                _selected = value;
            }
        }

        public enum MenuItemType
        {
            Title,
            Text,
            Item
        }

        public string Name { get; set; }

        public MenuItemType ItemType { get; set; }

        public ActorMenuItem(Core core, BaseMenu menu, string font, Brush color, double size, Point<double> location)
            : base(core, font, color, size, location, true)
        {
            Menu = menu;
            Visable = true;
            Velocity = new Velocity<double>();
        }
    }
}
