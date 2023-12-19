using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Menus.BaseClasses;
using StrikeforceInfinity.Game.Sprites.MenuItems;
using StrikeforceInfinity.Shared.Payload;

namespace StrikeforceInfinity.Game.Menus
{
    /// <summary>
    /// Allows user to set parameters for a hosted game.
    /// </summary>
    internal class MenuMultiplayerHostGame : MenuBase
    {
        SpriteMenuSelectableTextInput textBoxName;
        SpriteMenuSelectableTextInput textBoxMaxPlayers;

        public MenuMultiplayerHostGame(EngineCore gameCore)
            : base(gameCore)
        {
            var currentScaledScreenBounds = _gameCore.Display.GetCurrentScaledScreenBounds();

            double offsetX = _gameCore.Display.TotalCanvasSize.Width / 2;
            double offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new SiPoint(offsetX, offsetY), "Host a Game");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;
            itemTitle.Highlight = true;

            var helpItem = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "NameLabel");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            textBoxName = CreateAndAddSelectableTextInput(new SiPoint(offsetX, offsetY), "Name", "My Game Name");
            textBoxName.Selected = true;
            textBoxName.X -= textBoxName.Size.Width / 2;
            offsetY += textBoxName.Size.Height + 5;

            textBoxMaxPlayers = CreateAndAddSelectableTextInput(new SiPoint(offsetX, offsetY), "MaxPlayers", "100");
            textBoxMaxPlayers.X -= textBoxMaxPlayers.Size.Width / 2;
            offsetY += textBoxMaxPlayers.Size.Height + 5;

            helpItem = CreateAndAddSelectableItem(new SiPoint(offsetX, offsetY), "Start", " Start ");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            OnExecuteSelection += MenuMultiplayerHostOrJoin_OnExecuteSelection;
        }

        private void MenuMultiplayerHostOrJoin_OnExecuteSelection(SpriteMenuItem item)
        {
            //var nameTextBox = MenuItemByKey<SpriteMenuSelectableTextInput>("Name");

            if (!int.TryParse(textBoxMaxPlayers.Text, out int maxPlayers))
            {
                //return false;
            }

            _gameCore.Multiplay.CreateHost(new SiGameHost(textBoxName.Text, maxPlayers));

            _gameCore.Menus.Insert(new SituationSelectMenu(_gameCore));

            //SpriteMenuSelectableTextInput

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
