using Si.GameEngine.Core;
using Si.GameEngine.Menus._Superclass;
using Si.GameEngine.Sprites.MenuItems;
using Si.Menus.SinglePlayer;
using Si.Shared.Types.Geometry;
using System;
using System.Linq;
using static Si.Shared.SiConstants;

namespace Si.Menus.MultiPlayer.Client
{
    /// <summary>
    /// Allows user to set parameters for a hosted game.
    /// </summary>
    internal class MpMenuClientJoinLobby : MenuBase
    {
        private readonly SpriteMenuSelectableTextInput _textboxPlayerName;

        public MpMenuClientJoinLobby(Engine gameCore)
            : base(gameCore)
        {
            _gameCore.Multiplay.SetPlayMode(SiPlayMode.MutiPlayerClient);

            var currentScaledScreenBounds = _gameCore.Display.GetCurrentScaledScreenBounds();

            double offsetX = _gameCore.Display.TotalCanvasSize.Width / 2;
            double offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new SiPoint(offsetX, offsetY), "Join Game");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;
            itemTitle.Highlight = true;

            //---------------------------------------------------------------------------------------------------------

            var labelName = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "Player Name: ".PadLeft(25));
            labelName.X -= (labelName.Size.Width) + 200;

            double xPositionForlabel = labelName.X; //Save the X position for lables.
            double xPositionForTextBox = labelName.X + labelName.Size.Width; //Save the X position for textboxes.

            _textboxPlayerName = CreateAndAddSelectableTextInput(new SiPoint(xPositionForTextBox, labelName.Y), "PLAYERNAME", "Player 2");
            _textboxPlayerName.Selected = true;
            _textboxPlayerName.Y = labelName.Y;

            //---------------------------------------------------------------------------------------------------------

            offsetY += _textboxPlayerName.Size.Height + 25;

            //---------------------------------------------------------------------------------------------------------
            var labelplayerName = CreateAndAddTextblock(new SiPoint(xPositionForlabel, offsetY), "Active Lobbies: ".PadLeft(25));

            //---------------------------------------------------------------------------------------------------------

            offsetY += _textboxPlayerName.Size.Height + 5;

            var gameHosts = _gameCore.Multiplay.ListLobbies();
            foreach (var gameHost in gameHosts)
            {
                var helpItem = CreateAndAddSelectableItem(new SiPoint(xPositionForlabel, offsetY), gameHost.UID.ToString(), gameHost.Name);
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
            _gameCore.Menus.Add(new MpMenuCreateOrJoinLobby(_gameCore));
            return true;
        }

        private bool MenuMultiplayerHostOrJoin_OnExecuteSelection(SpriteMenuItem item)
        {
            var lobbyUID = Guid.Parse(item.Key);

            _gameCore.Multiplay.RegisterLobbyUID(lobbyUID, _textboxPlayerName.Text);
            _gameCore.Menus.Add(new MpMenuClientSelectLoadout(_gameCore));

            return true;
        }
    }
}
