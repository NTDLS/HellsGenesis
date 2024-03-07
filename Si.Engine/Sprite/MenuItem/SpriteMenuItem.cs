using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using Si.Engine;
using Si.GameEngine.Menu._Superclass;
using Si.Library.Mathematics;
using Si.Library.Mathematics.Geometry;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Sprite.MenuItem
{
    /// <summary>
    /// Represents a selectable menu item.
    /// </summary>
    public class SpriteMenuItem : SpriteTextBlock
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

        public SpriteMenuItem(EngineCore engine, MenuBase menu, TextFormat format, SolidColorBrush color, SiPoint location)
            : base(engine, format, color, location, true)
        {
            ItemType = SiMenuItemType.Undefined;
            Menu = menu;
            Visable = true;
            Velocity = new SiVelocity();


        }
    }
}
