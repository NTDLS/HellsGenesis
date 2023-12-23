using Si.Game.Engine;
using Si.Game.Engine.Types.Geometry;
using Si.Game.Menus.BasesAndInterfaces;
using Si.Game.Sprites.MenuItems;
using Si.Shared.Payload;

namespace Si.Menus.MultiPlayer.Host
{
    /// <summary>
    /// Allows user to set parameters for a hosted game.
    /// </summary>
    internal class MpMenuHostCreateLobby : MenuBase
    {
        readonly SpriteMenuSelectableTextInput textBoxName;
        readonly SpriteMenuSelectableTextInput textBoxMaxPlayers;
        readonly SpriteMenuSelectableTextInput textboxPlayerName;

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

            //---------------------------------------------------------------------------------------------------------

            var labelName = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "Lobby Name: ".PadLeft(25));
            labelName.X -= (labelName.Size.Width) + 200;

            double xPositionForlabel = labelName.X; //Save the X position for lables.
            double xPositionForTextBox = labelName.X + labelName.Size.Width; //Save the X position for textboxes.

            textBoxName = CreateAndAddSelectableTextInput(new SiPoint(xPositionForTextBox, labelName.Y), "NAME", "My Game Name");
            textBoxName.Selected = true;
            textBoxName.Y = labelName.Y;

            //---------------------------------------------------------------------------------------------------------

            offsetY += textBoxName.Size.Height + 5;

            //---------------------------------------------------------------------------------------------------------
            var labelplayerName = CreateAndAddTextblock(new SiPoint(xPositionForlabel, offsetY), "Player Name: ".PadLeft(25));
            textboxPlayerName = CreateAndAddSelectableTextInput(new SiPoint(xPositionForTextBox, labelplayerName.Y), "PLAYERNAME", "Player 1");
            //---------------------------------------------------------------------------------------------------------

            offsetY += textboxPlayerName.Size.Height + 5;

            //---------------------------------------------------------------------------------------------------------
            var labelMaxPlayers = CreateAndAddTextblock(new SiPoint(xPositionForlabel, offsetY), "Max Players: ".PadLeft(25));
            textBoxMaxPlayers = CreateAndAddSelectableTextInput(new SiPoint(xPositionForTextBox, labelMaxPlayers.Y), "MAXPLAYERS", "100");
            //---------------------------------------------------------------------------------------------------------

            offsetY += textBoxMaxPlayers.Size.Height + 25;

            //---------------------------------------------------------------------------------------------------------

            var startButton = CreateAndAddSelectableItem(new SiPoint(xPositionForTextBox, offsetY), "NEXT", "Next >");

            OnExecuteSelection += MenuMultiplayerHostOrJoin_OnExecuteSelection;
            OnEscape += MpMenuHostCreateLobby_OnEscape;
        }

        private bool MpMenuHostCreateLobby_OnEscape()
        {
            _gameCore.Menus.Insert(new MpMenuCreateOrJoinLobby(_gameCore));
            return true;
        }

        private bool MenuMultiplayerHostOrJoin_OnExecuteSelection(SpriteMenuItem item)
        {
            if (!int.TryParse(textBoxMaxPlayers.Text, out int maxPlayers))
            {
                return false;
            }

            //TODO: validate user input here.

            //Create the game host on the server.
            var lobbyUID = _gameCore.Multiplay.CreateLobby(new SiLobbyConfiguration(textBoxName.Text, maxPlayers));

            _gameCore.Multiplay.RegisterLobbyUID(lobbyUID, textboxPlayerName.Text);

            _gameCore.Menus.Insert(new MpMenuHostSituationSelect(_gameCore));

            return true;
        }
    }
}
