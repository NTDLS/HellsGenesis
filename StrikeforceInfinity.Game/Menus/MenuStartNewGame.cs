using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Menus.BaseClasses;
using StrikeforceInfinity.Game.Sprites.MenuItems;
using StrikeforceInfinity.Menus.MultiPlayer;
using StrikeforceInfinity.Menus.SinglePlayer;

namespace StrikeforceInfinity.Game.Menus
{
    /// <summary>
    /// The menu that is shows when the game is first started, allows to to select single or multiplayer.
    /// </summary>
    internal class MenuStartNewGame : MenuBase
    {
        public MenuStartNewGame(EngineCore gameCore)
            : base(gameCore)
        {
            var currentScaledScreenBounds = _gameCore.Display.GetCurrentScaledScreenBounds();

            double offsetX = _gameCore.Display.TotalCanvasSize.Width / 2;
            double offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new SiPoint(offsetX, offsetY), "Strikeforce Infinity");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;
            itemTitle.Highlight = true;

            var helpItem = CreateAndAddSelectableItem(new SiPoint(offsetX, offsetY), "SINGLE_PLAYER", " Single Player ");
            helpItem.Selected = true;
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            helpItem = CreateAndAddSelectableItem(new SiPoint(offsetX, offsetY), "MULTI_PLAYER", " Multiplayer ");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            offsetY += 50;

            helpItem = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "Move with <W>, <A>, <S>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            helpItem = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "Warp Drive with <SHIFT>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            helpItem = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "Fire primary with <SPACE>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 10;

            helpItem = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "Fire secondary with <CTRL>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 10;

            helpItem = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "Change weapons with <left> and <right> arrows.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 10;

            helpItem = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "Change speed with <up> and <down> arrows.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 10;

            //itemYes.Selected = true;

            OnExecuteSelection += MenuStartNewGame_OnExecuteSelection;
        }

        private void MenuStartNewGame_OnExecuteSelection(SpriteMenuItem item)
        {
            if (item.Key == "SINGLE_PLAYER")
            {
                _gameCore.Multiplay.SetPlayMode(HgPlayMode.SinglePlayer);
                _gameCore.Menus.Insert(new SpMenuSituationSelect(_gameCore));
            }
            else if (item.Key == "MULTI_PLAYER")
            {
                _gameCore.Multiplay.ConfigureConnection();
                _gameCore.Menus.Insert(new MpMenuCreateOrJoinLobby(_gameCore));
            }
        }
    }
}
