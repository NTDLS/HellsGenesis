using Si.GameEngine.Core;
using Si.GameEngine.Menus._Superclass;
using Si.GameEngine.Sprites.MenuItems;
using Si.Library.Mathematics.Geometry;
using Si.Menus.MultiPlayer;
using Si.Menus.SinglePlayer;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Menus
{
    /// <summary>
    /// The menu that is shows when the game is first started, allows to to select single or multiplayer.
    /// </summary>
    internal class MenuStartNewGame : MenuBase
    {
        public MenuStartNewGame(GameEngineCore gameEngine)
            : base(gameEngine)
        {
            var currentScaledScreenBounds = _gameEngine.Display.GetCurrentScaledScreenBounds();

            double offsetX = _gameEngine.Display.TotalCanvasSize.Width / 2;
            double offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new SiVector(offsetX, offsetY), "Strikeforce Infinity");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;
            itemTitle.IsHighlighted = true;

            var helpItem = CreateAndAddSelectableItem(new SiVector(offsetX, offsetY), "SINGLE_PLAYER", " Single Player ");
            helpItem.Selected = true;
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            helpItem = CreateAndAddSelectableItem(new SiVector(offsetX, offsetY), "MULTI_PLAYER", " Multiplayer ");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            offsetY += 50;

            helpItem = CreateAndAddTextblock(new SiVector(offsetX, offsetY), "Forward and Rotate with <W>, <A> and <S>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            helpItem = CreateAndAddTextblock(new SiVector(offsetX, offsetY), "Strafe with <LEFT> and <RIGHT> arrows.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            helpItem = CreateAndAddTextblock(new SiVector(offsetX, offsetY), "Surge Drive with <SHIFT>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            helpItem = CreateAndAddTextblock(new SiVector(offsetX, offsetY), "Fire primary with <SPACE>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 10;

            helpItem = CreateAndAddTextblock(new SiVector(offsetX, offsetY), "Fire secondary with <CTRL>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 10;

            helpItem = CreateAndAddTextblock(new SiVector(offsetX, offsetY), "Change weapons with <Q> and <E>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 10;

            //helpItem = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "Change speed with <UP> and <DOWN> arrows.");
            //helpItem.X -= helpItem.Size.Width / 2;
            //offsetY += helpItem.Size.Height + 10;

            //itemYes.Selected = true;

            OnExecuteSelection += MenuStartNewGame_OnExecuteSelection;
        }

        private bool MenuStartNewGame_OnExecuteSelection(SpriteMenuItem item)
        {
            if (item.Key == "SINGLE_PLAYER")
            {
                _gameEngine.Multiplay.SetPlayMode(SiPlayMode.SinglePlayer);
                _gameEngine.Menus.Add(new SpMenuSituationSelect(_gameEngine));
            }
            else if (item.Key == "MULTI_PLAYER")
            {
                _gameEngine.Multiplay.ConfigureConnection();
                _gameEngine.Menus.Add(new MpMenuCreateOrJoinLobby(_gameEngine));
            }
            return true;
        }
    }
}
