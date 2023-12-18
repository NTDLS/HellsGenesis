using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Engine.Types;
using NebulaSiege.Game.Engine.Types.Geometry;
using NebulaSiege.Game.Menus.BaseClasses;
using NebulaSiege.Game.Sprites;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;

namespace NebulaSiege.Game.Sprites.MenuItems
{
    /// <summary>
    /// Represents a selectable menu item.
    /// </summary>
    internal class SpriteMenuItem : SpriteTextBlock
    {
        /// <summary>
        /// User object associated with the menu item.
        /// </summary>
        public object UserData { get; set; }
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
                    Menu.InvokeSelectionChanged(this);
                }
                _selected = value;
            }
        }

        public string Key { get; set; }

        public HgMenuItemType ItemType { get; set; }

        public SpriteMenuItem(EngineCore core, MenuBase menu, TextFormat format, SolidColorBrush color, NsPoint location)
            : base(core, format, color, location, true)
        {
            ItemType = HgMenuItemType.Undefined;
            Menu = menu;
            Visable = true;
            Velocity = new HgVelocity();


        }
    }
}
