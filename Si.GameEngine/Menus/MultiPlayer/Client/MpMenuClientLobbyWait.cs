using Si.GameEngine.Engine;
using Si.GameEngine.Menus.BasesAndInterfaces;
using Si.Menus.SinglePlayer;
using Si.Shared.Types.Geometry;

namespace Si.Menus.MultiPlayer.Client
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

            OnEscape += MpMenuClientLobbyWait_OnEscape;

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

        private bool MpMenuClientLobbyWait_OnEscape()
        {
            _gameCore.Multiplay.SetLeftLobby();
            _gameCore.Menus.Insert(new MpMenuClientSelectLoadout(_gameCore));
            return true;
        }

        /*
        private bool MenuMultiplayerHostOrJoin_OnExecuteSelection(SpriteMenuItem item)
        {
            return true;
        }
        */
    }
}
