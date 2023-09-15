using HG.Engine;
using HG.Engine.Types;
using HG.Engine.Types.Geometry;
using HG.Menus.BaseClasses;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;

namespace HG.Actors.Ordinary
{
    internal class ActorMenuItem : ActorTextBlock
    {
        public MenuBase Menu { get; private set; }

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

        public string Key { get; set; }

        public HgMenuItemType ItemType { get; set; }

        public ActorMenuItem(EngineCore core, MenuBase menu, TextFormat format, SolidColorBrush color, HgPoint location)
            : base(core, format, color, location, true)
        {
            Menu = menu;
            Visable = true;
            Velocity = new HgVelocity();
        }
    }
}
