using HG.Engine;
using HG.Engine.Menus;
using HG.Types;
using System.Drawing;

namespace HG.Actors
{
    internal class ActorMenuItem : ActorTextBlock
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

        public ActorMenuItem(Core core, BaseMenu menu, string font, Brush color, double size, HgPoint<double> location)
            : base(core, font, color, size, location, true)
        {
            Menu = menu;
            Visable = true;
            Velocity = new HgVelocity<double>();
        }
    }
}
