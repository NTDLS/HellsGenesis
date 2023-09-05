using HG.Actors;
using HG.Actors.Enemies;
using HG.Actors.PowerUp;
using HG.Actors.Weapons.Bullets;
using HG.Menus;
using HG.TickHandlers;
using HG.Types;
using HG.Utility.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using static HG.Engine.Controllers.EngineDrawingCacheController;

namespace HG.Engine.Controllers
{
    /// <summary>
    /// Contains the collection of all actors and their factories.
    /// </summary>
    internal class EngineActorController
    {
        private readonly Core _core;

        public ActorTextBlock PlayerStatsText { get; private set; }
        public ActorTextBlock DebugText { get; private set; }
        public bool RenderRadar { get; set; } = false;

        #region Actors and their factories.

        internal List<ActorBase> Collection { get; private set; } = new();
        public ActorAnimationTickHandler Animations { get; set; }
        public ActorAttachmentTickHandler Attachments { get; set; }
        public ActorBulletTickHandler Bullets { get; set; }
        public ActorDebugTickHandler Debugs { get; set; }
        public ActorEnemyTickHandler Enemies { get; set; }
        public ActorPowerupTickHandler Powerups { get; set; }
        public ActorRadarPositionTickHandler RadarPositions { get; set; }
        public ActorStarTickHandler Stars { get; set; }
        public ActorTextBlockTickHandler TextBlocks { get; set; }



        #endregion

        public EngineActorController(Core core)
        {
            _core = core;

            Animations = new ActorAnimationTickHandler(_core, this);
            Attachments = new ActorAttachmentTickHandler(_core, this);
            Bullets = new ActorBulletTickHandler(_core, this);
            Debugs = new ActorDebugTickHandler(_core, this);
            Enemies = new ActorEnemyTickHandler(_core, this);
            Powerups = new ActorPowerupTickHandler(_core, this);
            RadarPositions = new ActorRadarPositionTickHandler(_core, this);
            Stars = new ActorStarTickHandler(_core, this);
            TextBlocks = new ActorTextBlockTickHandler(_core, this);
        }

        public void Start()
        {
            _core.Player.Actor = new ActorPlayer(_core, Constants.PlayerClass.Atlant) { Visable = false };

            PlayerStatsText = TextBlocks.Create("Consolas", Brushes.WhiteSmoke, 9, new HgPoint<double>(5, 5), true);
            PlayerStatsText.Visable = false;
            DebugText = TextBlocks.Create("Consolas", Brushes.Aqua, 10, new HgPoint<double>(5, PlayerStatsText.Y + 80), true);

            _core.Audio.BackgroundMusicSound.Play();
        }

        public void Stop()
        {
        }

        public void CleanupDeletedObjects()
        {
            lock (Collection)
            {
                _core.Actors.Collection.Where(o => o.ReadyForDeletion).ToList().ForEach(p => p.Cleanup());
                _core.Actors.Collection.RemoveAll(o => o.ReadyForDeletion);

                for (int i = 0; i < _core.Events.Collection.Count; i++)
                {
                    if (_core.Events.Collection[i].ReadyForDeletion)
                    {
                        _core.Events.Delete(_core.Events.Collection[i]);
                    }
                }

                _core.Menus.CleanupDeletedObjects();

                if (_core.Player.Actor.IsDead)
                {
                    _core.Player.Actor.Visable = false;
                    _core.Player.Actor.IsDead = false;
                    _core.Menus.Insert(new MenuStartNewGame(_core));
                }
            }
        }

        public void NewGame()
        {
            lock (Collection)
            {
                _core.Situations.Reset();
                PlayerStatsText.Visable = true;
                DeleteAll();

                _core.Situations.AdvanceSituation();
            }
        }

        public void DeleteAll()
        {
            Powerups.DeleteAll();
            Enemies.DeleteAll();
            Bullets.DeleteAll();
            Animations.DeleteAll();
        }

        public T GetActorByAssetTag<T>(string assetTag) where T : ActorBase
        {
            lock (Collection)
            {
                return Collection.Where(o => o.AssetTag == assetTag).SingleOrDefault() as T;
            }
        }

        public List<T> OfType<T>() where T : class
        {
            lock (Collection)
            {
                return _core.Actors.Collection.Where(o => o is T).Select(o => o as T).ToList();
            }
        }

        public List<T> VisibleOfType<T>() where T : class
        {
            lock (Collection)
            {
                return _core.Actors.Collection.Where(o => o is T && o.Visable == true).Select(o => o as T).ToList();
            }
        }

        public void DeleteAllActorsByAssetTag(string assetTag)
        {
            lock (Collection)
            {
                foreach (var actor in Collection)
                {
                    if (actor.AssetTag == assetTag)
                    {
                        actor.QueueForDelete();
                    }
                }
            }
        }

        public List<ActorBase> Intersections(ActorBase with)
        {
            lock (Collection)
            {
                var objs = new List<ActorBase>();

                foreach (var obj in Collection.Where(o => o.Visable == true))
                {
                    if (obj != with)
                    {
                        if (obj.Intersects(with.Location, new HgPoint<double>(with.Size.Width, with.Size.Height)))
                        {
                            objs.Add(obj);
                        }
                    }
                }
                return objs;
            }
        }

        public List<ActorBase> Intersections(double x, double y, double width, double height)
        {
            return Intersections(new HgPoint<double>(x, y), new HgPoint<double>(width, height));
        }

        public List<ActorBase> Intersections(HgPoint<double> location, HgPoint<double> size)
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

        public ActorBase Insert(ActorBase actor)
        {
            lock (Collection)
            {
                Collection.Add(actor);
                return actor;
            }
        }

        #region Rendering.

        private HgPoint<double> _radarScale;
        private HgPoint<double> _radarOffset;
        private Bitmap _RadarBackgroundImage = null;
        private readonly SolidBrush _playerRadarDotBrush = new SolidBrush(Color.FromArgb(255, 0, 0));

        TimeSpan lockTimeout = TimeSpan.FromMilliseconds(1);

        /// <summary>
        /// Will render the current game state to a single bitmap. If a lock cannot be acquired
        /// for drawing then the previous frame will be returned.
        /// </summary>
        /// <returns></returns>
        public Bitmap Render()
        {
            _core.IsRendering = true;

            bool lockTaken = false;

            var screenDrawing = _core.DrawingCache.Get(DrawingCacheType.Screen, _core.Display.TotalCanvasSize);
            DrawingCacheItem radarDrawing = null;

            if (_core.DrawingCache.Exists(DrawingCacheType.Radar) == false)
            {
                _RadarBackgroundImage = _core.Imaging.Get(@"Graphics\RadarTransparent.png");

                double radarDistance = 5;
                double radarWidth = _RadarBackgroundImage.Width;
                double radarHeight = _RadarBackgroundImage.Height;

                double radarVisionWidth = _core.Display.NatrualScreenSize.Width * radarDistance;
                double radarVisionHeight = _core.Display.NatrualScreenSize.Height * radarDistance;

                radarDrawing = _core.DrawingCache.Get(DrawingCacheType.Radar, new Size((int)radarWidth, (int)radarHeight));

                _radarScale = new HgPoint<double>(radarDrawing.Bitmap.Width / radarVisionWidth, radarDrawing.Bitmap.Height / radarVisionHeight);
                _radarOffset = new HgPoint<double>(radarWidth / 2.0, radarHeight / 2.0); //Best guess until player is visible.
            }
            else
            {
                radarDrawing = _core.DrawingCache.Get(DrawingCacheType.Radar);
            }

            if (RenderRadar)
            {
                if (_core.Player.Actor is not null && _core.Player.Actor.Visable)
                {
                    double centerOfRadarX = (int)(radarDrawing.Bitmap.Width / 2.0) - 2.0; //Subtract half the dot size.
                    double centerOfRadarY = (int)(radarDrawing.Bitmap.Height / 2.0) - 2.0; //Subtract half the dot size.

                    _radarOffset = new HgPoint<double>(
                        centerOfRadarX - _core.Player.Actor.X * _radarScale.X,
                        centerOfRadarY - _core.Player.Actor.Y * _radarScale.Y);
                }

                radarDrawing.Graphics.DrawImage(_RadarBackgroundImage, new Point(0, 0));
            }

            try
            {
                lockTaken = false;
                Monitor.TryEnter(_core.DrawingSemaphore, lockTimeout, ref lockTaken);

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
                                    HgConversion.DynamicCast(actor, actor.GetType()).RenderRadar(radarDrawing.Graphics, _radarScale, _radarOffset);
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
                                HgConversion.DynamicCast(actor, actor.GetType()).Render(screenDrawing.Graphics);
                            }
                        }
                        _core.Player.Actor?.Render(screenDrawing.Graphics);
                    }

                    _core.Menus.Render(screenDrawing.Graphics);
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
                if (_core.Player.Actor != null)
                {
                    //Scale the screen based on the player throttle.
                    if (_core.Player.Actor.Velocity.ThrottlePercentage > 0.5)
                        _core.Display.ThrottleFrameScaleFactor += 2;
                    else if (_core.Player.Actor.Velocity.ThrottlePercentage < 1)
                        _core.Display.ThrottleFrameScaleFactor -= 2;

                    //Scale the screen based on the player boost.
                    _core.Display.ThrottleFrameScaleFactor = _core.Display.ThrottleFrameScaleFactor.Box(0, 100);
                    if (_core.Player.Actor.Velocity.BoostPercentage > 0.5)
                        _core.Display.BoostFrameScaleFactor += 1;
                    else if (_core.Player.Actor.Velocity.BoostPercentage < 1)
                        _core.Display.BoostFrameScaleFactor -= 1;

                    _core.Display.BoostFrameScaleFactor = _core.Display.BoostFrameScaleFactor.Box(0, 100);
                }
            }

            double zoomFactor = _core.Display.SpeedOrientedFrameScalingFactor();
            int zoomedWidth = (int)(_core.Display.NatrualScreenSize.Width / zoomFactor);
            int zoomedHeight = (int)(_core.Display.NatrualScreenSize.Height / zoomFactor);
            int sourceX = (screenDrawing.Bitmap.Width - zoomedWidth) / 2;
            int sourceY = (screenDrawing.Bitmap.Height - zoomedHeight) / 2;

            // Draw the center-zoomed image
            scaledDrawing.Graphics.DrawImage(screenDrawing.Bitmap,
                new RectangleF(0, 0, _core.Display.NatrualScreenSize.Width, _core.Display.NatrualScreenSize.Height),
                new Rectangle(sourceX, sourceY, zoomedWidth, zoomedHeight),
                GraphicsUnit.Pixel);

            if (RenderRadar)
            {
                //We add the radar last so that it does not get scaled down.
                var rect = new Rectangle(
                        _core.Display.NatrualScreenSize.Width - (radarDrawing.Bitmap.Width + 25),
                        _core.Display.NatrualScreenSize.Height - (radarDrawing.Bitmap.Height + 50),
                        radarDrawing.Bitmap.Width, radarDrawing.Bitmap.Height
                    );
                scaledDrawing.Graphics.DrawImage(radarDrawing.Bitmap, rect);
            }

            lock (Collection)
            {
                //Render to display:
                foreach (var actor in OfType<ActorTextBlock>().Where(o => o.Visable == true && o.IsPositionStatic == true))
                {
                    HgConversion.DynamicCast(actor, actor.GetType()).Render(scaledDrawing.Graphics);
                }
            }

            _core.IsRendering = false;

            return scaledDrawing.Bitmap;
        }

        #endregion
    }
}
