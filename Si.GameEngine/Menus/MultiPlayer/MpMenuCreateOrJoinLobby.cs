using Si.GameEngine.Menus;
using Si.GameEngine.Menus._Superclass;
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
        public MpMenuCreateOrJoinLobby(GameEngine.Core.Engine gameEngine)
            : base(gameEngine)
        {
            //Set this here incase we are comming back to this menue from the join/host menu.
            _gameEngine.Multiplay.SetPlayMode(SiPlayMode.SinglePlayer);

            var currentScaledScreenBounds = _gameEngine.Display.GetCurrentScaledScreenBounds();

            double offsetX = _gameEngine.Display.TotalCanvasSize.Width / 2;
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
            _gameEngine.Menus.Add(new MenuStartNewGame(_gameEngine));
            return true;
        }

        private bool MenuMultiplayerHostOrJoin_OnExecuteSelection(SpriteMenuItem item)
        {
            if (item.Key == "JOIN")
            {
                _gameEngine.Multiplay.SetPlayMode(SiPlayMode.MutiPlayerClient);
                _gameEngine.Menus.Add(new MpMenuClientJoinLobby(_gameEngine));
            }
            else if (item.Key == "HOST")
            {
                _gameEngine.Multiplay.SetPlayMode(SiPlayMode.MutiPlayerHost);
                _gameEngine.Menus.Add(new MpMenuHostCreateLobby(_gameEngine));
            }

            return true;
        }
    }
}
