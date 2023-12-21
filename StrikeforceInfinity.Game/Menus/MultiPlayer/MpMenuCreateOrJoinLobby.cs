using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Menus;
using StrikeforceInfinity.Game.Menus.BasesAndInterfaces;
using StrikeforceInfinity.Game.Sprites.MenuItems;
using StrikeforceInfinity.Menus.MultiPlayer.Client;
using StrikeforceInfinity.Menus.MultiPlayer.Host;

namespace StrikeforceInfinity.Menus.MultiPlayer
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

        private void MenuMultiplayerHostOrJoin_OnEscape()
        {
            QueueForDelete();
            _gameCore.Menus.Insert(new MenuStartNewGame(_gameCore));
        }

        private void MenuMultiplayerHostOrJoin_OnExecuteSelection(SpriteMenuItem item)
        {
            if (item.Key == "JOIN")
            {
                _gameCore.Multiplay.SetPlayMode(HgPlayMode.MutiPlayerClient);
                _gameCore.Menus.Insert(new MpMenuClientJoinLobby(_gameCore));
            }
            else if (item.Key == "HOST")
            {
                _gameCore.Multiplay.SetPlayMode(HgPlayMode.MutiPlayerHost);
                _gameCore.Menus.Insert(new MpMenuHostCreateLobby(_gameCore));
            }
        }
    }
}
