using NebulaSiege.Engine;
using NebulaSiege.Engine.Types;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Menus;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;

namespace NebulaSiege.Sprites
{
    internal class SpriteMenuItem : SpriteTextBlock
    {
        public _MenuBase Menu { get; private set; }

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

        public SpriteMenuItem(EngineCore core, _MenuBase menu, TextFormat format, SolidColorBrush color, NsPoint location)
            : base(core, format, color, location, true)
        {
            Menu = menu;
            Visable = true;
            Velocity = new HgVelocity();
        }
    }
}
