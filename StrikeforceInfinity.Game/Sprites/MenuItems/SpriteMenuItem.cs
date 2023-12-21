using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Menus.BasesAndInterfaces;

namespace StrikeforceInfinity.Game.Sprites.MenuItems
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

        public SpriteMenuItem(EngineCore gameCore, MenuBase menu, TextFormat format, SolidColorBrush color, SiPoint location)
            : base(gameCore, format, color, location, true)
        {
            ItemType = HgMenuItemType.Undefined;
            Menu = menu;
            Visable = true;
            Velocity = new HgVelocity();


        }
    }
}
