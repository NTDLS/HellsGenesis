using AI2D.Actors;
using AI2D.Actors.Bullets;
using AI2D.Actors.Enemies;
using AI2D.Actors.PowerUp;
using AI2D.Engine.Managers.EngineActorFactories;
using AI2D.Engine.Menus;
using AI2D.Types;
using AI2D.Weapons;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using static AI2D.Engine.Managers.EngineDrawingCacheManager;

namespace AI2D.Engine.Managers
{
    public class EngineActorManager
    {
        private readonly Core _core;

        #region Actors.
        internal List<ActorBase> Collection { get; private set; } = new List<ActorBase>();
        public ActorPlayer Player { get; private set; }
        public ActorTextBlock PlayerStatsText { get; private set; }
        public ActorTextBlock DebugText { get; private set; }
        public bool RenderRadar { get; set; } = false;

        public EngineActorAnimationFactory AnimationFactory { get; set; }
        public EngineActorBulletFactory BulletFactory { get; set; }
        public EngineActorEventFactory EventFactory { get; set; }
        public EngineActorDebugFactory DebugFactory { get; set; }
        public EngineActorEnemyFactory EnemyFactory { get; set; }
        public EngineActorMenuFactory MenuFactory { get; set; }
        public EngineActorPowerupFactory PowerupFactory { get; set; }
        public EngineActorRadarPositionFactory RadarPositionFactory { get; set; }
        public EngineActorStarFactory StarFactory { get; set; }
        public EngineActorTextBlockFactory TextBlockFactory { get; set; }

        #endregion

        #region Resources.

        public AudioClip BackgroundMusicSound { get; private set; }
        public AudioClip RadarBlipsSound { get; private set; }
        public AudioClip DoorIsAjarSound { get; private set; }
        public AudioClip LockedOnBlip { get; private set; }

        private Dictionary<string, AudioClip> _audioClips { get; set; } = new Dictionary<string, AudioClip>();
        private Dictionary<string, Bitmap> _Bitmaps { get; set; } = new Dictionary<string, Bitmap>();

        //Thread _renderThread = null;

        #endregion

        public EngineActorManager(Core core)
        {
            _core = core;

            AnimationFactory = new EngineActorAnimationFactory(_core, this);
            BulletFactory = new EngineActorBulletFactory(_core, this);
            EventFactory = new EngineActorEventFactory(_core, this);
            DebugFactory = new EngineActorDebugFactory(_core, this);
            EnemyFactory = new EngineActorEnemyFactory(_core, this);
            MenuFactory = new EngineActorMenuFactory(_core, this);
            PowerupFactory = new EngineActorPowerupFactory(_core, this);
            RadarPositionFactory = new EngineActorRadarPositionFactory(_core, this);
            StarFactory = new EngineActorStarFactory(_core, this);
            TextBlockFactory = new EngineActorTextBlockFactory(_core, this);
        }

        public void Start()
        {
            Player = new ActorPlayer(_core, Constants.PlayerClass.Atlant) { Visable = false };

            DoorIsAjarSound = GetSoundCached(@"..\..\..\Assets\Sounds\Ship\Door Is Ajar.wav", 0.50f, false);
            RadarBlipsSound = GetSoundCached(@"..\..\..\Assets\Sounds\Ship\Radar Blips.wav", 0.20f, false);
            LockedOnBlip = GetSoundCached(@"..\..\..\Assets\Sounds\Ship\Locked On.wav", 0.20f, false);

            BackgroundMusicSound = GetSoundCached(@"..\..\..\Assets\Sounds\Music\Background.wav", 0.25f, true);

            PlayerStatsText = TextBlockFactory.Create("Consolas", Brushes.WhiteSmoke, 9,
                //new Point<double>((_core.Display.OverdrawSize.Width) / 2 + 5, (_core.Display.OverdrawSize.Height / 2) + 5), true);
                new Point<double>(5, 5), true);
            PlayerStatsText.Visable = false;
            DebugText = TextBlockFactory.Create("Consolas", Brushes.Aqua, 10, new Point<double>(5, PlayerStatsText.Y + 80), true);

            BackgroundMusicSound.Play();

            //_renderThread = new Thread(RenderThreadProc);
            //_renderThread.Start();
        }

        public void Stop()
        {

        }

        public void CleanupDeletedObjects()
        {
            _core.Actors.Collection.Where(o => o.ReadyForDeletion).ToList().ForEach(p => p.Cleanup());
            _core.Actors.Collection.RemoveAll(o => o.ReadyForDeletion);

            for (int i = 0; i < EventFactory.Collection.Count; i++)
            {
                if (EventFactory.Collection[i].ReadyForDeletion)
                {
                    EventFactory.Delete(EventFactory.Collection[i]);
                }
            }

            for (int i = 0; i < MenuFactory.Collection.Count; i++)
            {
                if (MenuFactory.Collection[i].ReadyForDeletion)
                {
                    MenuFactory.Delete(MenuFactory.Collection[i]);
                }
            }

            if (_core.Actors.Player.IsDead)
            {
                _core.Actors.Player.Visable = false;
                _core.Actors.Player.IsDead = false;
                MenuFactory.Insert(new MenuStartNewGame(_core));
            }
        }

        public void NewGame()
        {
            lock (Collection)
            {
                _core.Situations.Reset();
                PlayerStatsText.Visable = true;
                DeleteAllActors();

                _core.Situations.AdvanceSituation();
            }
        }

        public void DeleteAllActors()
        {
            DeleteAllPowerUps();
            DeleteAllEnemies();
            DeleteAllBullets();
            DeleteAllAnimations();
        }

        public T GetActorByTag<T>(string tag) where T : ActorBase
        {
            lock (Collection)
            {
                return Collection.Where(o => o.Tag == tag).FirstOrDefault() as T;
            }
        }

        public void DeleteAllActorsByTag(string tag)
        {
            lock (Collection)
            {
                foreach (var actor in Collection)
                {
                    if (actor.Tag == tag)
                    {
                        actor.QueueForDelete();
                    }
                }
            }
        }

        public void ResetAndShowPlayer()
        {
            Player.Reset();

            RenderRadar = true;
            Player.Visable = true;
            Player.ShipEngineIdleSound.Play();
            Player.AllSystemsGoSound.Play();
        }

        public void HidePlayer()
        {
            Player.Visable = false;
            RenderRadar = false;
            Player.ShipEngineIdleSound.Stop();
            Player.ShipEngineRoarSound.Stop();
        }

        public List<T> VisibleOfType<T>() where T : class
        {
            return (from o in _core.Actors.Collection
                    where o is T
                    && o.Visable == true
                    select o as T).ToList();
        }

        public List<T> OfType<T>() where T : class
        {
            return (from o in _core.Actors.Collection
                    where o is T
                    select o as T).ToList();
        }

        public List<ActorBase> Intersections(ActorBase with)
        {
            var objs = new List<ActorBase>();

            foreach (var obj in Collection.Where(o => o.Visable == true))
            {
                if (obj != with)
                {
                    if (obj.Intersects(with.Location, new Point<double>(with.Size.Width, with.Size.Height)))
                    {
                        objs.Add(obj);
                    }
                }
            }
            return objs;
        }

        public List<ActorBase> Intersections(double x, double y, double width, double height)
        {
            return Intersections(new Point<double>(x, y), new Point<double>(width, height));
        }

        public List<ActorBase> Intersections(Point<double> location, Point<double> size)
        {
            lock (Collection)
            {
                var objs = new List<ActorBase>();

                foreach (var obj in Collection.Where(o => o.Visable == true))
                {
                    if (obj.Intersects(location, size))
                    {
                        objs.Add(obj);
                    }
                }
                return objs;
            }
        }



        public List<T> VisibleEnemiesOfType<T>() where T : class
        {
            return (from o in _core.Actors.OfType<EnemyBase>()
                    where o is T
                    && o.Visable == true
                    select o as T).ToList();
        }

        public void DeleteAllPowerUps()
        {
            lock (Collection)
            {
                OfType<PowerUpBase>().ForEach(c => c.QueueForDelete());
            }
        }

        public void DeleteAllEnemies()
        {
            lock (Collection)
            {
                OfType<EnemyBase>().ForEach(c => c.QueueForDelete());
            }
        }

        public void DeleteAllBullets()
        {
            lock (Collection)
            {
                OfType<BulletBase>().ForEach(c => c.QueueForDelete());
            }
        }

        public void DeleteAllAnimations()
        {
            lock (Collection)
            {
                OfType<ActorAnimation>().ForEach(c => c.QueueForDelete());
            }
        }

        public Bitmap GetBitmapCached(string path)
        {
            Bitmap result = null;

            path = path.ToLower();

            lock (_Bitmaps)
            {
                if (_Bitmaps.ContainsKey(path))
                {
                    result = _Bitmaps[path].Clone() as Bitmap;
                }
                else
                {
                    using (var image = Image.FromFile(path))
                    using (var newbitmap = new Bitmap(image))
                    {
                        result = newbitmap.Clone() as Bitmap;
                        _Bitmaps.Add(path, result);
                    }
                }
            }

            return result;
        }

        public AudioClip GetSoundCached(string wavFilePath, float initialVolumne, bool loopForever = false)
        {
            lock (_audioClips)
            {
                AudioClip result = null;

                wavFilePath = wavFilePath.ToLower();

                if (_audioClips.ContainsKey(wavFilePath))
                {
                    result = _audioClips[wavFilePath];
                }
                else
                {
                    result = new AudioClip(wavFilePath, initialVolumne, loopForever);
                    _audioClips.Add(wavFilePath, result);
                }

                return result;
            }
        }

        public ActorBase Add(string imagePath = null, Size? size = null, string tag = "")
        {
            lock (Collection)
            {
                var actor = new ActorBase(_core, tag)
                {
                    Visable = true
                };

                actor.Initialize(imagePath, size);

                Collection.Add(actor);
                return actor;
            }
        }


        public ActorBase Add(ActorBase actor)
        {
            lock (Collection)
            {
                Collection.Add(actor);
                return actor;
            }
        }

        #region Factories.


        public ActorAttachment AddNewActorAttachment(string imagePath = null, Size? size = null, string tag = "")
        {
            lock (Collection)
            {
                ActorAttachment obj = new ActorAttachment(_core, imagePath, size)
                {
                    Tag = tag
                };
                Collection.Add(obj);
                return obj;
            }
        }


        #endregion

        #region Rendering.

        private Point<double> _radarScale;
        private Point<double> _radarOffset;
        private Bitmap _RadarBackgroundImage = null;
        private readonly SolidBrush _playerRadarDotBrush = new SolidBrush(Color.FromArgb(255, 0, 0));

        private Bitmap _latestFrame = null;
        private readonly object _LatestFrameLock = new object();

        /// <summary>
        /// Using the render thread, we can always have a frame ready, but that really means we render even when we dont need to.
        /// </summary>
        private void RenderThreadProc()
        {
            while (_core.IsRunning)
            {
                RefreshLatestFrame();
                Thread.Sleep(10);
            }
        }

        public void RefreshLatestFrame()
        {
            var frame = Render();

            lock (_LatestFrameLock)
            {
                if (_latestFrame != null)
                {
                    _latestFrame.Dispose();
                    _latestFrame = null;
                }
                _latestFrame = (Bitmap)frame.Clone();
            }
        }

        public Bitmap GetLatestFrame()
        {
            lock (_LatestFrameLock)
            {
                if (_latestFrame == null)
                {
                    return null;
                }

                return (Bitmap)_latestFrame.Clone();
            }
        }

        /// <summary>
        /// Will render the current game state to a single bitmap. If a lock cannot be acquired
        /// for drawing then the previous frame will be returned.
        /// </summary>
        /// <returns></returns>
        public Bitmap Render()
        {
            _core.IsRendering = true;

            var timeout = TimeSpan.FromMilliseconds(1);
            bool lockTaken = false;

            var screenDrawing = _core.DrawingCache.Get(DrawingCacheType.Screen, _core.Display.TotalCanvasSize);
            DrawingCacheItem radarDrawing = null;

            if (_core.DrawingCache.Exists(DrawingCacheType.Radar) == false)
            {
                _RadarBackgroundImage = _core.Actors.GetBitmapCached(@"..\..\..\Assets\Graphics\Radar.png");

                double radarDistance = 5;
                double radarWidth = _RadarBackgroundImage.Width;
                double radarHeight = _RadarBackgroundImage.Height;

                double radarVisionWidth = _core.Display.NatrualScreenSize.Width * radarDistance;
                double radarVisionHeight = _core.Display.NatrualScreenSize.Height * radarDistance;

                radarDrawing = _core.DrawingCache.Get(DrawingCacheType.Radar, new Size((int)radarWidth, (int)radarHeight));

                _radarScale = new Point<double>((double)radarDrawing.Bitmap.Width / radarVisionWidth, (double)radarDrawing.Bitmap.Height / radarVisionHeight);
                _radarOffset = new Point<double>(radarWidth / 2.0, radarHeight / 2.0); //Best guess until player is visible.
            }
            else
            {
                radarDrawing = _core.DrawingCache.Get(DrawingCacheType.Radar);
            }

            if (RenderRadar)
            {
                if (Player is not null && Player.Visable)
                {
                    double centerOfRadarX = (int)(radarDrawing.Bitmap.Width / 2.0) - 2.0; //Subtract half the dot size.
                    double centerOfRadarY = (int)(radarDrawing.Bitmap.Height / 2.0) - 2.0; //Subtract half the dot size.

                    _radarOffset = new Point<double>(
                        centerOfRadarX - Player.X * _radarScale.X,
                        centerOfRadarY - Player.Y * _radarScale.Y);
                }

                radarDrawing.Graphics.DrawImage(_RadarBackgroundImage, new Point(0, 0));
            }

            try
            {
                lockTaken = false;
                Monitor.TryEnter(_core.DrawingSemaphore, timeout, ref lockTaken);

                if (lockTaken)
                {
                    lock (Collection)
                    {
                        screenDrawing.Graphics.Clear(Color.Black);

                        if (RenderRadar)
                        {
                            //Render radar:
                            foreach (var actor in Collection.Where(o => o.Visable == true))
                            {
                                if ((actor is EnemyBase || actor is BulletBase || actor is PowerUpBase) && actor.Visable == true)
                                {
                                    Utility.DynamicCast(actor, actor.GetType()).RenderRadar(radarDrawing.Graphics, _radarScale, _radarOffset);
                                }
                            }

                            //Render player blip:
                            radarDrawing.Graphics.FillEllipse(_playerRadarDotBrush,
                                (int)(radarDrawing.Bitmap.Width / 2.0) - 2,
                                (int)(radarDrawing.Bitmap.Height / 2.0) - 2, 4, 4);
                        }

                        //Render to display:
                        foreach (var actor in Collection.Where(o => o.Visable == true))
                        {
                            if (actor is ActorTextBlock actorTextBlock)
                            {
                                if (actorTextBlock.IsPositionStatic == true)
                                {
                                    continue; //We want to add these later so they are not scaled.
                                }
                            }

                            if (_core.Display.CurrentScaledScreenBounds.IntersectsWith(actor.Bounds))
                            {
                                Utility.DynamicCast(actor, actor.GetType()).Render(screenDrawing.Graphics);
                            }
                        }
                        Player?.Render(screenDrawing.Graphics);
                    }

                    MenuFactory.Render(screenDrawing.Graphics);
                }

                //displayDC.DrawImage(screenDrawing.Bitmap, 0, 0);
            }
            finally
            {
                // Ensure that the lock is released.
                if (lockTaken)
                {
                    Monitor.Exit(_core.DrawingSemaphore);
                }
            }

            if (_core.Settings.HighlightNatrualBounds)
            {
                //Highlight the 1:1 frame
                using (var pen = new Pen(Color.Gray, 1))
                {
                    screenDrawing.Graphics.DrawRectangle(pen, _core.Display.NatrualScreenBounds);
                }


                //Highlight the 1:1 frame
                using (var pen = new Pen(Color.Red, 5))
                {
                    screenDrawing.Graphics.DrawRectangle(pen, _core.Display.CurrentScaledScreenBounds);
                }
            }

            var scaledDrawing = _core.DrawingCache.Get(DrawingCacheType.Scaling, _core.Display.NatrualScreenSize);

            if (_core.Settings.AutoZoomWhenMoving)
            {
                if (_core.Actors.Player != null)
                {
                    //Scale the screen based on the player throttle.
                    if (_core.Actors.Player.Velocity.ThrottlePercentage > 0.5)
                        _core.Display.ThrottleFrameScaleFactor += 2;
                    else if (_core.Actors.Player.Velocity.ThrottlePercentage < 1)
                        _core.Display.ThrottleFrameScaleFactor -= 2;

                    //Scale the screen based on the player boost.
                    _core.Display.ThrottleFrameScaleFactor = _core.Display.ThrottleFrameScaleFactor.Box(0, 40);
                    if (_core.Actors.Player.Velocity.BoostPercentage > 0.5)
                        _core.Display.BoostFrameScaleFactor += 1;
                    else if (_core.Actors.Player.Velocity.BoostPercentage < 1)
                        _core.Display.BoostFrameScaleFactor -= 1;

                    _core.Display.BoostFrameScaleFactor = _core.Display.BoostFrameScaleFactor.Box(0, 20);
                }
            }

            //Select the bitmap from the large screen bitmap and copy it to the "scaling drawing".
            int scaleSubtraction = _core.Display.SpeedOrientedFrameScalingSubtraction();

            scaledDrawing.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            scaledDrawing.Graphics.DrawImage(screenDrawing.Bitmap,
                    new RectangleF(0, 0, _core.Display.NatrualScreenSize.Width, _core.Display.NatrualScreenSize.Height),
                    new Rectangle(
                        _core.Display.OverdrawSize.Width / 2 - scaleSubtraction,
                        _core.Display.OverdrawSize.Height / 2 - scaleSubtraction,
                        _core.Display.NatrualScreenSize.Width + scaleSubtraction * 2,
                        _core.Display.NatrualScreenSize.Height + scaleSubtraction * 2
                    ),
                GraphicsUnit.Pixel);

            if (RenderRadar)
            {
                //We add the radar last so that it does not get scaled down.
                var rect = new Rectangle(
                        (_core.Display.NatrualScreenSize.Width - (radarDrawing.Bitmap.Width + 25)),
                        (_core.Display.NatrualScreenSize.Height - (radarDrawing.Bitmap.Height + 50)),
                        radarDrawing.Bitmap.Width, radarDrawing.Bitmap.Height
                    );
                scaledDrawing.Graphics.DrawImage(radarDrawing.Bitmap, rect);
            }
            try
            {
                lockTaken = false;
                Monitor.TryEnter(_core.DrawingSemaphore, timeout, ref lockTaken);

                if (lockTaken)
                {
                    lock (Collection)
                    {
                        //Render to display:
                        foreach (var actor in OfType<ActorTextBlock>().Where(o => o.Visable == true && o.IsPositionStatic == true))
                        {
                            Utility.DynamicCast(actor, actor.GetType()).Render(scaledDrawing.Graphics);
                        }
                    }
                }
            }
            finally
            {
                // Ensure that the lock is released.
                if (lockTaken)
                {
                    Monitor.Exit(_core.DrawingSemaphore);
                }
            }

            _core.IsRendering = false;

            return scaledDrawing.Bitmap;
        }

        #endregion
    }
}