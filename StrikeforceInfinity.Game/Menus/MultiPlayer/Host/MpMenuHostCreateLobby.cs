using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Menus.BasesAndInterfaces;
using StrikeforceInfinity.Game.Sprites.MenuItems;
using StrikeforceInfinity.Shared.Payload;

namespace StrikeforceInfinity.Menus.MultiPlayer.Host
{
    /// <summary>
    /// Allows user to set parameters for a hosted game.
    /// </summary>
    internal class MpMenuHostCreateLobby : MenuBase
    {
        SpriteMenuSelectableTextInput textBoxName;
        SpriteMenuSelectableTextInput textBoxMaxPlayers;

        public MpMenuHostCreateLobby(EngineCore gameCore)
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
            if (!int.TryParse(textBoxMaxPlayers.Text, out int maxPlayers))
            {
                //return false;
            }

            //Create the game host on the server.
            var lobbyUID = _gameCore.Multiplay.CreateHost(new SiLobbyConfiguration(textBoxName.Text, maxPlayers));

            _gameCore.Multiplay.RegisterLobbyUID(lobbyUID);

            _gameCore.Menus.Insert(new MpMenuHostSituationSelect(_gameCore));
        }
    }
}
