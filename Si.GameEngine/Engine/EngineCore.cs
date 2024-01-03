using Newtonsoft.Json;
using Si.GameEngine.Controller;
using Si.GameEngine.Engine.Debug.BasesAndInterfaces;
using Si.GameEngine.Engine.GraphicsProcessing;
using Si.GameEngine.Engine.Types;
using Si.GameEngine.Managers;
using Si.GameEngine.Menus;
using Si.Multiplay;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Si.GameEngine.Engine
{
    /// <summary>
    /// The core game engine. Containd the controllers and managers.
    /// </summary>
    public class EngineCore
    {
        /// <summary>
        /// If TRUE the game can run "headless" with no diplay or audio.
        /// </summary>
        public bool IsRunningHeadless { get; private set; }

        public SituationsTickController Situations { get; private set; }
        public EventsTickController Events { get; private set; }
        public PlayerSpriteTickController Player { get; private set; }
        public EngineMultiplayManager Multiplay { get; private set; }
        public EngineInputManager Input { get; private set; }
        public EngineDisplayManager Display { get; private set; }
        public EngineSpriteManager Sprites { get; private set; } //Also contains all of the sprite tick controllers.
        public EngineAudioManager Audio { get; private set; }
        public EngineAssetManager Assets { get; private set; }
        public EngineDebugManager Debug { get; private set; }
        public MenusTickController Menus { get; private set; }
        public EngineRendering Rendering { get; private set; }
        public EngineSettings Settings { get; private set; }

        public bool IsRunning { get; private set; } = false;

        private readonly EngineWorldClock _worldClock;

        static uint _nextSequentialId = 1;
        static readonly object _nextSequentialLock = new();
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

        public delegate void StartEngineEvent(EngineCore sender);
        public event StartEngineEvent OnStartEngine;

        public delegate void StopEngineEvent(EngineCore sender);
        public event StopEngineEvent OnStopEngine;

        #endregion

        /// <summary>
        /// Initializes a new instace of the game engine in headless mode with no display or audio.
        /// </summary>
        public EngineCore(EngineMultiplayManager multiplayManager)
        {
            IsRunningHeadless = true;

            Settings = LoadSettings();

            var drawingSurface = new Control()
            {
                Height = 1080,
                Width = 1920
            };

            Display = new EngineDisplayManager(this, drawingSurface, new Size(drawingSurface.Width, drawingSurface.Height));
            Assets = new EngineAssetManager(this);
            Sprites = new EngineSpriteManager(this);
            Input = new EngineInputManager(this);
            Situations = new SituationsTickController(this);
            Events = new EventsTickController(this);
            Audio = new EngineAudioManager(this);
            Menus = new MenusTickController(this);
            Player = new PlayerSpriteTickController(this);
            Rendering = new EngineRendering(this);

            Multiplay = multiplayManager;
            Multiplay.OnReceivedSituationLayout += Sprites.MultiplayApplySituationLayout;
            Multiplay.OnNeedSituationLayout += Sprites.MultiplayGetSituationLayout;
            Multiplay.OnApplySpriteActions += Sprites.MultiplayApplySpriteActions;
            Multiplay.OnPlayerSpriteCreated += Sprites.MultiplayPlayerSpriteCreated;

            _worldClock = new EngineWorldClock(this);

            Events.Create(new TimeSpan(0, 0, 0, 1), NewGameMenuCallback);
        }

        /// <summary>
        /// Initializes a new instace of the game engine.
        /// </summary>
        /// <param name="drawingSurface">The window that the game will be rendered to.</param>
        public EngineCore(Control drawingSurface)
        {
            Settings = LoadSettings();

            Display = new EngineDisplayManager(this, drawingSurface, new Size(drawingSurface.Width, drawingSurface.Height));
            Assets = new EngineAssetManager(this);
            Sprites = new EngineSpriteManager(this);
            Input = new EngineInputManager(this);
            Situations = new SituationsTickController(this);
            Events = new EventsTickController(this);
            Audio = new EngineAudioManager(this);
            Menus = new MenusTickController(this);
            Player = new PlayerSpriteTickController(this);
            Rendering = new EngineRendering(this);

            Multiplay = new EngineMultiplayManager();
            Multiplay.OnReceivedSituationLayout += Sprites.MultiplayApplySituationLayout;
            Multiplay.OnNeedSituationLayout += Sprites.MultiplayGetSituationLayout;
            Multiplay.OnApplySpriteActions += Sprites.MultiplayApplySpriteActions;
            Multiplay.OnPlayerSpriteCreated += Sprites.MultiplayPlayerSpriteCreated;
            Multiplay.OnLevelStarted += Sprites.MultiplayLevelStarted;

            _worldClock = new EngineWorldClock(this);

            Events.Create(new TimeSpan(0, 0, 0, 1), NewGameMenuCallback);
        }

        public void EnableDebugging(IDebugForm debugForm)
        {
            Debug = new EngineDebugManager(this, debugForm);
        }

        public static EngineSettings LoadSettings()
        {
            var engineSettingsText = EngineAssetManager.GetUserText("Engine.Settings.json");

            if (string.IsNullOrEmpty(engineSettingsText))
            {
                engineSettingsText = JsonConvert.SerializeObject(new EngineSettings(), Formatting.Indented);
                EngineAssetManager.PutUserText("Engine.Settings.json", engineSettingsText);
            }

            return JsonConvert.DeserializeObject<EngineSettings>(engineSettingsText);
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
                case Shared.SiConstants.SiPlayMode.SinglePlayer:
                    Situations.AdvanceLevel();
                    break;
                case Shared.SiConstants.SiPlayMode.MutiPlayerClient:
                    Player.Show();
                    Multiplay.NotifyPlayerSpriteCreated(Player.Sprite.GetType(), Player.Sprite.MultiplayUID);
                    break;
                case Shared.SiConstants.SiPlayMode.MutiPlayerHost:
                    Multiplay.NotifyHostIsStartingGame();
                    Situations.AdvanceLevel();
                    Multiplay.NotifyPlayerSpriteCreated(Player.Sprite.GetType(), Player.Sprite.MultiplayUID);
                    break;
            }
        }

        public static void SaveSettings(EngineSettings settings)
        {
            EngineAssetManager.PutUserText("Engine.Settings.json", JsonConvert.SerializeObject(settings, Formatting.Indented));
        }

        public void Render()
        {
            if (!IsRunningHeadless)
            {
                try
                {
                    Rendering.ScreenRenderTarget.BeginDraw();
                    Rendering.IntermediateRenderTarget.BeginDraw();

                    Rendering.ScreenRenderTarget.Clear(Rendering.Materials.Raw.Black);

                    Rendering.IntermediateRenderTarget.Clear(Rendering.Materials.Raw.Black);
                    Sprites.RenderPreScaling(Rendering.IntermediateRenderTarget);
                    Rendering.IntermediateRenderTarget.EndDraw();

                    if (Settings.AutoZoomWhenMoving)
                    {
                        Rendering.ApplyScaling((float)Display.SpeedOrientedFrameScalingFactor());
                    }
                    else
                    {
                        Rendering.ApplyScaling((float)Display.BaseDrawScale);
                    }
                    Sprites.RenderPostScaling(Rendering.ScreenRenderTarget);

                    Rendering.ScreenRenderTarget.EndDraw();
                }
                catch
                {
                }
            }
        }

        private void NewGameMenuCallback(EngineCore gameCore, SiEngineCallbackEvent sender, object refObj)
        {
            Menus.Insert(new MenuStartNewGame(this));
        }

        public void StartEngine()
        {
            if (IsRunning == false)
            {
                IsRunning = true;
                Sprites.Start();
                //Sprites.ResetPlayer();

                _worldClock.Start();

                OnStartEngine?.Invoke(this);
            }
        }

        public void ShutdownEngine()
        {
            if (IsRunning)
            {
                Multiplay.Shutdown();
                IsRunning = false;
                _worldClock.Shutdown();
                Sprites.Shutdown();
                OnStopEngine?.Invoke(this);
                Rendering.Cleanup();
            }
        }

        public bool IsPaused() => _worldClock.IsPaused();
        public void TogglePause() => _worldClock.TogglePause();
        public void Pause() => _worldClock.Pause();
        public void Resume() => _worldClock.Resume();
    }
}
