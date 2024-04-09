using Newtonsoft.Json;
using Si.Engine.EngineLibrary;
using Si.Engine.Interrogation._Superclass;
using Si.Engine.Manager;
using Si.Engine.Menu;
using Si.Engine.Sprite._Superclass;
using Si.Engine.TickController.PlayerSpriteTickController;
using Si.Engine.TickController.UnvectoredTickController;
using Si.GameEngine.AI._Superclass;
using Si.Library;
using Si.Library.Mathematics;
using Si.Rendering;
using System;
using System.Drawing;
using System.Windows.Forms;
using static Si.Library.SiConstants;

namespace Si.Engine
{
    /// <summary>
    /// The core game engine. Containd the controllers and managers.
    /// </summary>
    public class EngineCore
    {
        #region Backend variables.

        private readonly EngineWorldClock _worldClock;

        #endregion

        #region Public properties.

        public SiEngineInitilizationType ExecutionType { get; private set; }

        public bool IsRunning { get; private set; } = false;

        #endregion

        #region Managers. 

        public InputManager Input { get; private set; }
        public DisplayManager Display { get; private set; }
        public SpriteManager Sprites { get; private set; } //Also contains all of the sprite tick controllers.
        public AudioManager Audio { get; private set; }
        public AssetManager Assets { get; private set; }
        public InterrogationManager Debug { get; private set; }
        public CollisionManager Collisions { get; private set; }

        #endregion

        #region Tick Controllers.

        public SituationTickController Situations { get; private set; }
        public EventTickController Events { get; private set; }
        public PlayerSpriteTickController Player { get; private set; }
        public MenuTickController Menus { get; private set; }
        public SiRendering Rendering { get; private set; }
        public SiEngineSettings Settings { get; private set; }

        #endregion

        #region Events.

        public delegate void InitializationEvent(EngineCore engine);
        public event InitializationEvent OnInitialization;

        public delegate void ShutdownEvent(EngineCore engine);
        public event ShutdownEvent OnShutdown;

        #endregion

        /// <summary>
        /// Initializes a new instace of the game engine.
        /// </summary>
        /// <param name="drawingSurface">The window that the game will be rendered to.</param>
        public EngineCore(Control drawingSurface, SiEngineInitilizationType executionType)
        {
            ExecutionType = executionType;

            Settings = LoadSettings();

            Display = new DisplayManager(this, drawingSurface);
            Rendering = new SiRendering(Settings, drawingSurface, Display.TotalCanvasSize);
            Assets = new AssetManager(this);
            Events = new EventTickController(this);
            Sprites = new SpriteManager(this);
            Input = new InputManager(this);
            Collisions = new CollisionManager(this);

            Situations = new SituationTickController(this);
            Audio = new AudioManager(this);
            Menus = new MenuTickController(this);
            Player = new PlayerSpriteTickController(this);

            _worldClock = new EngineWorldClock(this);
        }

        public void EnableDebugging(IInterrogationForm debugForm)
        {
            Debug = new InterrogationManager(this, debugForm);
        }

        public static SiEngineSettings LoadSettings()
        {
            var engineSettingsText = AssetManager.GetUserText("Engine.Settings.json");

            if (string.IsNullOrEmpty(engineSettingsText))
            {
                var defaultSettings = new SiEngineSettings();

                int x = (int)(Screen.PrimaryScreen.Bounds.Width * 0.75);
                int y = (int)(Screen.PrimaryScreen.Bounds.Height * 0.75);
                if (x % 2 != 0) x++;
                if (y % 2 != 0) y++;
                defaultSettings.Resolution = new Size(x, y);

                engineSettingsText = JsonConvert.SerializeObject(defaultSettings, Formatting.Indented);
                AssetManager.PutUserText("Engine.Settings.json", engineSettingsText);
            }

            return JsonConvert.DeserializeObject<SiEngineSettings>(engineSettingsText);
        }

        public static void SaveSettings(SiEngineSettings settings)
        {
            AssetManager.PutUserText("Engine.Settings.json", JsonConvert.SerializeObject(settings, Formatting.Indented));
        }

        public void ResetGame()
        {
            Sprites.TextBlocks.PlayerStatsText.Visable = false;
            Situations.End();
            Sprites.QueueDeletionOfActionSprites();
        }

        public void StartGame()
        {
            Sprites.QueueDeletionOfActionSprites();
            Situations.AdvanceLevel();
        }

        public void RenderEverything()
        {
            try
            {
                Rendering.RenderTargets.Use((NTDLS.Semaphore.PessimisticCriticalResource<SiCriticalRenderTargets>.CriticalResourceDelegateWithVoidResult)(o =>
                {
                    o.ScreenRenderTarget.BeginDraw();
                    o.IntermediateRenderTarget.BeginDraw();

                    if (ExecutionType == SiEngineInitilizationType.Play)
                    {
                        o.IntermediateRenderTarget.Clear(Rendering.Materials.Colors.Black);
                    }
                    else
                    {
                        o.IntermediateRenderTarget.Clear(Rendering.Materials.Colors.EditorBackground);
                    }

                    Sprites.RenderPreScaling(o.IntermediateRenderTarget);

                    #region Render Collisions.

                    if (Settings.HighlightCollisions)
                    {
                        foreach (var collision in Collisions.Detected)
                        {
                            Rendering.DrawRectangleAt(o.IntermediateRenderTarget,
                                -Display.RenderWindowPosition.X, -Display.RenderWindowPosition.Y,
                                collision.Value.OverlapRectangle.ToRawRectangleF(),
                                0, Rendering.Materials.Colors.Orange, 1, 2);

                            Rendering.DrawPolygon(o.IntermediateRenderTarget, -Display.RenderWindowPosition.X, -Display.RenderWindowPosition.Y,
                                collision.Value.OverlapPolygon,
                                Rendering.Materials.Colors.Cyan, 3);

                            Rendering.DrawRectangleAt((SharpDX.Direct2D1.RenderTarget)o.IntermediateRenderTarget,
                                collision.Value.Body1.RawRenderBounds,
                                collision.Value.Body1.Direction.RadiansSigned,
                                Rendering.Materials.Colors.Red, (float)1, (float)1);

                            Rendering.DrawRectangleAt((SharpDX.Direct2D1.RenderTarget)o.IntermediateRenderTarget,
                                collision.Value.Body2.RawRenderBounds,
                                collision.Value.Body2.Direction.RadiansSigned,
                                Rendering.Materials.Colors.LawnGreen, (float)1, (float)1);
                        }
                    }
                    #endregion


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
                }));
            }
            catch
            {
            }
        }

        public void StartEngine()
        {
            if (IsRunning)
            {
                throw new Exception("The game engine is already running.");
            }

            IsRunning = true;
            //Sprites.ResetPlayer();
            _worldClock.Start();

            /*
            var loadingSprite = Sprites.GenericSprites.CreateFromImagePath(@"Sprites\Loading.png");
            loadingSprite.CenterInUniverse();

            var loadingEvent = Events.Add(10, (sender, parameter) =>
            {
                loadingSprite.Velocity.ForwardAngle.Degrees -= 0.1f;
            }, SiDefermentEventMode.Recurring );
            */

            var textBlock = Sprites.TextBlocks.Add(Rendering.TextFormats.Loading,
                Rendering.Materials.Brushes.Red, new SiVector(100, 100), true);

            textBlock.SetTextAndCenterXY("Building cache...");

            var percentTextBlock = Sprites.TextBlocks.Add(Rendering.TextFormats.Loading,
                Rendering.Materials.Brushes.Red, new SiVector(textBlock.X, textBlock.Y + 50), true);

            textBlock.SetTextAndCenterXY("Building reflection cache...");
            SiReflection.BuildReflectionCacheOfType<SpriteBase>();
            SiReflection.BuildReflectionCacheOfType<AIStateMachine>();

            if (Settings.PreCacheAllAssets)
            {
                textBlock.SetTextAndCenterXY("Building asset cache...");
                Assets.PreCacheAllAssets(percentTextBlock);
                //loadingSprite.QueueForDelete();
                //loadingEvent.QueueForDeletion();
            }

            textBlock.QueueForDelete();
            percentTextBlock.QueueForDelete();

            OnInitialization?.Invoke(this);

            if (ExecutionType == SiEngineInitilizationType.Play)
            {
                if (Settings.PlayMusic)
                {
                    Audio.BackgroundMusicSound.Play();
                }

                Sprites.SkyBoxes.AddAtCenterUniverse();

                Events.Add(1, () => Menus.Show(new MenuStartNewGame(this)));
            }
        }

        public void ShutdownEngine()
        {
            if (IsRunning)
            {
                IsRunning = false;

                OnShutdown?.Invoke(this);

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
