using Si.Game.Engine;
using Si.Shared.Types.Geometry;
using Si.Game.Menus;
using Si.Game.Menus.BasesAndInterfaces;
using Si.Game.Sprites.MenuItems;
using Si.Menus.MultiPlayer.Client;
using Si.Menus.MultiPlayer.Host;
using static Si.Shared.SiConstants;

namespace Si.Menus.MultiPlayer
{
    /// <summary>
    /// Allows the player to select to host or join a game.
    /// </summary>
    internal class MpMenuCreateOrJoinLobby : MenuBase
    {
        public MpMenuCreateOrJoinLobby(EngineCore gameCore)
            : base(gameCore)
        {
            var currentScaledScreenBounds = _gameCore.Display.GetCurrentScaledScreenBounds();

            double offsetX = _gameCore.Display.TotalCanvasSize.Width / 2;
            double offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new SiPoint(offsetX, offsetY), "Multiplayer");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;
            itemTitle.Highlight = true;

            var helpItem = CreateAndAddSelectableItem(new SiPoint(offsetX, offsetY), "JOIN", " Join a Game ");
            helpItem.Selected = true;
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            helpItem = CreateAndAddSelectableItem(new SiPoint(offsetX, offsetY), "HOST", " Host a Game ");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            OnExecuteSelection += MenuMultiplayerHostOrJoin_OnExecuteSelection;
            OnEscape += MenuMultiplayerHostOrJoin_OnEscape;
        }

        private bool MenuMultiplayerHostOrJoin_OnEscape()
        {
            _gameCore.Menus.Insert(new MenuStartNewGame(_gameCore));
            return true;
        }

        private bool MenuMultiplayerHostOrJoin_OnExecuteSelection(SpriteMenuItem item)
        {
            if (item.Key == "JOIN")
            {
                _gameCore.Multiplay.SetPlayMode(SiPlayMode.MutiPlayerClient);
                _gameCore.Menus.Insert(new MpMenuClientJoinLobby(_gameCore));
            }
            else if (item.Key == "HOST")
            {
                _gameCore.Multiplay.SetPlayMode(SiPlayMode.MutiPlayerHost);
                _gameCore.Menus.Insert(new MpMenuHostCreateLobby(_gameCore));
            }

            return true;
        }
    }
}
