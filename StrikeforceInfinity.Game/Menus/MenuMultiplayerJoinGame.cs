using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Menus.BaseClasses;
using StrikeforceInfinity.Game.Sprites.MenuItems;
using System;
using System.Linq;

namespace StrikeforceInfinity.Game.Menus
{
    /// <summary>
    /// Allows user to set parameters for a hosted game.
    /// </summary>
    internal class MenuMultiplayerJoinGame : MenuBase
    {
        public MenuMultiplayerJoinGame(EngineCore gameCore)
            : base(gameCore)
        {
            var currentScaledScreenBounds = _gameCore.Display.GetCurrentScaledScreenBounds();

            double offsetX = _gameCore.Display.TotalCanvasSize.Width / 2;
            double offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new SiPoint(offsetX, offsetY), "Join Game");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;
            itemTitle.Highlight = true;

            var helpItem = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "Name");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            var gameHosts = _gameCore.Multiplay.GetHostList();
            foreach (var gameHost in gameHosts)
            {
                helpItem = CreateAndAddSelectableItem(new SiPoint(offsetX, offsetY), gameHost.UID.ToString(), gameHost.Name);
                helpItem.X -= helpItem.Size.Width / 2;
                offsetY += helpItem.Size.Height + 5;
            }

            var firstItem = VisibleSelectableItems().FirstOrDefault();
            if (firstItem != null)
            {
                firstItem.Selected = true;
            }

            OnExecuteSelection += MenuMultiplayerHostOrJoin_OnExecuteSelection;
        }

        private void MenuMultiplayerHostOrJoin_OnExecuteSelection(SpriteMenuItem item)
        {
            _gameCore.Multiplay.SetGameHostUID(Guid.Parse(item.Key));

            _gameCore.Menus.Insert(new SituationSelectMenu(_gameCore));

            /*
            if (item.Key == "JOIN")
            {
                //_gameCore.Menus.Insert(new SituationSelectMenu(_gameCore));
            }
            else if (item.Key == "HOST")
            {
                //_gameCore.Menus.Insert(new SituationSelectMenu(_gameCore));
            }
            */
        }
    }
}
