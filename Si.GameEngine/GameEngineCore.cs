using Newtonsoft.Json;
using Si.GameEngine.Core.Types;
using Si.GameEngine.Interrogation._Superclass;
using Si.GameEngine.Managers;
using Si.GameEngine.Menus;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.TickControllers;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using Si.MultiplayClient;
using Si.Rendering;
using System.Drawing;
using System.Windows.Forms;

namespace Si.GameEngine
{
    /// <summary>
    /// The core game engine. Containd the controllers and managers.
    /// </summary>
    public class GameEngineCore
    {
        /// <summary>
        /// If TRUE the game can run "headless" with no diplay or audio.
        /// </summary>
        public readonly bool _isRunningHeadless;
        public bool IsRunningHeadless => _isRunningHeadless;

        public SituationsTickController Situations { get; private set; }
        public EventsTickController Events { get; private set; }
        public PlayerSpriteTickController Player { get; private set; }
        public EngineMultiplayManager Multiplay { get; private set; }
        public EngineInputManager Input { get; private set; }
        public EngineDisplayManager Display { get; private set; }
        public EngineSpriteManager Sprites { get; private set; } //Also contains all of the sprite tick controllers.
        public EngineAudioManager Audio { get; private set; }
        public EngineAssetManager Assets { get; private set; }
        public EngineInterrogationManager Debug { get; private set; }
        public MenusTickController Menus { get; private set; }
        public SiRendering Rendering { get; private set; }
        public SiEngineSettings Settings { get; private set; }

        public bool IsRunning { get; private set; } = false;

        private readonly EngineWorldClock _worldClock;
        private readonly MultiplayClientEventHandlers _multiplayClientEventHandlers;

        private static uint _nextSequentialId = 1;
        private static readonly object _nextSequentialLock = new();
        /// <summary>
        /// Used to give all loaded sprites a unique ID. Very handy for debugging.
        /// </summary>
        /// <returns></returns>
        public static uint GetNextSequentialId()
        {
            lock (_nextSequentialLock)
            {
                return _nextSequentialId++;
            }
        }

        #region Events.

        public delegate void StartEngineEvent(GameEngineCore sender);
        public event StartEngineEvent OnStartEngine;

        public delegate void StopEngineEvent(GameEngineCore sender);
        public event StopEngineEvent OnStopEngine;

        #endregion

        /// <summary>
        /// Initializes a new instace of the game engine in headless mode with no display or audio.
        /// </summary>
        public GameEngineCore(EngineMultiplayManager multiplayManager)
        {
            _isRunningHeadless = true;

            Settings = LoadSettings();

            var drawingSurface = new Control()
            {
                Height = 1080,
                Width = 1920
            };

            Display = new EngineDisplayManager(this, drawingSurface);
            Assets = new EngineAssetManager(this);
            Sprites = new EngineSpriteManager(this);
            Input = new EngineInputManager(this);
            Situations = new SituationsTickController(this);
            Events = new EventsTickController(this);
            Audio = new EngineAudioManager(this);
            Menus = new MenusTickController(this);
            Player = new PlayerSpriteTickController(this);
            Rendering = new SiRendering(Settings, drawingSurface, Display.TotalCanvasSize);

            Multiplay = multiplayManager;

            _multiplayClientEventHandlers = new MultiplayClientEventHandlers(this);
            Multiplay.OnReceivedLevelLayout += _multiplayClientEventHandlers.OnReceivedLevelLayout;
            Multiplay.OnNeedLevelLayout += _multiplayClientEventHandlers.OnNeedLevelLayout;
            Multiplay.OnApplySpriteActions += _multiplayClientEventHandlers.OnApplySpriteActions;
            Multiplay.OnPlayerSpriteCreated += _multiplayClientEventHandlers.OnPlayerSpriteCreated;
            Multiplay.OnHostLevelStarted += _multiplayClientEventHandlers.OnHostLevelStarted;
            Multiplay.OnSpriteCreated += _multiplayClientEventHandlers.OnSpriteCreated;

            _worldClock = new EngineWorldClock(this);
        }

        /// <summary>
        /// Initializes a new instace of the game engine.
        /// </summary>
        /// <param name="drawingSurface">The window that the game will be rendered to.</param>
        public GameEngineCore(Control drawingSurface)
        {
            Settings = LoadSettings();

            Display = new EngineDisplayManager(this, drawingSurface);
            Assets = new EngineAssetManager(this);
            Sprites = new EngineSpriteManager(this);
            Input = new EngineInputManager(this);
            Situations = new SituationsTickController(this);
            Events = new EventsTickController(this);
            Audio = new EngineAudioManager(this);
            Menus = new MenusTickController(this);
            Player = new PlayerSpriteTickController(this);
            Rendering = new SiRendering(Settings, drawingSurface, Display.TotalCanvasSize);
            Multiplay = new EngineMultiplayManager();

            _multiplayClientEventHandlers = new MultiplayClientEventHandlers(this);
            Multiplay.OnReceivedLevelLayout += _multiplayClientEventHandlers.OnReceivedLevelLayout;
            Multiplay.OnNeedLevelLayout += _multiplayClientEventHandlers.OnNeedLevelLayout;
            Multiplay.OnApplySpriteActions += _multiplayClientEventHandlers.OnApplySpriteActions;
            Multiplay.OnPlayerSpriteCreated += _multiplayClientEventHandlers.OnPlayerSpriteCreated;
            Multiplay.OnHostLevelStarted += _multiplayClientEventHandlers.OnHostLevelStarted;
            Multiplay.OnSpriteCreated += _multiplayClientEventHandlers.OnSpriteCreated;

            _worldClock = new EngineWorldClock(this);
        }

        public void EnableDebugging(IInterrogationForm debugForm)
        {
            Debug = new EngineInterrogationManager(this, debugForm);
        }

        public static SiEngineSettings LoadSettings()
        {
            var engineSettingsText = EngineAssetManager.GetUserText("Engine.Settings.json");

            if (string.IsNullOrEmpty(engineSettingsText))
            {
                var defaultSettings = new SiEngineSettings();

                int x = (int)(Screen.PrimaryScreen.Bounds.Width * 0.75);
                int y = (int)(Screen.PrimaryScreen.Bounds.Height * 0.75);
                if (x % 2 != 0) x++;
                if (y % 2 != 0) y++;
                defaultSettings.Resolution = new Size(x, y);

                engineSettingsText = JsonConvert.SerializeObject(defaultSettings, Formatting.Indented);
                EngineAssetManager.PutUserText("Engine.Settings.json", engineSettingsText);
            }

            return JsonConvert.DeserializeObject<SiEngineSettings>(engineSettingsText);
        }

        public void ResetGame()
        {
            Sprites.PlayerStatsText.Visable = false;
            Situations.End();
            Sprites.DeleteAll();
        }

        public void StartGame()
        {
            Sprites.DeleteAll();

            switch (Multiplay.State.PlayMode)
            {
                case SiConstants.SiPlayMode.SinglePlayer:
                    Situations.AdvanceLevel();
                    break;
                case SiConstants.SiPlayMode.MutiPlayerClient:
                    Player.Show();
                    Multiplay.NotifyPlayerSpriteCreated(Player.Sprite.GetType(), Player.Sprite.MultiplayUID);
                    break;
                case SiConstants.SiPlayMode.MutiPlayerHost:
                    Multiplay.NotifyHostIsStartingGame();
                    Situations.AdvanceLevel();
                    Multiplay.NotifyPlayerSpriteCreated(Player.Sprite.GetType(), Player.Sprite.MultiplayUID);
                    break;
            }
        }

        public static void SaveSettings(SiEngineSettings settings)
        {
            EngineAssetManager.PutUserText("Engine.Settings.json", JsonConvert.SerializeObject(settings, Formatting.Indented));
        }

        public void Render()
        {
            if (!_isRunningHeadless)
            {
                try
                {
                    Rendering.RenderTargets.Use(o =>
                    {
                        o.ScreenRenderTarget.BeginDraw();
                        o.IntermediateRenderTarget.BeginDraw();

                        o.ScreenRenderTarget.Clear(Rendering.Materials.Colors.Black);
                        o.IntermediateRenderTarget.Clear(Rendering.Materials.Colors.Black);

                        Sprites.RenderPreScaling(o.IntermediateRenderTarget);
                        o.IntermediateRenderTarget.EndDraw();

                        if (Settings.EnableSpeedScaleFactoring)
                        {
                            Rendering.TransferWithZoom(o.IntermediateRenderTarget, o.ScreenRenderTarget, (float)Display.SpeedOrientedFrameScalingFactor());
                        }
                        else
                        {
                            Rendering.TransferWithZoom(o.IntermediateRenderTarget, o.ScreenRenderTarget, (float)Display.BaseDrawScale);
                        }
                        Sprites.RenderPostScaling(o.ScreenRenderTarget);

                        o.ScreenRenderTarget.EndDraw();
                    });
                }
                catch
                {
                }
            }
        }

        private void NewGameMenuCallback(SiEngineCallbackEvent sender, object refObj)
        {
            Menus.Add(new MenuStartNewGame(this));
        }

        public void StartEngine()
        {
            if (IsRunning == false)
            {
                IsRunning = true;
                Sprites.Start();
                //Sprites.ResetPlayer();
                _worldClock.Start();

                var textBlock = Sprites.TextBlocks.Create(Rendering.TextFormats.Loading,
                    Rendering.Materials.Brushes.Red, new SiVector(100, 100), true);

                textBlock.SetTextAndCenterXY("Building cache...");

                var percentTextBlock = Sprites.TextBlocks.Create(Rendering.TextFormats.Loading,
                    Rendering.Materials.Brushes.Red, new SiVector(textBlock.X, textBlock.Y + 50), true);

                textBlock.SetTextAndCenterXY("Building reflection cache...");
                SiReflection.BuildReflectionCacheOfType<SpriteBase>();

                if (Settings.PreCacheAllAssets)
                {
                    textBlock.SetTextAndCenterXY("Building asset cache...");
                    Assets.PreCacheAllAssets(percentTextBlock);
                }

                textBlock.QueueForDelete();
                percentTextBlock.QueueForDelete();

                OnStartEngine?.Invoke(this);

                if (Settings.PlayMusic)
                {
                    Audio.BackgroundMusicSound.Play();
                }

                Events.Create(1, NewGameMenuCallback);
            }
        }

        public void ShutdownEngine()
        {
            if (IsRunning)
            {
                IsRunning = false;

                OnStopEngine?.Invoke(this);

                Multiplay.Dispose();
                _worldClock.Dispose();
                Sprites.Dispose();
                Rendering.Dispose();
                Assets.Dispose();
            }
        }

        public bool IsPaused() => _worldClock.IsPaused();
        public void TogglePause() => _worldClock.TogglePause();
        public void Pause() => _worldClock.Pause();
        public void Resume() => _worldClock.Resume();
    }
}
