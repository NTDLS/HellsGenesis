using Si.GameEngine.Engine;
using Si.GameEngine.Menus.BasesAndInterfaces;
using Si.GameEngine.Sprites.MenuItems;
using Si.Shared.Payload;
using Si.Shared.Types.Geometry;

namespace Si.Menus.MultiPlayer.Host
{
    /// <summary>
    /// Allows user to set parameters for a hosted game.
    /// </summary>
    internal class MpMenuHostCreateLobby : MenuBase
    {
        readonly SpriteMenuSelectableTextInput textBoxName;
        readonly SpriteMenuSelectableTextInput textBoxMinPlayers;
        readonly SpriteMenuSelectableTextInput textBoxMaxPlayers;
        readonly SpriteMenuSelectableTextInput textBoxAutoStartSeconds;
        readonly SpriteMenuSelectableTextInput textboxPlayerName;

        public MpMenuHostCreateLobby(EngineCore gameCore)
            : base(gameCore)
        {
            var currentScaledScreenBounds = _gameCore.Display.GetCurrentScaledScreenBounds();

            double offsetX = _gameCore.Display.TotalCanvasSize.Width / 2;
            double offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new SiPoint(offsetX, offsetY), "Host a Game");
            itemTitle.LocalX -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;
            itemTitle.Highlight = true;

            //---------------------------------------------------------------------------------------------------------

            var labelName = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "Lobby Name: ".PadLeft(25));
            labelName.LocalX -= (labelName.Size.Width) + 200;

            double xPositionForlabel = labelName.LocalX; //Save the X position for lables.
            double xPositionForTextBox = labelName.LocalX + labelName.Size.Width; //Save the X position for textboxes.

            textBoxName = CreateAndAddSelectableTextInput(new SiPoint(xPositionForTextBox, labelName.LocalY), "NAME", "My Game Name");
            textBoxName.Selected = true;
            textBoxName.LocalY = labelName.LocalY;

            //---------------------------------------------------------------------------------------------------------

            offsetY += textBoxName.Size.Height + 5;

            //---------------------------------------------------------------------------------------------------------
            var labelplayerName = CreateAndAddTextblock(new SiPoint(xPositionForlabel, offsetY), "Player Name: ".PadLeft(25));
            textboxPlayerName = CreateAndAddSelectableTextInput(new SiPoint(xPositionForTextBox, labelplayerName.LocalY), "PLAYERNAME", "Player 1");
            //---------------------------------------------------------------------------------------------------------

            offsetY += textboxPlayerName.Size.Height + 5;

            //---------------------------------------------------------------------------------------------------------
            var labelMinPlayers = CreateAndAddTextblock(new SiPoint(xPositionForlabel, offsetY), "Min. Players: ".PadLeft(25));
            textBoxMinPlayers = CreateAndAddSelectableTextInput(new SiPoint(xPositionForTextBox, labelMinPlayers.LocalY), "MINPLAYERS", "2");
            //---------------------------------------------------------------------------------------------------------

            offsetY += textboxPlayerName.Size.Height + 5;

            //---------------------------------------------------------------------------------------------------------
            var labelMaxPlayers = CreateAndAddTextblock(new SiPoint(xPositionForlabel, offsetY), "Max Players: ".PadLeft(25));
            textBoxMaxPlayers = CreateAndAddSelectableTextInput(new SiPoint(xPositionForTextBox, labelMaxPlayers.LocalY), "MAXPLAYERS", "100");
            //---------------------------------------------------------------------------------------------------------

            offsetY += textboxPlayerName.Size.Height + 5;

            //---------------------------------------------------------------------------------------------------------
            var labelAutoStartSeconds = CreateAndAddTextblock(new SiPoint(xPositionForlabel, offsetY), "Auto Start Seconds: ".PadLeft(25));
            textBoxAutoStartSeconds = CreateAndAddSelectableTextInput(new SiPoint(xPositionForTextBox, labelAutoStartSeconds.LocalY), "AUTOSTARTSECONDS", "60");
            //---------------------------------------------------------------------------------------------------------

            offsetY += textBoxMaxPlayers.Size.Height + 25;

            //---------------------------------------------------------------------------------------------------------

            var startButton = CreateAndAddSelectableItem(new SiPoint(xPositionForTextBox, offsetY), "NEXT", "Next >");

            OnExecuteSelection += MenuMultiplayerHostOrJoin_OnExecuteSelection;
            OnEscape += MpMenuHostCreateLobby_OnEscape;
        }

        private bool MpMenuHostCreateLobby_OnEscape()
        {
            _gameCore.Menus.Add(new MpMenuCreateOrJoinLobby(_gameCore));
            return true;
        }

        private bool MenuMultiplayerHostOrJoin_OnExecuteSelection(SpriteMenuItem item)
        {
            if (!int.TryParse(textBoxMinPlayers.Text, out int minxPlayers))
            {
                return false;
            }
            if (!int.TryParse(textBoxMaxPlayers.Text, out int maxPlayers))
            {
                return false;
            }
            if (!int.TryParse(textBoxAutoStartSeconds.Text, out int autoStartSeconds))
            {
                return false;
            }

            //TODO: validate user input here.

            //Create the game host on the server.
            var lobbyUID = _gameCore.Multiplay.CreateLobby(new SiLobbyConfiguration(textBoxName.Text, minxPlayers, maxPlayers, autoStartSeconds));

            _gameCore.Multiplay.RegisterLobbyUID(lobbyUID, textboxPlayerName.Text);

            _gameCore.Menus.Add(new MpMenuHostSituationSelect(_gameCore));

            return true;
        }
    }
}
