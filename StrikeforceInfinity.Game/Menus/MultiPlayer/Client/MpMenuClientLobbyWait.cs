using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Menus.BaseClasses;

namespace StrikeforceInfinity.Menus.MultiPlayer.Client
{
    /// <summary>
    /// The menu that clients wait at for the host onwer to start the game.
    /// </summary>
    internal class MpMenuClientLobbyWait : MenuBase
    {
        public MpMenuClientLobbyWait(EngineCore gameCore)
            : base(gameCore)
        {
            var currentScaledScreenBounds = _gameCore.Display.GetCurrentScaledScreenBounds();

            double offsetX = _gameCore.Display.TotalCanvasSize.Width / 2;
            double offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new SiPoint(offsetX, offsetY), "Waiting on Game to Start");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;
            itemTitle.Highlight = true;

            var helpItem = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "Name");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            _gameCore.Multiplay.SetWaitingInLobby();

            /*
            var gameHosts = _gameCore.Multiplay.GetHostList();
            foreach (var gameHost in gameHosts)
            {
                helpItem = CreateAndAddSelectableItem(new SiPoint(offsetX, offsetY), gameHost.UID.ToString(), gameHost.Name);
                helpItem.X -= helpItem.Size.Width / 2;
                offsetY += helpItem.Size.Height + 5;
            }

            var firstItem = VisibleSelectableItems().FirstOrDefault();
            if (firstItem != null)
            {
                firstItem.Selected = true;
            }

            OnExecuteSelection += MenuMultiplayerHostOrJoin_OnExecuteSelection;
            */
        }

        /*
        private void MenuMultiplayerHostOrJoin_OnExecuteSelection(SpriteMenuItem item)
        {
        }
        */
    }
}
