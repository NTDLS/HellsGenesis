using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using Si.GameEngine.Core;
using Si.GameEngine.Menus._Superclass;
using Si.Library.Types;
using Si.Library.Types.Geometry;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Sprites.MenuItems
{
    /// <summary>
    /// Menu item that accepts user text input.
    /// </summary>
    public class SpriteMenuSelectableTextInput : SpriteMenuItem
    {
        public int CharacterLimit { get; set; }

        public SpriteMenuSelectableTextInput(GameEngineCore gameEngine, MenuBase menu, TextFormat format, SolidColorBrush color, SiPoint location, int characterLimit = 100)
            : base(gameEngine, menu, format, color, location)
        {
            ItemType = SiMenuItemType.SelectableTextInput;
            Visable = true;
            Velocity = new SiVelocity();
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
