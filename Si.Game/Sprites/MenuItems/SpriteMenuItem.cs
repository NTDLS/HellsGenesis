using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using Si.Game.Engine;
using Si.Game.Engine.Types;
using Si.Game.Engine.Types.Geometry;
using Si.Game.Menus.BasesAndInterfaces;
using static Si.Shared.SiConstants;

namespace Si.Game.Sprites.MenuItems
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

        public SiMenuItemType ItemType { get; set; }

        public SpriteMenuItem(EngineCore gameCore, MenuBase menu, TextFormat format, SolidColorBrush color, SiPoint location)
            : base(gameCore, format, color, location, true)
        {
            ItemType = SiMenuItemType.Undefined;
            Menu = menu;
            Visable = true;
            Velocity = new SiVelocity();


        }
    }
}
