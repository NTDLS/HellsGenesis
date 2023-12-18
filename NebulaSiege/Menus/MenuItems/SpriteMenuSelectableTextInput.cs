using NebulaSiege.Engine;
using NebulaSiege.Engine.Types;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Menus.BaseClasses;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;

namespace NebulaSiege.Menus.MenuItems
{
    /// <summary>
    /// Menu item that accepts user text input.
    /// </summary>
    internal class SpriteMenuSelectableTextInput : SpriteMenuItem
    {
        public SpriteMenuSelectableTextInput(EngineCore core, MenuBase menu, TextFormat format, SolidColorBrush color, NsPoint location)
            : base(core, menu, format, color, location)
        {
            ItemType = HgMenuItemType.SelectableTextInput;
            Visable = true;
            Velocity = new HgVelocity();
        }
    }
}
