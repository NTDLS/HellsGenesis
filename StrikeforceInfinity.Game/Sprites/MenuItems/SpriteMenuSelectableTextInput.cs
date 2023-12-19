using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Menus.BaseClasses;

namespace StrikeforceInfinity.Game.Sprites.MenuItems
{
    /// <summary>
    /// Menu item that accepts user text input.
    /// </summary>
    internal class SpriteMenuSelectableTextInput : SpriteMenuItem
    {
        public int CharacterLimit { get; set; }

        public SpriteMenuSelectableTextInput(EngineCore gameCore, MenuBase menu, TextFormat format, SolidColorBrush color, SiPoint location, int characterLimit = 100)
            : base(gameCore, menu, format, color, location)
        {
            ItemType = HgMenuItemType.SelectableTextInput;
            Visable = true;
            Velocity = new HgVelocity();
            CharacterLimit = characterLimit;
        }

        public void Backspace()
        {
            if (Text.Length > 0)
            {
                Text = Text.Substring(0, Text.Length - 1);
            }
        }

        public void Append(string text)
        {
            var totalString = Text + text;

            if (totalString.Length > CharacterLimit)
            {
                Text = totalString.Substring(0, CharacterLimit);
            }
            else
            {
                Text = totalString;
            }
        }

    }
}
