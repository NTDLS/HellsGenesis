using Si.GameEngine.Menus._Superclass;
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
        private readonly SpriteMenuSelectableTextInput _textBoxName;
        private readonly SpriteMenuSelectableTextInput _textBoxMinPlayers;
        private readonly SpriteMenuSelectableTextInput _textBoxMaxPlayers;
        private readonly SpriteMenuSelectableTextInput _textBoxAutoStartSeconds;
        private readonly SpriteMenuSelectableTextInput _textboxPlayerName;

        public MpMenuHostCreateLobby(GameEngine.Core.Engine gameEngine)
            : base(gameEngine)
        {
            var currentScaledScreenBounds = _gameEngine.Display.GetCurrentScaledScreenBounds();

            double offsetX = _gameEngine.Display.TotalCanvasSize.Width / 2;
            double offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new SiPoint(offsetX, offsetY), "Host a Game");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;
            itemTitle.IsHighlighted = true;

            //---------------------------------------------------------------------------------------------------------

            var labelName = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "Lobby Name: ".PadLeft(25));
            labelName.X -= (labelName.Size.Width) + 200;

            double xPositionForlabel = labelName.X; //Save the X position for lables.
            double xPositionForTextBox = labelName.X + labelName.Size.Width; //Save the X position for textboxes.

            _textBoxName = CreateAndAddSelectableTextInput(new SiPoint(xPositionForTextBox, labelName.Y), "NAME", "My Game Name");
            _textBoxName.Selected = true;
            _textBoxName.Y = labelName.Y;

            //---------------------------------------------------------------------------------------------------------

            offsetY += _textBoxName.Size.Height + 5;

            //---------------------------------------------------------------------------------------------------------
            var labelplayerName = CreateAndAddTextblock(new SiPoint(xPositionForlabel, offsetY), "Player Name: ".PadLeft(25));
            _textboxPlayerName = CreateAndAddSelectableTextInput(new SiPoint(xPositionForTextBox, labelplayerName.Y), "PLAYERNAME", "Player 1");
            //---------------------------------------------------------------------------------------------------------

            offsetY += _textboxPlayerName.Size.Height + 5;

            //---------------------------------------------------------------------------------------------------------
            var labelMinPlayers = CreateAndAddTextblock(new SiPoint(xPositionForlabel, offsetY), "Min. Players: ".PadLeft(25));
            _textBoxMinPlayers = CreateAndAddSelectableTextInput(new SiPoint(xPositionForTextBox, labelMinPlayers.Y), "MINPLAYERS", "2");
            //---------------------------------------------------------------------------------------------------------

            offsetY += _textboxPlayerName.Size.Height + 5;

            //---------------------------------------------------------------------------------------------------------
            var labelMaxPlayers = CreateAndAddTextblock(new SiPoint(xPositionForlabel, offsetY), "Max Players: ".PadLeft(25));
            _textBoxMaxPlayers = CreateAndAddSelectableTextInput(new SiPoint(xPositionForTextBox, labelMaxPlayers.Y), "MAXPLAYERS", "100");
            //---------------------------------------------------------------------------------------------------------

            offsetY += _textboxPlayerName.Size.Height + 5;

            //---------------------------------------------------------------------------------------------------------
            var labelAutoStartSeconds = CreateAndAddTextblock(new SiPoint(xPositionForlabel, offsetY), "Auto Start Seconds: ".PadLeft(25));
            _textBoxAutoStartSeconds = CreateAndAddSelectableTextInput(new SiPoint(xPositionForTextBox, labelAutoStartSeconds.Y), "AUTOSTARTSECONDS", "60");
            //---------------------------------------------------------------------------------------------------------

            offsetY += _textBoxMaxPlayers.Size.Height + 25;

            //---------------------------------------------------------------------------------------------------------

            var startButton = CreateAndAddSelectableItem(new SiPoint(xPositionForTextBox, offsetY), "NEXT", "Next >");

            OnExecuteSelection += MenuMultiplayerHostOrJoin_OnExecuteSelection;
            OnEscape += MpMenuHostCreateLobby_OnEscape;
        }

        private bool MpMenuHostCreateLobby_OnEscape()
        {
            _gameEngine.Menus.Add(new MpMenuCreateOrJoinLobby(_gameEngine));
            return true;
        }

        private bool MenuMultiplayerHostOrJoin_OnExecuteSelection(SpriteMenuItem item)
        {
            if (!int.TryParse(_textBoxMinPlayers.Text, out int minxPlayers))
            {
                return false;
            }
            if (!int.TryParse(_textBoxMaxPlayers.Text, out int maxPlayers))
            {
                return false;
            }
            if (!int.TryParse(_textBoxAutoStartSeconds.Text, out int autoStartSeconds))
            {
                return false;
            }

            //TODO: validate user input here.

            //Create the game host on the server.
            var lobbyUID = _gameEngine.Multiplay.CreateLobby(new SiLobbyConfiguration(_textBoxName.Text, minxPlayers, maxPlayers, autoStartSeconds));

            _gameEngine.Multiplay.RegisterLobbyUID(lobbyUID, _textboxPlayerName.Text);

            _gameEngine.Menus.Add(new MpMenuHostSituationSelect(_gameEngine));

            return true;
        }
    }
}
