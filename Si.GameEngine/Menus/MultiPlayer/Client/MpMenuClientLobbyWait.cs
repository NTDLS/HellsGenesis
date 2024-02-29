using Si.GameEngine.Menus._Superclass;
using Si.GameEngine.Sprites.MenuItems;
using Si.Library.Mathematics.Geometry;
using Si.Menus.SinglePlayer;
using System.Drawing;
using System.Timers;
using static Si.Library.SiConstants;

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

        public MpMenuClientLobbyWait(GameEngine.GameEngineCore gameEngine)
            : base(gameEngine)
        {
            _gameEngine.Multiplay.SetPlayMode(SiPlayMode.MutiPlayerClient);

            _currentScaledScreenBounds = _gameEngine.Display.GetCurrentScaledScreenBounds();

            float offsetX = _gameEngine.Display.TotalCanvasSize.Width / 2;
            float offsetY = _currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new SiVector(offsetX, offsetY), "Waiting in Lobby");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;
            itemTitle.IsHighlighted = true;

            _countOfReadyPlayers = CreateAndAddTextblock(new SiVector(offsetX, offsetY), "?");
            _countOfReadyPlayers.X -= _countOfReadyPlayers.Size.Width / 2;

            offsetY += _countOfReadyPlayers.Size.Height + 10;

            _countdownToAutoStart = CreateAndAddTextblock(new SiVector(offsetX, offsetY), "");
            _countdownToAutoStart.X -= _countdownToAutoStart.Size.Width / 2;

            offsetY += _countdownToAutoStart.Size.Height + 10;

            OnCleanup += MpMenuClientLobbyWait_OnCleanup;
            OnEscape += MpMenuClientLobbyWait_OnEscape;

            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();

            _gameEngine.Multiplay.OnHostIsStartingGame += Multiplay_OnHostIsStartingGame;

            _gameEngine.Multiplay.SetWaitingInLobby();
        }

        private void Multiplay_OnHostIsStartingGame()
        {
            Close();
            _gameEngine.StartGame();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _gameEngine.Multiplay.GetLobbyInfo(_gameEngine.Multiplay.State.LobbyUID).ContinueWith(o =>
            {
                _countOfReadyPlayers.Text = $"Players: {o.Result.WaitingCount:n0}";
                _countOfReadyPlayers.X = (_gameEngine.Display.TotalCanvasSize.Width / 2) - (_countOfReadyPlayers.Size.Width / 2);

                if (o.Result.RemainingSecondsUntilAutoStart != null)
                {
                    _countdownToAutoStart.Text = $"Auto-starting in {o.Result.RemainingSecondsUntilAutoStart:n0}s.";
                    _countdownToAutoStart.X = (_gameEngine.Display.TotalCanvasSize.Width / 2) - (_countdownToAutoStart.Size.Width / 2);
                }
                else
                {
                    _countdownToAutoStart.Text = "";
                }
            });
        }

        private void MpMenuClientLobbyWait_OnCleanup()
        {
            _gameEngine.Multiplay.OnHostIsStartingGame -= Multiplay_OnHostIsStartingGame;
            _timer.Stop();
            _timer.Dispose();
        }

        private bool MpMenuClientLobbyWait_OnEscape()
        {
            _gameEngine.Multiplay.SetLeftLobby();
            _gameEngine.Menus.Add(new MpMenuClientSelectLoadout(_gameEngine));
            return true;
        }
    }
}
