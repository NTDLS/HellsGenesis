using NebulaSiege.Client.Payloads;
using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Engine.Types.Geometry;
using NebulaSiege.Game.Menus.BaseClasses;
using NebulaSiege.Game.Sprites.MenuItems;
using System.Windows.Forms;

namespace NebulaSiege.Game.Menus
{
    /// <summary>
    /// Allows user to set parameters for a hosted game.
    /// </summary>
    internal class MenuMultiplayerHostGame : MenuBase
    {
        SpriteMenuSelectableTextInput textBoxName;
        SpriteMenuSelectableTextInput textBoxMaxPlayers;

        public MenuMultiplayerHostGame(EngineCore core)
            : base(core)
        {
            var currentScaledScreenBounds = _core.Display.GetCurrentScaledScreenBounds();

            double offsetX = _core.Display.TotalCanvasSize.Width / 2;
            double offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new NsPoint(offsetX, offsetY), "Host a Game");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;
            itemTitle.Highlight = true;

            var helpItem = CreateAndAddTextblock(new NsPoint(offsetX, offsetY), "NameLabel");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            textBoxName = CreateAndAddSelectableTextInput(new NsPoint(offsetX, offsetY), "Name", "My Game Name");
            textBoxName.Selected = true;
            textBoxName.X -= textBoxName.Size.Width / 2;
            offsetY += textBoxName.Size.Height + 5;

            textBoxMaxPlayers = CreateAndAddSelectableTextInput(new NsPoint(offsetX, offsetY), "MaxPlayers", "100");
            textBoxMaxPlayers.X -= textBoxMaxPlayers.Size.Width / 2;
            offsetY += textBoxMaxPlayers.Size.Height + 5;

            helpItem = CreateAndAddSelectableItem(new NsPoint(offsetX, offsetY), "Start", " Start ");
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

            var configuration = new NsGameHost(textBoxName.Text, maxPlayers);
            _core.ServerClient.GameHost.Create(configuration);

            //SpriteMenuSelectableTextInput

            /*
            if (item.Key == "JOIN")
            {
                //_core.Menus.Insert(new SituationSelectMenu(_core));
            }
            else if (item.Key == "HOST")
            {
                //_core.Menus.Insert(new SituationSelectMenu(_core));
            }
            */
        }
    }
}
