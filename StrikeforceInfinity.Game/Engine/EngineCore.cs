using Newtonsoft.Json;
using StrikeforceInfinity.Game.Controller;
using StrikeforceInfinity.Game.Engine.GraphicsProcessing;
using StrikeforceInfinity.Game.Engine.Types;
using StrikeforceInfinity.Game.Managers;
using StrikeforceInfinity.Game.Menus;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace StrikeforceInfinity.Game.Engine
{
    /// <summary>
    /// The core game engine. Containd the controllers and managers.
    /// </summary>
    internal class EngineCore : IDisposable
    {
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
            Debug = new EngineDebugManager(this);
            Multiplay = new EngineMultiplayManager(this);

            _worldClock = new EngineWorldClock(this);

            Events.Create(new System.TimeSpan(0, 0, 0, 1), NewGameMenuCallback);
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
            Sprites.PlayerStatsText.Visable = true;
            Sprites.DeleteAll();
            Situations.AdvanceLevel();
        }

        public static void SaveSettings(EngineSettings settings)
        {
            EngineAssetManager.PutUserText("Engine.Settings.json", JsonConvert.SerializeObject(settings, Formatting.Indented));
        }

        public void Render()
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

        public void StopEngine()
        {
            if (IsRunning)
            {
                IsRunning = false;
                _worldClock.Stop();
                Sprites.Stop();
                OnStopEngine?.Invoke(this);
                Rendering.Cleanup();
            }
        }

        public bool IsPaused() => _worldClock.IsPaused();
        public void TogglePause() => _worldClock.TogglePause();
        public void Pause() => _worldClock.Pause();
        public void Resume() => _worldClock.Resume();

        public void Dispose()
        {
            Multiplay.Dispose();
        }
    }
}
