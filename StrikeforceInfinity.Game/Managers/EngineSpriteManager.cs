using StrikeforceInfinity.Game.Controller;
using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Menus;
using StrikeforceInfinity.Game.Sprites;
using StrikeforceInfinity.Game.Sprites.Enemies.BasesAndInterfaces;
using StrikeforceInfinity.Game.Sprites.Player;
using StrikeforceInfinity.Game.Sprites.Player.BasesAndInterfaces;
using StrikeforceInfinity.Game.Sprites.PowerUp.BasesAndInterfaces;
using StrikeforceInfinity.Game.Utility;
using StrikeforceInfinity.Game.Utility.ExtensionMethods;
using StrikeforceInfinity.Game.Weapons.Munitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StrikeforceInfinity.Game.Managers
{
    /// <summary>
    /// Contains the collection of all sprites and their factories.
    /// </summary>
    internal class EngineSpriteManager
    {
        private readonly EngineCore _gameCore;
        private SiPoint _radarScale;
        private SiPoint _radarOffset;

        public SpriteTextBlock PlayerStatsText { get; private set; }
        public SpriteTextBlock DebugText { get; private set; }
        public bool RenderRadar { get; set; } = false;

        #region Sprites and their factories.

        internal List<SpriteBase> Collection { get; private set; } = new();

        public AnimationSpriteTickController Animations { get; private set; }
        public AttachmentSpriteTickController Attachments { get; private set; }
        public MunitionSpriteTickController Munitions { get; private set; }
        public DebugsSpriteTickController Debugs { get; private set; }
        public EnemiesSpriteTickController Enemies { get; private set; }
        public ParticlesSpriteTickController Particles { get; private set; }
        public PowerupsSpriteTickController Powerups { get; private set; }
        public RadarPositionsSpriteTickController RadarPositions { get; set; }
        public StarsSpriteTickController Stars { get; private set; }
        public TextBlocksSpriteTickController TextBlocks { get; private set; }
        public PlayerDronesSpriteTickController PlayerDrones { get; private set; }

        #endregion

        public EngineSpriteManager(EngineCore gameCore)
        {
            _gameCore = gameCore;

            Animations = new AnimationSpriteTickController(_gameCore, this);
            Attachments = new AttachmentSpriteTickController(_gameCore, this);
            Munitions = new MunitionSpriteTickController(_gameCore, this);
            Debugs = new DebugsSpriteTickController(_gameCore, this);
            Enemies = new EnemiesSpriteTickController(_gameCore, this);
            Particles = new ParticlesSpriteTickController(_gameCore, this);
            Powerups = new PowerupsSpriteTickController(_gameCore, this);
            RadarPositions = new RadarPositionsSpriteTickController(_gameCore, this);
            Stars = new StarsSpriteTickController(_gameCore, this);
            TextBlocks = new TextBlocksSpriteTickController(_gameCore, this);
            PlayerDrones = new PlayerDronesSpriteTickController(_gameCore, this);
        }

        public void Start()
        {
            _gameCore.Player.Sprite = new SpriteDebugPlayer(_gameCore) { Visable = false };

            PlayerStatsText = TextBlocks.Create(_gameCore.Rendering.TextFormats.RealtimePlayerStats, _gameCore.Rendering.Materials.Brushes.WhiteSmoke, new SiPoint(5, 5), true);
            PlayerStatsText.Visable = false;
            DebugText = TextBlocks.Create(_gameCore.Rendering.TextFormats.RealtimePlayerStats, _gameCore.Rendering.Materials.Brushes.Cyan, new SiPoint(5, PlayerStatsText.Y + 100), true);

            _gameCore.Audio.BackgroundMusicSound.Play();
        }

        public void Stop()
        {
        }

        public SpriteBase CreateByNameOfType(string typeFullName)
        {
            lock (Collection)
            {
                var type = Type.GetType(typeFullName) ?? throw new ArgumentException($"Type with FullName '{typeFullName}' not found.");
                object[] param = { _gameCore };
                var obj = (SpriteBase)Activator.CreateInstance(type, param);

                obj.Location = _gameCore.Display.RandomOffScreenLocation();
                obj.Velocity.MaxSpeed = HgRandom.Generator.Next(_gameCore.Settings.MinEnemySpeed, _gameCore.Settings.MaxEnemySpeed);
                obj.Velocity.Angle.Degrees = HgRandom.Generator.Next(0, 360);

                var enemy = obj as SpriteEnemyBase;

                enemy?.BeforeCreate();
                Collection.Add(obj);
                enemy?.AfterCreate();

                return obj;
            }
        }

        public void CleanupDeletedObjects()
        {
            lock (Collection)
            {
                _gameCore.Sprites.Collection.Where(o => o.QueuedForDeletion).ToList().ForEach(p => p.Cleanup());
                _gameCore.Sprites.Collection.RemoveAll(o => o.QueuedForDeletion);

                for (int i = 0; i < _gameCore.Events.Collection.Count; i++)
                {
                    if (_gameCore.Events.Collection[i].QueuedForDeletion)
                    {
                        _gameCore.Events.Delete(_gameCore.Events.Collection[i]);
                    }
                }

                _gameCore.Menus.CleanupDeletedObjects();

                if (_gameCore.Player.Sprite.IsDead)
                {
                    _gameCore.Player.Sprite.Visable = false;
                    _gameCore.Player.Sprite.IsDead = false;
                    _gameCore.Menus.Insert(new MenuStartNewGame(_gameCore));
                }
            }
        }

        public void DeleteAll()
        {
            Powerups.DeleteAll();
            Enemies.DeleteAll();
            Munitions.DeleteAll();
            Animations.DeleteAll();
        }

        public T GetSpriteByTag<T>(string name) where T : SpriteBase
        {
            lock (Collection)
            {
                return Collection.Where(o => o.SpriteTag == name).SingleOrDefault() as T;
            }
        }

        public List<T> OfType<T>() where T : class
        {
            lock (Collection)
            {
                return _gameCore.Sprites.Collection.Where(o => o is T).Select(o => o as T).ToList();
            }
        }

        public List<T> VisibleOfType<T>() where T : class
        {
            lock (Collection)
            {
                return _gameCore.Sprites.Collection.Where(o => o is T && o.Visable == true).Select(o => o as T).ToList();
            }
        }

        public void DeleteAllSpritesByTag(string name)
        {
            lock (Collection)
            {
                foreach (var sprite in Collection)
                {
                    if (sprite.SpriteTag == name)
                    {
                        sprite.QueueForDelete();
                    }
                }
            }
        }

        public List<SpriteBase> Intersections(SpriteBase with)
        {
            lock (Collection)
            {
                var objs = new List<SpriteBase>();

                foreach (var obj in Collection.Where(o => o.Visable == true))
                {
                    if (obj != with)
                    {
                        if (obj.Intersects(with.Location, new SiPoint(with.Size.Width, with.Size.Height)))
                        {
                            objs.Add(obj);
                        }
                    }
                }
                return objs;
            }
        }

        public List<SpriteBase> Intersections(double x, double y, double width, double height)
            => Intersections(new SiPoint(x, y), new SiPoint(width, height));

        public List<SpriteBase> Intersections(SiPoint location, SiPoint size)
        {
            lock (Collection)
            {
                var objs = new List<SpriteBase>();

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

        public SpritePlayerBase InsertPlayer(SpritePlayerBase sprite)
        {
            lock (Collection)
            {
                Collection.Add(sprite);
                return sprite;
            }
        }

        public void RenderPostScaling(SharpDX.Direct2D1.RenderTarget renderTarget)
        {
            //Render to display:
            foreach (var sprite in OfType<SpriteTextBlock>().Where(o => o.Visable == true && o.IsFixedPosition == true))
            {
                sprite.Render(renderTarget);
            }

            if (RenderRadar)
            {
                var radarBgImage = _gameCore.Assets.GetBitmap(@"Graphics\RadarTransparent.png");

                _gameCore.Rendering.DrawBitmapAt(renderTarget, radarBgImage,
                    _gameCore.Display.NatrualScreenSize.Width - radarBgImage.Size.Width,
                    _gameCore.Display.NatrualScreenSize.Height - radarBgImage.Size.Height,
                    0);

                double radarDistance = 8;

                if (_radarScale == null)
                {
                    double radarVisionWidth = _gameCore.Display.TotalCanvasSize.Width * radarDistance;
                    double radarVisionHeight = _gameCore.Display.TotalCanvasSize.Height * radarDistance;

                    _radarScale = new SiPoint(radarBgImage.Size.Width / radarVisionWidth, radarBgImage.Size.Height / radarVisionHeight);
                    _radarOffset = new SiPoint(radarBgImage.Size.Width / 2.0, radarBgImage.Size.Height / 2.0); //Best guess until player is visible.
                }

                if (_gameCore.Player.Sprite is not null && _gameCore.Player.Sprite.Visable)
                {
                    double centerOfRadarX = (int)(radarBgImage.Size.Width / 2.0) - 2.0; //Subtract half the dot size.
                    double centerOfRadarY = (int)(radarBgImage.Size.Height / 2.0) - 2.0; //Subtract half the dot size.

                    _radarOffset = new SiPoint(
                            _gameCore.Display.NatrualScreenSize.Width - radarBgImage.Size.Width + (centerOfRadarX - _gameCore.Player.Sprite.X * _radarScale.X),
                            _gameCore.Display.NatrualScreenSize.Height - radarBgImage.Size.Height + (centerOfRadarY - _gameCore.Player.Sprite.Y * _radarScale.Y)
                        );

                    //Render radar:
                    foreach (var sprite in Collection.Where(o => o.Visable == true))
                    {
                        //HgPoint scale, HgPoint< double > offset
                        int x = (int)(_radarOffset.X + sprite.X * _radarScale.X);
                        int y = (int)(_radarOffset.Y + sprite.Y * _radarScale.Y);

                        if (x > _gameCore.Display.NatrualScreenSize.Width - radarBgImage.Size.Width
                            && x < _gameCore.Display.NatrualScreenSize.Width - radarBgImage.Size.Width + radarBgImage.Size.Width
                            && y > _gameCore.Display.NatrualScreenSize.Height - radarBgImage.Size.Height
                            && y < _gameCore.Display.NatrualScreenSize.Height - radarBgImage.Size.Height + radarBgImage.Size.Height
                            )
                        {
                            if ((sprite is SpriteEnemyBase || sprite is MunitionBase || sprite is SpritePowerUpBase) && sprite.Visable == true)
                            {
                                sprite.RenderRadar(renderTarget, x, y);
                            }
                        }
                    }

                    //Render player blip:
                    _gameCore.Rendering.FillEllipseAt(
                        renderTarget,
                        _gameCore.Display.NatrualScreenSize.Width - radarBgImage.Size.Width + centerOfRadarX,
                        _gameCore.Display.NatrualScreenSize.Height - radarBgImage.Size.Height + centerOfRadarY,
                        2, 2, _gameCore.Rendering.Materials.Raw.Green);
                }
            }
        }

        /// <summary>
        /// Will render the current game state to a single bitmap. If a lock cannot be acquired
        /// for drawing then the previous frame will be returned.
        /// </summary>
        /// <returns></returns>
        public void RenderPreScaling(SharpDX.Direct2D1.RenderTarget renderTarget)
        {
            //Render to display:
            foreach (var sprite in Collection.Where(o => o.Visable == true))
            {
                if (sprite is SpriteTextBlock spriteTextBlock)
                {
                    if (spriteTextBlock.IsFixedPosition == true)
                    {
                        continue; //We want to add these later so they are not scaled.
                    }
                }

                if (sprite.IsWithinCurrentScaledScreenBounds)
                {
                    sprite.Render(renderTarget);
                }
            }

            _gameCore.Player.Sprite?.Render(renderTarget);
            _gameCore.Menus.Render(renderTarget);

            if (_gameCore.Settings.HighlightNatrualBounds)
            {
                //Highlight the 1:1 frame
                _gameCore.Rendering.DrawRectangleAt(renderTarget, _gameCore.Display.NatrualScreenBounds.ToRawRectangleF(), 0, _gameCore.Rendering.Materials.Raw.Red, 0, 1);
            }
        }
    }
}
