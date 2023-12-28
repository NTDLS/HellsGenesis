using Si.GameEngine.Engine;
using Si.GameEngine.Menus.BasesAndInterfaces;
using Si.GameEngine.Sprites.MenuItems;
using Si.Menus.SinglePlayer;
using Si.Shared.Types.Geometry;
using System.Drawing;
using System.Timers;

namespace Si.Menus.MultiPlayer.Client
{
    /// <summary>
    /// The menu that clients wait at for the host onwer to start the game.
    /// </summary>
    internal class MpMenuClientLobbyWait : MenuBase
    {
        private readonly Timer _timer = new(1000);
        private readonly SpriteMenuItem _countdownToAutoStart;
        private readonly SpriteMenuItem _countOfReadyPlayers;
        private readonly RectangleF _currentScaledScreenBounds;

        public MpMenuClientLobbyWait(EngineCore gameCore)
            : base(gameCore)
        {
            _currentScaledScreenBounds = _gameCore.Display.GetCurrentScaledScreenBounds();

            double offsetX = _gameCore.Display.TotalCanvasSize.Width / 2;
            double offsetY = _currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new SiPoint(offsetX, offsetY), "Waiting in Lobby");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;
            itemTitle.Highlight = true;

            _countOfReadyPlayers = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "?");
            _countOfReadyPlayers.X -= _countOfReadyPlayers.Size.Width / 2;

            offsetY += _countOfReadyPlayers.Size.Height + 10;

            _countdownToAutoStart = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "");
            _countdownToAutoStart.X -= _countdownToAutoStart.Size.Width / 2;

            offsetY += _countdownToAutoStart.Size.Height + 10;

            OnCleanup += MpMenuClientLobbyWait_OnCleanup;
            OnEscape += MpMenuClientLobbyWait_OnEscape;

            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();

            _gameCore.Multiplay.OnHostIsStartingGame += Multiplay_OnHostIsStartingGame;

            _gameCore.Multiplay.SetWaitingInLobby();
        }

        private void Multiplay_OnHostIsStartingGame()
        {
            Close();
            _gameCore.StartGame();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _gameCore.Multiplay.GetLobbyInfo(_gameCore.Multiplay.State.LobbyUID).ContinueWith(o =>
            {
                _countOfReadyPlayers.Text = $"Players: {o.Result.WaitingCount:n0}";
                _countOfReadyPlayers.X = (_gameCore.Display.TotalCanvasSize.Width / 2) - (_countOfReadyPlayers.Size.Width / 2);

                if (o.Result.RemainingSecondsUntilAutoStart != null)
                {
                    _countdownToAutoStart.Text = $"Auto-starting in {o.Result.RemainingSecondsUntilAutoStart:n0}s.";
                    _countdownToAutoStart.X = (_gameCore.Display.TotalCanvasSize.Width / 2) - (_countdownToAutoStart.Size.Width / 2);
                }
                else
                {
                    _countdownToAutoStart.Text = "";
                }
            });
        }

        private void MpMenuClientLobbyWait_OnCleanup()
        {
            _gameCore.Multiplay.OnHostIsStartingGame -= Multiplay_OnHostIsStartingGame;
            _timer.Stop();
            _timer.Dispose();
        }

        private bool MpMenuClientLobbyWait_OnEscape()
        {
            _gameCore.Multiplay.SetLeftLobby();
            _gameCore.Menus.Insert(new MpMenuClientSelectLoadout(_gameCore));
            return true;
        }
    }
}
