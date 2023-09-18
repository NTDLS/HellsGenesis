using HG.Controller;
using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Menus;
using HG.Sprites;
using HG.Sprites.Enemies;
using HG.Sprites.PowerUp;
using HG.Utility.ExtensionMethods;
using HG.Weapons.Bullets;
using System.Collections.Generic;
using System.Linq;

namespace HG.Managers
{
    /// <summary>
    /// Contains the collection of all sprites and their factories.
    /// </summary>
    internal class EngineSpriteManager
    {
        private readonly EngineCore _core;
        private HgPoint _radarScale;
        private HgPoint _radarOffset;

        public SpriteTextBlock PlayerStatsText { get; private set; }
        public SpriteTextBlock DebugText { get; private set; }
        public bool RenderRadar { get; set; } = false;

        #region Sprites and their factories.

        internal List<SpriteBase> Collection { get; private set; } = new();

        public AnimationSpriteTickController Animations { get; set; }
        public AttachmentSpriteTickController Attachments { get; set; }
        public BulletSpriteTickController Bullets { get; set; }
        public DebugSpriteTickController Debugs { get; set; }
        public EnemySpriteTickController Enemies { get; set; }
        public ParticleSpriteTickController Particles { get; set; }
        public PowerupSpriteTickController Powerups { get; set; }
        public RadarPositionSpriteTickController RadarPositions { get; set; }
        public StarSpriteTickController Stars { get; set; }
        public TextBlockSpriteTickController TextBlocks { get; set; }

        #endregion

        public EngineSpriteManager(EngineCore core)
        {
            _core = core;

            Animations = new AnimationSpriteTickController(_core, this);
            Attachments = new AttachmentSpriteTickController(_core, this);
            Bullets = new BulletSpriteTickController(_core, this);
            Debugs = new DebugSpriteTickController(_core, this);
            Enemies = new EnemySpriteTickController(_core, this);
            Particles = new ParticleSpriteTickController(_core, this);
            Powerups = new PowerupSpriteTickController(_core, this);
            RadarPositions = new RadarPositionSpriteTickController(_core, this);
            Stars = new StarSpriteTickController(_core, this);
            TextBlocks = new TextBlockSpriteTickController(_core, this);
        }

        public void Start()
        {
            _core.Player.Sprite = new SpritePlayer(_core, _core.PrefabPlayerLoadouts.GetDefault()) { Visable = false };

            PlayerStatsText = TextBlocks.Create(_core.DirectX.TextFormats.RealtimePlayerStats, _core.DirectX.Materials.Brushes.WhiteSmoke, new HgPoint(5, 5), true);
            PlayerStatsText.Visable = false;
            DebugText = TextBlocks.Create(_core.DirectX.TextFormats.RealtimePlayerStats, _core.DirectX.Materials.Brushes.Cyan, new HgPoint(5, PlayerStatsText.Y + 80), true);

            _core.Audio.BackgroundMusicSound.Play();
        }

        public void Stop()
        {
        }

        public void CleanupDeletedObjects()
        {
            lock (Collection)
            {
                _core.Sprites.Collection.Where(o => o.ReadyForDeletion).ToList().ForEach(p => p.Cleanup());
                _core.Sprites.Collection.RemoveAll(o => o.ReadyForDeletion);

                for (int i = 0; i < _core.Events.Collection.Count; i++)
                {
                    if (_core.Events.Collection[i].ReadyForDeletion)
                    {
                        _core.Events.Delete(_core.Events.Collection[i]);
                    }
                }

                _core.Menus.CleanupDeletedObjects();

                if (_core.Player.Sprite.IsDead)
                {
                    _core.Player.Sprite.Visable = false;
                    _core.Player.Sprite.IsDead = false;
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

        public T GetSpriteByAssetTag<T>(string name) where T : SpriteBase
        {
            lock (Collection)
            {
                return Collection.Where(o => o.Name == name).SingleOrDefault() as T;
            }
        }

        public List<T> OfType<T>() where T : class
        {
            lock (Collection)
            {
                return _core.Sprites.Collection.Where(o => o is T).Select(o => o as T).ToList();
            }
        }

        public List<T> VisibleOfType<T>() where T : class
        {
            lock (Collection)
            {
                return _core.Sprites.Collection.Where(o => o is T && o.Visable == true).Select(o => o as T).ToList();
            }
        }

        public void DeleteAllSpriteByAssetTag(string name)
        {
            lock (Collection)
            {
                foreach (var sprite in Collection)
                {
                    if (sprite.Name == name)
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
                        if (obj.Intersects(with.Location, new HgPoint(with.Size.Width, with.Size.Height)))
                        {
                            objs.Add(obj);
                        }
                    }
                }
                return objs;
            }
        }

        public List<SpriteBase> Intersections(double x, double y, double width, double height)
            => Intersections(new HgPoint(x, y), new HgPoint(width, height));

        public List<SpriteBase> Intersections(HgPoint location, HgPoint size)
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

        public SpritePlayer InsertPlayer(SpritePlayer sprite)
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
                var radarBgImage = _core.Assets.GetBitmap(@"Graphics\RadarTransparent.png");

                _core.DirectX.DrawBitmapAt(renderTarget, radarBgImage,
                    _core.Display.NatrualScreenSize.Width - radarBgImage.Size.Width,
                    _core.Display.NatrualScreenSize.Height - radarBgImage.Size.Height,
                    0);

                double radarDistance = 5;

                if (_radarScale == null)
                {
                    double radarVisionWidth = _core.Display.TotalCanvasSize.Width * radarDistance;
                    double radarVisionHeight = _core.Display.TotalCanvasSize.Height * radarDistance;

                    _radarScale = new HgPoint(radarBgImage.Size.Width / radarVisionWidth, radarBgImage.Size.Height / radarVisionHeight);
                    _radarOffset = new HgPoint(radarBgImage.Size.Width / 2.0, radarBgImage.Size.Height / 2.0); //Best guess until player is visible.
                }

                if (_core.Player.Sprite is not null && _core.Player.Sprite.Visable)
                {
                    double centerOfRadarX = (int)(radarBgImage.Size.Width / 2.0) - 2.0; //Subtract half the dot size.
                    double centerOfRadarY = (int)(radarBgImage.Size.Height / 2.0) - 2.0; //Subtract half the dot size.

                    _radarOffset = new HgPoint(
                            _core.Display.NatrualScreenSize.Width - radarBgImage.Size.Width + (centerOfRadarX - _core.Player.Sprite.X * _radarScale.X),
                            _core.Display.NatrualScreenSize.Height - radarBgImage.Size.Height + (centerOfRadarY - _core.Player.Sprite.Y * _radarScale.Y)
                        );

                    //Render radar:
                    foreach (var sprite in Collection.Where(o => o.Visable == true))
                    {
                        //HgPoint scale, HgPoint< double > offset
                        int x = (int)(_radarOffset.X + sprite.X * _radarScale.X);
                        int y = (int)(_radarOffset.Y + sprite.Y * _radarScale.Y);

                        if (x > _core.Display.NatrualScreenSize.Width - radarBgImage.Size.Width
                            && x < _core.Display.NatrualScreenSize.Width - radarBgImage.Size.Width + radarBgImage.Size.Width
                            && y > _core.Display.NatrualScreenSize.Height - radarBgImage.Size.Height
                            && y < _core.Display.NatrualScreenSize.Height - radarBgImage.Size.Height + radarBgImage.Size.Height
                            )
                        {
                            if ((sprite is SpriteEnemyBase || sprite is BulletBase || sprite is SpritePowerUpBase) && sprite.Visable == true)
                            {
                                sprite.RenderRadar(renderTarget, x, y);
                            }
                        }
                    }

                    //Render player blip:
                    _core.DirectX.FillEllipseAt(
                        renderTarget,
                        _core.Display.NatrualScreenSize.Width - radarBgImage.Size.Width + centerOfRadarX,
                        _core.Display.NatrualScreenSize.Height - radarBgImage.Size.Height + centerOfRadarY,
                        2, 2, _core.DirectX.Materials.Raw.Green);
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

                if (_core.Display.CurrentScaledScreenBounds.IntersectsWith(sprite.Bounds))
                {
                    sprite.Render(renderTarget);
                }
            }

            _core.Player.Sprite?.Render(renderTarget);
            _core.Menus.Render(renderTarget);

            if (_core.Settings.HighlightNatrualBounds)
            {
                //Highlight the 1:1 frame
                _core.DirectX.DrawRectangleAt(renderTarget, _core.Display.NatrualScreenBounds.ToRawRectangleF(), 0, _core.DirectX.Materials.Raw.Red, 0, 1);
            }
        }
    }
}
