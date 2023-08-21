using AI2D.Actors;
using AI2D.Actors.Bullets;
using AI2D.Actors.Enemies;
using AI2D.Actors.PowerUp;
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
        private Core _core;

        #region Actors.
        public List<EngineCallbackEvent> EngineEvents { get; private set; } = new List<EngineCallbackEvent>();
        public List<ActorBase> Collection { get; private set; } = new List<ActorBase>();
        public List<BaseMenu> Menus { get; private set; } = new List<BaseMenu>();
        public ActorPlayer Player { get; private set; }
        public ActorTextBlock PlayerStatsText { get; private set; }
        public ActorTextBlock DebugText { get; private set; }
        public bool RenderRadar { get; set; } = false;

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
        }

        public void Start()
        {
            Player = new ActorPlayer(_core, Constants.PlayerClass.Atlant) { Visable = false };

            DoorIsAjarSound = GetSoundCached(@"..\..\..\Assets\Sounds\Ship\Door Is Ajar.wav", 0.50f, false);
            RadarBlipsSound = GetSoundCached(@"..\..\..\Assets\Sounds\Ship\Radar Blips.wav", 0.20f, false);
            LockedOnBlip = GetSoundCached(@"..\..\..\Assets\Sounds\Ship\Locked On.wav", 0.20f, false);

            BackgroundMusicSound = GetSoundCached(@"..\..\..\Assets\Sounds\Music\Background.wav", 0.25f, true);

            PlayerStatsText = AddNewTextBlock("Consolas", Brushes.WhiteSmoke, 9,
                //new Point<double>((_core.Display.OverdrawSize.Width) / 2 + 5, (_core.Display.OverdrawSize.Height / 2) + 5), true);
                new Point<double>(5, 5), true);
            PlayerStatsText.Visable = false;
            DebugText = AddNewTextBlock("Consolas", Brushes.Aqua, 10, new Point<double>(5, PlayerStatsText.Y + 80), true);

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

            for (int i = 0; i < _core.Actors.EngineEvents.Count; i++)
            {
                if (_core.Actors.EngineEvents[i].ReadyForDeletion)
                {
                    _core.Actors.DeleteEngineCallbackEvent(_core.Actors.EngineEvents[i]);
                }
            }

            for (int i = 0; i < _core.Actors.Menus.Count; i++)
            {
                if (_core.Actors.Menus[i].ReadyForDeletion)
                {
                    _core.Actors.DeleteMenu(_core.Actors.Menus[i]);
                }
            }

            if (_core.Actors.Player.IsDead)
            {
                _core.Actors.Player.Visable = false;
                _core.Actors.Player.IsDead = false;
                _core.Actors.InsertMenu(new MenuStartNewGame(_core));
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

        private void TheDoorIsAjarCallback(Core core, EngineCallbackEvent sender, object refObj)
        {
            DoorIsAjarSound.Play();
            InsertMenu(new MenuStartNewGame(_core));
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

        public ActorRadarPositionIndicator AddNewRadarPositionIndicator()
        {
            lock (Collection)
            {
                var obj = new ActorRadarPositionIndicator(_core);
                Collection.Add(obj);
                return obj;
            }
        }

        public void DeleteRadarPositionIndicator(ActorRadarPositionIndicator obj)
        {
            lock (Collection)
            {
                obj.Cleanup();
                obj.Visable = false;
                Collection.Remove(obj);
            }
        }

        public void PlaceAnimationOnTopOf(ActorAnimation animation, ActorBase defaultPosition)
        {
            lock (Collection)
            {
                animation.X = defaultPosition.X;
                animation.Y = defaultPosition.Y;
                animation.RotationMode = RotationMode.Clip; //Much less expensive. Use this or NONE if you can.
                Collection.Add(animation);
            }
        }

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

        public ActorAnimation AddNewAnimation(string imageFrames, Size frameSize, int _frameDelayMilliseconds = 10, ActorAnimation.PlayMode playMode = null)
        {
            lock (Collection)
            {
                ActorAnimation obj = new ActorAnimation(_core, imageFrames, frameSize, _frameDelayMilliseconds, playMode);
                Collection.Add(obj);
                return obj;
            }
        }

        public void DeleteAnimation(ActorAnimation obj)
        {
            lock (Collection)
            {
                obj.Cleanup();
                Collection.Remove(obj);
            }
        }

        public void QueueTheDoorIsAjar()
        {

            AddNewEngineCallbackEvent(new TimeSpan(0, 0, 0, 5), _core.Actors.TheDoorIsAjarCallback);
        }


        public EngineCallbackEvent AddNewEngineCallbackEvent(
            TimeSpan countdown, EngineCallbackEvent.OnExecute executeCallback, object refObj,
            EngineCallbackEvent.CallbackEventMode callbackEventMode = EngineCallbackEvent.CallbackEventMode.OneTime,
            EngineCallbackEvent.CallbackEventAsync callbackEventAsync = EngineCallbackEvent.CallbackEventAsync.Synchronous)
        {
            lock (EngineEvents)
            {
                EngineCallbackEvent obj = new EngineCallbackEvent(_core, countdown, executeCallback, refObj, callbackEventMode, callbackEventAsync);
                EngineEvents.Add(obj);
                return obj;
            }
        }

        public EngineCallbackEvent AddNewEngineCallbackEvent(TimeSpan countdown, EngineCallbackEvent.OnExecute executeCallback, object refObj)
        {
            lock (EngineEvents)
            {
                EngineCallbackEvent obj = new EngineCallbackEvent(_core, countdown, executeCallback, refObj);
                EngineEvents.Add(obj);
                return obj;
            }
        }

        public EngineCallbackEvent AddNewEngineCallbackEvent(TimeSpan countdown, EngineCallbackEvent.OnExecute executeCallback)
        {
            lock (EngineEvents)
            {
                EngineCallbackEvent obj = new EngineCallbackEvent(_core, countdown, executeCallback);
                EngineEvents.Add(obj);
                return obj;
            }
        }

        public EngineCallbackEvent InjectCallbackEvent(EngineCallbackEvent obj)
        {
            lock (EngineEvents)
            {
                EngineEvents.Add(obj);
                return obj;
            }
        }

        public void DeleteEngineCallbackEvent(EngineCallbackEvent obj)
        {
            lock (EngineEvents)
            {
                EngineEvents.Remove(obj);
            }
        }

        public ActorRadarPositionTextBlock AddNewRadarPositionTextBlock(string font, Brush color, double size, Point<double> location)
        {
            lock (Collection)
            {
                var obj = new ActorRadarPositionTextBlock(_core, font, color, size, location);
                Collection.Add(obj);
                return obj;
            }
        }

        public ActorTextBlock AddNewTextBlock(string font, Brush color, double size, Point<double> location, bool isPositionStatic)
        {
            lock (Collection)
            {
                var obj = new ActorTextBlock(_core, font, color, size, location, isPositionStatic);
                Collection.Add(obj);
                return obj;
            }
        }

        public ActorTextBlock AddNewTextBlock(string font, Brush color, double size, Point<double> location, bool isPositionStatic, string tag)
        {
            lock (Collection)
            {
                var obj = new ActorTextBlock(_core, font, color, size, location, isPositionStatic);
                obj.Tag = tag;
                Collection.Add(obj);
                return obj;
            }
        }

        public void DeleteTextBlock(ActorTextBlock obj)
        {
            lock (Collection)
            {
                obj.Cleanup();
                Collection.Remove(obj);
            }
        }

        public ActorDebug AddNewDebug()
        {
            lock (Collection)
            {
                var obj = new ActorDebug(_core);
                Collection.Add(obj);
                return obj;
            }
        }

        public void DeleteDebug(ActorDebug obj)
        {
            lock (Collection)
            {
                obj.Cleanup();
                Collection.Remove(obj);
            }
        }

        public ActorStar AddNewStar(double x, double y)
        {
            lock (Collection)
            {
                var obj = new ActorStar(_core)
                {
                    X = x,
                    Y = y
                };
                Collection.Add(obj);
                return obj;
            }
        }

        public ActorStar AddNewStar()
        {
            lock (Collection)
            {
                var obj = new ActorStar(_core);
                Collection.Add(obj);
                return obj;
            }
        }

        public void DeleteStar(ActorStar obj)
        {
            lock (Collection)
            {
                obj.Cleanup();
                Collection.Remove(obj);
            }
        }

        public void InjectEnemy(EnemyBase obj)
        {
            lock (Collection)
            {
                Collection.Add(obj);
            }
        }

        public void InjectPowerUp(PowerUpBase obj)
        {
            lock (Collection)
            {
                Collection.Add(obj);
            }
        }

        public void DeletePowerUp(PowerUpBase obj)
        {
            lock (Collection)
            {
                obj.Cleanup();
                Collection.Remove(obj);
            }
        }

        public T AddNewPowerUp<T>() where T : PowerUpBase
        {
            lock (Collection)
            {
                object[] param = { _core };
                PowerUpBase obj = (PowerUpBase)Activator.CreateInstance(typeof(T), param);

                obj.Location = _core.Display.RandomOffScreenLocation(100, 1000);

                Collection.Add(obj);
                return (T)obj;
            }
        }

        public T AddNewEnemy<T>() where T : EnemyBase
        {
            lock (Collection)
            {
                object[] param = { _core };
                EnemyBase obj = (EnemyBase)Activator.CreateInstance(typeof(T), param);

                obj.Location = _core.Display.RandomOffScreenLocation();
                obj.Velocity.MaxSpeed = Utility.Random.Next(Constants.Limits.MinSpeed, Constants.Limits.MaxSpeed);
                obj.Velocity.Angle.Degrees = Utility.Random.Next(0, 360);

                obj.BeforeCreate();
                Collection.Add(obj);
                obj.AfterCreate();

                return (T)obj;
            }
        }

        public void DeleteEnemy(EnemyBase obj)
        {
            lock (Collection)
            {
                obj.Cleanup();
                Collection.Remove(obj);
            }
        }

        public BulletBase AddNewLockedBullet(WeaponBase weapon, ActorBase firedFrom, ActorBase lockedTarget, Point<double> xyOffset = null)
        {
            lock (Collection)
            {
                var obj = weapon.CreateBullet(lockedTarget, xyOffset);
                Collection.Add(obj);
                return obj;
            }
        }

        public BulletBase AddNewBullet(WeaponBase weapon, ActorBase firedFrom, Point<double> xyOffset = null)
        {
            lock (Collection)
            {
                var obj = weapon.CreateBullet(null, xyOffset);
                Collection.Add(obj);
                return obj;
            }
        }

        public void DeleteBullet(BulletBase obj)
        {
            lock (Collection)
            {
                obj.Cleanup();
                Collection.Remove(obj);
            }
        }

        public void InsertMenu(BaseMenu menu)
        {
            lock (Menus)
            {
                Menus.Add(menu);
            }
        }

        public void DeleteMenu(BaseMenu menu)
        {
            lock (Menus)
            {
                menu.Cleanup();
                Menus.Remove(menu);
            }
        }

        #endregion

        #region Rendering.

        private Point<double> _radarScale;
        private Point<double> _radarOffset;
        private Bitmap _RadarBackgroundImage = null;
        private SolidBrush _playerRadarDotBrush = new SolidBrush(Color.FromArgb(255, 0, 0));

        private Bitmap _latestFrame = null;
        private object _LatestFrameLock = new object();

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

                            if (_core.Display.NatrualScreenBounds.IntersectsWith(actor.Bounds))
                            {
                                Utility.DynamicCast(actor, actor.GetType()).Render(screenDrawing.Graphics);
                            }
                        }
                        Player?.Render(screenDrawing.Graphics);
                    }

                    lock (Menus)
                    {
                        foreach (var obj in Menus)
                        {
                            obj.Render(screenDrawing.Graphics);
                        }
                    }
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
                using (var pen = new Pen(Color.Red, 3))
                {
                    var bounds = _core.Display.NatrualScreenBounds.Clone();

                    bounds.Height -= 40;
                    bounds.Width -= 16;

                    screenDrawing.Graphics.DrawRectangle(pen, bounds);
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