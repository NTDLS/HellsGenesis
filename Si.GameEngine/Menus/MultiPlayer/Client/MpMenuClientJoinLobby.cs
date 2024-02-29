using Si.GameEngine.Menus._Superclass;
using Si.GameEngine.Sprites.MenuItems;
using Si.Library.Mathematics.Geometry;
using Si.Menus.SinglePlayer;
using System;
using System.Linq;
using static Si.Library.SiConstants;

namespace Si.Menus.MultiPlayer.Client
{
    /// <summary>
    /// Allows user to set parameters for a hosted game.
    /// </summary>
    internal class MpMenuClientJoinLobby : MenuBase
    {
        private readonly SpriteMenuSelectableTextInput _textboxPlayerName;

        public MpMenuClientJoinLobby(GameEngine.GameEngineCore gameEngine)
            : base(gameEngine)
        {
            _gameEngine.Multiplay.SetPlayMode(SiPlayMode.MutiPlayerClient);

            var currentScaledScreenBounds = _gameEngine.Display.GetCurrentScaledScreenBounds();

            float offsetX = _gameEngine.Display.TotalCanvasSize.Width / 2;
            float offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new SiVector(offsetX, offsetY), "Join Game");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;
            itemTitle.IsHighlighted = true;

            //---------------------------------------------------------------------------------------------------------

            var labelName = CreateAndAddTextblock(new SiVector(offsetX, offsetY), "Player Name: ".PadLeft(25));
            labelName.X -= (labelName.Size.Width) + 200;

            float xPositionForlabel = labelName.X; //Save the X position for lables.
            float xPositionForTextBox = labelName.X + labelName.Size.Width; //Save the X position for textboxes.

            _textboxPlayerName = CreateAndAddSelectableTextInput(new SiVector(xPositionForTextBox, labelName.Y), "PLAYERNAME", "Player 2");
            _textboxPlayerName.Selected = true;
            _textboxPlayerName.Y = labelName.Y;

            //---------------------------------------------------------------------------------------------------------

            offsetY += _textboxPlayerName.Size.Height + 25;

            //---------------------------------------------------------------------------------------------------------
            var labelplayerName = CreateAndAddTextblock(new SiVector(xPositionForlabel, offsetY), "Active Lobbies: ".PadLeft(25));

            //---------------------------------------------------------------------------------------------------------

            offsetY += _textboxPlayerName.Size.Height + 5;

            var gameHosts = _gameEngine.Multiplay.ListLobbies();
            foreach (var gameHost in gameHosts)
            {
                var helpItem = CreateAndAddSelectableItem(new SiVector(xPositionForlabel, offsetY), gameHost.UID.ToString(), gameHost.Name);
                offsetY += helpItem.Size.Height + 5;
            }

            var firstItem = VisibleSelectableItems().FirstOrDefault();
            if (firstItem != null)
            {
                firstItem.Selected = true;
            }

            OnExecuteSelection += MenuMultiplayerHostOrJoin_OnExecuteSelection;
            OnEscape += MpMenuClientJoinLobby_OnEscape;
        }

        private bool MpMenuClientJoinLobby_OnEscape()
        {
            _gameEngine.Menus.Add(new MpMenuCreateOrJoinLobby(_gameEngine));
            return true;
        }

        private bool MenuMultiplayerHostOrJoin_OnExecuteSelection(SpriteMenuItem item)
        {
            var lobbyUID = Guid.Parse(item.Key);

            _gameEngine.Multiplay.RegisterLobbyUID(lobbyUID, _textboxPlayerName.Text);
            _gameEngine.Menus.Add(new MpMenuClientSelectLoadout(_gameEngine));

            return true;
        }
    }
}
