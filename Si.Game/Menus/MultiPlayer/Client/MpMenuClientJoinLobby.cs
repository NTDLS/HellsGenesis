using Si.Game.Engine;
using Si.Shared.Types.Geometry;
using Si.Game.Menus.BasesAndInterfaces;
using Si.Game.Sprites.MenuItems;
using Si.Menus.SinglePlayer;
using System;
using System.Linq;

namespace Si.Menus.MultiPlayer.Client
{
    /// <summary>
    /// Allows user to set parameters for a hosted game.
    /// </summary>
    internal class MpMenuClientJoinLobby : MenuBase
    {
        readonly SpriteMenuSelectableTextInput textboxPlayerName;

        public MpMenuClientJoinLobby(EngineCore gameCore)
            : base(gameCore)
        {
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

            textboxPlayerName = CreateAndAddSelectableTextInput(new SiPoint(xPositionForTextBox, labelName.Y), "PLAYERNAME", "Player 2");
            textboxPlayerName.Selected = true;
            textboxPlayerName.Y = labelName.Y;

            //---------------------------------------------------------------------------------------------------------

            offsetY += textboxPlayerName.Size.Height + 25;

            //---------------------------------------------------------------------------------------------------------
            var labelplayerName = CreateAndAddTextblock(new SiPoint(xPositionForlabel, offsetY), "Active Lobbies: ".PadLeft(25));

            //---------------------------------------------------------------------------------------------------------

            offsetY += textboxPlayerName.Size.Height + 5;

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
            _gameCore.Menus.Insert(new MpMenuCreateOrJoinLobby(_gameCore));
            return true;
        }

        private bool MenuMultiplayerHostOrJoin_OnExecuteSelection(SpriteMenuItem item)
        {
            var lobbyUID = Guid.Parse(item.Key);

            _gameCore.Multiplay.RegisterLobbyUID(lobbyUID, textboxPlayerName.Text);
            _gameCore.Menus.Insert(new MpMenuClientSelectLoadout(_gameCore));

            return true;
        }
    }
}
