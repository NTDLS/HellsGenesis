using NTDLS.Semaphore;
using SharpDX.Mathematics.Interop;
using Si.GameEngine.Core.TickControllers;
using Si.GameEngine.Menus;
using Si.GameEngine.Sprites;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Enemies._Superclass;
using Si.GameEngine.Sprites.Player;
using Si.GameEngine.Sprites.Player._Superclass;
using Si.GameEngine.Sprites.Powerup._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.Library;
using Si.Library.Payload;
using Si.Library.Types.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Core.Managers
{
    /// <summary>
    /// Contains the collection of all sprites and their factories.
    /// </summary>
    public class EngineSpriteManager : IDisposable
    {
        public delegate void CollectionAccessor(List<SpriteBase> sprites);
        public delegate T CollectionAccessorT<T>(List<SpriteBase> sprites);

        private readonly GameEngineCore _gameEngine;
        private SiPoint _radarScale;
        private SiPoint _radarOffset;

        public SpriteTextBlock PlayerStatsText { get; private set; }
        public SpriteTextBlock DebugText { get; private set; }
        public bool RenderRadar { get; set; } = false;

        #region Sprites and their factories.

        private readonly PessimisticCriticalResource<List<SpriteBase>> _collection = new();
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
        public EnemyDronesSpriteTickController EnemyDrones { get; private set; }

        #endregion

        public EngineSpriteManager(GameEngineCore gameEngine)
        {
            _gameEngine = gameEngine;

            Animations = new AnimationSpriteTickController(_gameEngine, this);
            Attachments = new AttachmentSpriteTickController(_gameEngine, this);
            Munitions = new MunitionSpriteTickController(_gameEngine, this);
            Debugs = new DebugsSpriteTickController(_gameEngine, this);
            Enemies = new EnemiesSpriteTickController(_gameEngine, this);
            Particles = new ParticlesSpriteTickController(_gameEngine, this);
            Powerups = new PowerupsSpriteTickController(_gameEngine, this);
            RadarPositions = new RadarPositionsSpriteTickController(_gameEngine, this);
            Stars = new StarsSpriteTickController(_gameEngine, this);
            TextBlocks = new TextBlocksSpriteTickController(_gameEngine, this);
            PlayerDrones = new PlayerDronesSpriteTickController(_gameEngine, this);
            EnemyDrones = new EnemyDronesSpriteTickController(_gameEngine, this);
        }

        public List<SpritePlayerBase> AllVisiblePlayers
        {
            get
            {
                var players = VisibleOfType<SpritePlayerBase>();
                players.Add(_gameEngine.Player.Sprite);
                return players;
            }
        }

        public void Add(SpriteBase item)
            => _collection.Use(o => o.Add(item));

        public void MultiplayNotifyOfSpriteCreation(SpriteBase item)
        {
            //If this is not a drone, then tell the server about its creation.
            var fullTypeName = item.GetType().Name;
            if (fullTypeName.EndsWith("Drone") == false)
            {
                _gameEngine.Multiplay.NotifySpriteCreated(new SiSpriteLayout()
                {
                    FullTypeName = fullTypeName,
                    MultiplayUID = item.MultiplayUID,
                    Vector = new SiSpriteVector
                    {
                        X = item.X,
                        Y = item.Y,
                        AngleDegrees = item.Velocity.Angle.Degrees,
                        BoostPercentage = item.Velocity.BoostPercentage,
                        ThrottlePercentage = item.Velocity.ThrottlePercentage,
                        MaxBoost = item.Velocity.MaxBoost,
                        MaxSpeed = item.Velocity.MaxSpeed,
                    }
                });
            }
        }

        public void Delete(SpriteBase item)
        {
            _collection.Use(o =>
            {
                item.Cleanup();
                o.Remove(item);
            });
        }

        public void Use(CollectionAccessor collectionAccessor)
            => _collection.Use(o => collectionAccessor(o));

        public T Use<T>(CollectionAccessorT<T> collectionAccessor)
            => _collection.Use(o => collectionAccessor(o));

        public void DeleteAllOfType<T>() where T : SpriteBase
        {
            _collection.Use(o =>
            {
                OfType<T>().ForEach(c => c.QueueForDelete());
            });
        }

        public void Start()
        {
            _gameEngine.Player.Sprite = new SpriteDebugPlayer(_gameEngine) { Visable = false };

            PlayerStatsText = TextBlocks.Create(_gameEngine.Rendering.TextFormats.RealtimePlayerStats, _gameEngine.Rendering.Materials.Brushes.WhiteSmoke, new SiPoint(5, 5), true);
            PlayerStatsText.Visable = false;
            DebugText = TextBlocks.Create(_gameEngine.Rendering.TextFormats.RealtimePlayerStats, _gameEngine.Rendering.Materials.Brushes.Cyan, new SiPoint(5, PlayerStatsText.Y + 100), true);
        }

        public void Dispose()
        {
        }

        public SpriteBase CreateByNameOfType(string typeFullName)
        {
            var type = Type.GetType(typeFullName) ?? throw new ArgumentException($"Type with FullName '{typeFullName}' not found.");
            object[] param = { _gameEngine };
            var obj = (SpriteBase)Activator.CreateInstance(type, param);

            obj.Location = _gameEngine.Display.RandomOffScreenLocation();
            obj.Velocity.MaxSpeed = SiRandom.Between(_gameEngine.Settings.MinEnemySpeed, _gameEngine.Settings.MaxEnemySpeed);
            obj.Velocity.Angle.Degrees = SiRandom.Between(0, 359);

            var enemy = obj as SpriteEnemyBase;

            enemy?.BeforeCreate();
            Add(obj);
            enemy?.AfterCreate();

            return obj;
        }

        public void CleanupDeletedObjects()
        {
            _collection.Use(o =>
            {
                o.Where(o => o.IsQueuedForDeletion).ToList().ForEach(p => p.Cleanup());
                o.RemoveAll(o => o.IsQueuedForDeletion);

                _gameEngine.Events.CleanupQueuedForDeletion();

                _gameEngine.Menus.CleanupDeletedObjects();

                if (_gameEngine.Player.Sprite.IsDeadOrExploded)
                {
                    _gameEngine.Player.Sprite.Visable = false;
                    _gameEngine.Player.Sprite.ReviveDeadOrExploded();
                    _gameEngine.Menus.Add(new MenuStartNewGame(_gameEngine));
                }
            });
        }

        public void DeleteAll()
        {
            Powerups.DeleteAll();
            Enemies.DeleteAll();
            Munitions.DeleteAll();
            Animations.DeleteAll();
        }

        public T GetSpriteByTag<T>(string name) where T : SpriteBase
            => _collection.Use(o => o.Where(o => o.SpriteTag == name).SingleOrDefault() as T);

        public List<T> OfType<T>() where T : class
            => _collection.Use(o => o.Where(o => o is T).Select(o => o as T).ToList());

        public List<T> VisibleOfType<T>() where T : class
                => _collection.Use(o => o.Where(o => o is T && o.Visable == true).Select(o => o as T).ToList());

        public void DeleteAllSpritesByTag(string name)
        {
            _collection.Use(o =>
            {
                foreach (var sprite in o)
                {
                    if (sprite.SpriteTag == name)
                    {
                        sprite.QueueForDelete();
                    }
                }
            });
        }

        public List<SpriteBase> Intersections(SpriteBase with)
        {
            return _collection.Use(o =>
            {
                var objs = new List<SpriteBase>();

                foreach (var obj in o.Where(o => o.Visable == true))
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
            });
        }

        public List<SpriteBase> Intersections(double x, double y, double width, double height)
            => Intersections(new SiPoint(x, y), new SiPoint(width, height));

        public List<SpriteBase> Intersections(SiPoint location, SiPoint size)
        {
            return _collection.Use(o =>
            {
                var objs = new List<SpriteBase>();

                foreach (var obj in o.Where(o => o.Visable == true))
                {
                    if (obj.Intersects(location, size))
                    {
                        objs.Add(obj);
                    }
                }
                return objs;
            });
        }

        public List<SpriteBase> RenderLocationIntersections(SiPoint location, SiPoint size)
        {
            return _collection.Use(o =>
            {
                var objs = new List<SpriteBase>();

                foreach (var obj in o.Where(o => o.Visable == true))
                {
                    if (obj.RenderLocationIntersects(location, size))
                    {
                        objs.Add(obj);
                    }
                }
                return objs;
            });
        }

        public SpritePlayerBase AddPlayer(SpritePlayerBase sprite)
        {
            Add(sprite);
            return sprite;
        }

        public void RenderPostScaling(SharpDX.Direct2D1.RenderTarget renderTarget)
        {
            _collection.Use(o => //Render PostScale sprites.
            {
                foreach (var sprite in o.Where(o => o.Visable == true && o.RenderScaleOrder == SiRenderScaleOrder.PostScale))
                {
                    sprite.Render(renderTarget);
                }
            });

            if (RenderRadar)
            {
                var radarBgImage = _gameEngine.Assets.GetBitmap(@"Graphics\RadarTransparent.png");

                _gameEngine.Rendering.DrawBitmapAt(renderTarget, radarBgImage,
                    _gameEngine.Display.NatrualScreenSize.Width - radarBgImage.Size.Width,
                    _gameEngine.Display.NatrualScreenSize.Height - radarBgImage.Size.Height,
                    0);

                double radarDistance = 8;

                if (_radarScale == null)
                {
                    double radarVisionWidth = _gameEngine.Display.TotalCanvasSize.Width * radarDistance;
                    double radarVisionHeight = _gameEngine.Display.TotalCanvasSize.Height * radarDistance;

                    _radarScale = new SiPoint(radarBgImage.Size.Width / radarVisionWidth, radarBgImage.Size.Height / radarVisionHeight);
                    _radarOffset = new SiPoint(radarBgImage.Size.Width / 2.0, radarBgImage.Size.Height / 2.0); //Best guess until player is visible.
                }

                if (_gameEngine.Player.Sprite is not null && _gameEngine.Player.Sprite.Visable)
                {
                    double centerOfRadarX = (int)(radarBgImage.Size.Width / 2.0) - 2.0; //Subtract half the dot size.
                    double centerOfRadarY = (int)(radarBgImage.Size.Height / 2.0) - 2.0; //Subtract half the dot size.

                    _radarOffset = new SiPoint(
                            _gameEngine.Display.NatrualScreenSize.Width - radarBgImage.Size.Width + (centerOfRadarX - _gameEngine.Player.Sprite.X * _radarScale.X),
                            _gameEngine.Display.NatrualScreenSize.Height - radarBgImage.Size.Height + (centerOfRadarY - _gameEngine.Player.Sprite.Y * _radarScale.Y)
                        );

                    _collection.Use(o =>
                    {
                        //Render radar:
                        foreach (var sprite in o.Where(o => o.Visable == true))
                        {
                            //SiPoint scale, SiPoint< double > offset
                            int x = (int)(_radarOffset.X + sprite.Location.X * _radarScale.X);
                            int y = (int)(_radarOffset.Y + sprite.Location.Y * _radarScale.Y);

                            if (x > _gameEngine.Display.NatrualScreenSize.Width - radarBgImage.Size.Width
                                && x < _gameEngine.Display.NatrualScreenSize.Width - radarBgImage.Size.Width + radarBgImage.Size.Width
                                && y > _gameEngine.Display.NatrualScreenSize.Height - radarBgImage.Size.Height
                                && y < _gameEngine.Display.NatrualScreenSize.Height - radarBgImage.Size.Height + radarBgImage.Size.Height
                                )
                            {
                                if ((sprite is SpritePlayerBase || sprite is SpriteEnemyBase || sprite is MunitionBase || sprite is SpritePowerupBase) && sprite.Visable == true)
                                {
                                    sprite.RenderRadar(renderTarget, x, y);
                                }
                            }
                        }
                    });

                    //Render player blip:
                    _gameEngine.Rendering.FillEllipseAt(
                        renderTarget,
                        _gameEngine.Display.NatrualScreenSize.Width - radarBgImage.Size.Width + centerOfRadarX,
                        _gameEngine.Display.NatrualScreenSize.Height - radarBgImage.Size.Height + centerOfRadarY,
                        2, 2, _gameEngine.Rendering.Materials.Colors.Green);
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
            _collection.Use(o => //Render PreScale sprites.
            {
                foreach (var sprite in o.Where(o => o.Visable == true && o.RenderScaleOrder == SiRenderScaleOrder.PreScale))
                {
                    if (sprite.IsWithinCurrentScaledScreenBounds)
                    {
                        sprite.Render(renderTarget);
                    }
                }
            });

            _gameEngine.Player.Sprite?.Render(renderTarget);
            _gameEngine.Menus.Render(renderTarget);

            if (_gameEngine.Settings.HighlightNatrualBounds)
            {
                var natrualScreenBounds = _gameEngine.Display.NatrualScreenBounds;
                var rawRectF = new RawRectangleF(natrualScreenBounds.Left, natrualScreenBounds.Top, natrualScreenBounds.Right, natrualScreenBounds.Bottom);

                //Highlight the 1:1 frame
                _gameEngine.Rendering.DrawRectangleAt(renderTarget, rawRectF, 0, _gameEngine.Rendering.Materials.Colors.Red, 0, 1);
            }
        }
    }
}
