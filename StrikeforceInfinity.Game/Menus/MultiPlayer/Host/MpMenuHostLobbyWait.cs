using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Menus.BasesAndInterfaces;
using StrikeforceInfinity.Game.Sprites.MenuItems;
using System.Timers;

namespace StrikeforceInfinity.Menus.MultiPlayer.Host
{
    /// <summary>
    /// The menu that the host owner waits at for clients to join.
    /// </summary>
    internal class MpMenuHostLobbyWait : MenuBase
    {
        private Timer _timer = new(1000);

        SpriteMenuItem _countOfReadyPlayers;

        public MpMenuHostLobbyWait(EngineCore gameCore)
            : base(gameCore)
        {
            var currentScaledScreenBounds = _gameCore.Display.GetCurrentScaledScreenBounds();

            double offsetX = _gameCore.Display.TotalCanvasSize.Width / 2;
            double offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new SiPoint(offsetX, offsetY), "Host Lobby");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;
            itemTitle.Highlight = true;

            _countOfReadyPlayers = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "?");
            _countOfReadyPlayers.X = offsetX + 300;
            _countOfReadyPlayers.Y = offsetY - _countOfReadyPlayers.Size.Height;

            var helpItem = CreateAndAddSelectableItem(new SiPoint(offsetX, offsetY), "START_NOW", " Start Now ");
            helpItem.Selected = true;
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            /*
            var gameHosts = _gameCore.Multiplay.GetHostList();
            foreach (var gameHost in gameHosts)
            {
                helpItem = CreateAndAddSelectableItem(new SiPoint(offsetX, offsetY), gameHost.UID.ToString(), gameHost.Name);
                helpItem.X -= helpItem.Size.Width / 2;
                offsetY += helpItem.Size.Height + 5;
            }
            */

            OnExecuteSelection += MenuMultiplayerHostOrJoin_OnExecuteSelection;
            OnCleanup += MpMenuHostLobbyWait_OnCleanup;

            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();

            _gameCore.Multiplay.SetWaitingInLobby();
        }

        private void MpMenuHostLobbyWait_OnCleanup()
        {
            _timer.Stop();
            _timer.Dispose();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _gameCore.Multiplay.GetLobbyInfo(_gameCore.Multiplay.LobbyUID).ContinueWith(o =>
            {
                _countOfReadyPlayers.Text = $"{o.Result.WaitingCount:n0}";
            });
        }

        private void MenuMultiplayerHostOrJoin_OnExecuteSelection(SpriteMenuItem item)
        {
            _gameCore.StartGame();
        }
    }
}
