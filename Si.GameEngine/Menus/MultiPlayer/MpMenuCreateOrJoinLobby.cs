using Si.GameEngine.Engine;
using Si.GameEngine.Menus;
using Si.GameEngine.Menus.BasesAndInterfaces;
using Si.GameEngine.Sprites.MenuItems;
using Si.Menus.MultiPlayer.Client;
using Si.Menus.MultiPlayer.Host;
using Si.Shared.Types.Geometry;
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
            //Set this here incase we are comming back to this menue from the join/host menu.
            _gameCore.Multiplay.SetPlayMode(SiPlayMode.SinglePlayer);

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
            _gameCore.Menus.Add(new MenuStartNewGame(_gameCore));
            return true;
        }

        private bool MenuMultiplayerHostOrJoin_OnExecuteSelection(SpriteMenuItem item)
        {
            if (item.Key == "JOIN")
            {
                _gameCore.Multiplay.SetPlayMode(SiPlayMode.MutiPlayerClient);
                _gameCore.Menus.Add(new MpMenuClientJoinLobby(_gameCore));
            }
            else if (item.Key == "HOST")
            {
                _gameCore.Multiplay.SetPlayMode(SiPlayMode.MutiPlayerHost);
                _gameCore.Menus.Add(new MpMenuHostCreateLobby(_gameCore));
            }

            return true;
        }
    }
}
