using NTDLS.Semaphore;
using SharpDX.Mathematics.Interop;
using Si.Engine.Menu;
using Si.Engine.Sprite;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Engine.Sprite.Player;
using Si.Engine.Sprite.Player._Superclass;
using Si.Engine.Sprite.PowerUp._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Engine.TickController.PlayerSpriteTickController;
using Si.Engine.TickController.SpriteTickController;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using static Si.Library.SiConstants;

namespace Si.Engine.Manager
{
    /// <summary>
    /// Contains the collection of all sprites and their factories.
    /// </summary>
    public class EngineSpriteManager : IDisposable
    {
        public delegate void CollectionAccessor(List<SpriteBase> sprites);
        public delegate T CollectionAccessorT<T>(List<SpriteBase> sprites);

        private readonly EngineCore _engine;
        private SiPoint _radarScale;
        private SiPoint _radarOffset;

        public SpriteTextBlock PlayerStatsText { get; private set; }
        public SpriteTextBlock DebugText { get; private set; }
        public bool RenderRadar { get; set; } = false;

        private readonly OptimisticCriticalResource<List<SpriteBase>> _collection = new();

        #region Sprite Tick Controllerss.

        public AnimationSpriteTickController Animations { get; private set; }
        public AttachmentSpriteTickController Attachments { get; private set; }
        public GenericSpriteTickController GenericSprites { get; private set; }
        public MunitionSpriteTickController Munitions { get; private set; }
        public DebugsSpriteTickController Debugs { get; private set; }
        public EnemiesSpriteTickController Enemies { get; private set; }
        public ParticlesSpriteTickController Particles { get; private set; }
        public PowerupsSpriteTickController Powerups { get; private set; }
        public RadarPositionsSpriteTickController RadarPositions { get; set; }
        public StarsSpriteTickController Stars { get; private set; }
        public TextBlocksSpriteTickController TextBlocks { get; private set; }
        public PlayerSpriteTickController Player { get; private set; }

        #endregion

        public EngineSpriteManager(EngineCore engine)
        {
            _engine = engine;

            Animations = new AnimationSpriteTickController(_engine, this);
            Attachments = new AttachmentSpriteTickController(_engine, this);
            Debugs = new DebugsSpriteTickController(_engine, this);
            Enemies = new EnemiesSpriteTickController(_engine, this);
            GenericSprites = new GenericSpriteTickController(_engine, this);
            Munitions = new MunitionSpriteTickController(_engine, this);
            Particles = new ParticlesSpriteTickController(_engine, this);
            Powerups = new PowerupsSpriteTickController(_engine, this);
            RadarPositions = new RadarPositionsSpriteTickController(_engine, this);
            Stars = new StarsSpriteTickController(_engine, this);
            TextBlocks = new TextBlocksSpriteTickController(_engine, this);
        }

        public List<SpritePlayerBase> AllVisiblePlayers
        {
            get
            {
                var players = VisibleOfType<SpritePlayerBase>();
                players.Add(_engine.Player.Sprite);
                return players;
            }
        }

        public void Add(SpriteBase item)
            => _engine.Events.Add(() => _collection.Write(o => o.Add(item)));

        public void HardDelete(SpriteBase item)
        {
            _collection.Write(o =>
            {
                item.Cleanup();
                o.Remove(item);
            });
        }

        public void Read(CollectionAccessor collectionAccessor)
            => _collection.Read(o => collectionAccessor(o));

        public T Read<T>(CollectionAccessorT<T> collectionAccessor)
            => _collection.Read(o => collectionAccessor(o));

        public void QueueAllForDeleteOfType<T>() where T : SpriteBase
        {
            _collection.Read(o =>
            {
                OfType<T>().ForEach(c => c.QueueForDelete());
            });
        }

        public void Start()
        {
            _engine.Player.Sprite = new SpriteDebugPlayer(_engine) { Visable = false };

            PlayerStatsText = TextBlocks.Create(_engine.Rendering.TextFormats.RealtimePlayerStats, _engine.Rendering.Materials.Brushes.WhiteSmoke, new SiPoint(5, 5), true);
            PlayerStatsText.Visable = false;
            DebugText = TextBlocks.Create(_engine.Rendering.TextFormats.RealtimePlayerStats, _engine.Rendering.Materials.Brushes.Cyan, new SiPoint(5, PlayerStatsText.Y + 100), true);
        }

        public void Dispose()
        {
        }

        public SpriteBase CreateByNameOfType(string typeFullName)
        {
            var type = Type.GetType(typeFullName) ?? throw new ArgumentException($"Type with FullName '{typeFullName}' not found.");
            object[] param = { _engine };
            var obj = (SpriteBase)Activator.CreateInstance(type, param);

            obj.Location = _engine.Display.RandomOffScreenLocation();
            obj.Velocity.ForwardAngle.Degrees = SiRandom.Between(0, 359);

            var enemy = obj as SpriteEnemyBase;

            enemy?.BeforeCreate();
            Add(obj);
            enemy?.AfterCreate();

            return obj;
        }

        public void HardDeleteQueuedDeletions()
        {
            _collection.Write(o =>
            {
                o.Where(o => o.IsQueuedForDeletion).ToList().ForEach(p => p.Cleanup());
                o.RemoveAll(o => o.IsQueuedForDeletion);

                _engine.Events.CleanupQueuedForDeletion();

                if (_engine.Player.Sprite.IsDeadOrExploded)
                {
                    _engine.Player.Sprite.Visable = false;
                    _engine.Player.Sprite.ReviveDeadOrExploded();
                    _engine.Menus.Show(new MenuStartNewGame(_engine));
                }
            });
        }

        /// <summary>
        /// Deletes all the non-background type of sprites.
        /// </summary>
        public void DeleteActionSprites()
        {
            Powerups.QueueAllForDelete();
            Enemies.QueueAllForDelete();
            Munitions.QueueAllForDelete();
            Animations.QueueAllForDelete();
        }

        public List<T> GetSpritesByTag<T>(string name) where T : SpriteBase
            => _collection.Read(o => o.Where(o => o.SpriteTag == name).ToList() as List<T>);

        public T GetSingleSpriteByTag<T>(string name) where T : SpriteBase
            => _collection.Read(o => o.Where(o => o.SpriteTag == name).SingleOrDefault() as T);

        public T GetSpriteByOwner<T>(uint ownerUID) where T : SpriteBase
            => _collection.Read(o => o.Where(o => o.UID == ownerUID).SingleOrDefault() as T);

        public List<T> OfType<T>() where T : class
            => _collection.Read(o => o.Where(o => o is T).Select(o => o as T).ToList());

        public List<T> VisibleOfType<T>() where T : class
            => _collection.Read(o => o.Where(o => o is T && o.Visable == true).Select(o => o as T).ToList());

        public List<SpriteBase> Visible() => _collection.Read(o => o.Where(o => o.Visable == true).ToList());

        public List<SpriteBase> All() => _collection.Read(o => o).ToList();

        public void QueueAllForDeleteByTag(string name)
        {
            _collection.Read(o =>
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

        public void QueueAllForDeleteByOwner(uint ownerUID)
        {
            _collection.Read(o =>
            {
                foreach (var sprite in o)
                {
                    if (sprite.OwnerUID == ownerUID)
                    {
                        sprite.QueueForDelete();
                    }
                }
            });
        }

        public List<SpriteBase> Intersections(SpriteBase with)
        {
            return _collection.Read(o =>
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

        public List<SpriteBase> Intersections(float x, float y, float width, float height)
            => Intersections(new SiPoint(x, y), new SiPoint(width, height));

        public List<SpriteBase> Intersections(SiPoint location, SiPoint size)
        {
            return _collection.Read(o =>
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

        public List<SpriteBase> RenderLocationIntersectionsEvenInvisible(SiPoint location, SiPoint size)
        {
            return _collection.Read(o =>
            {
                var objs = new List<SpriteBase>();

                foreach (var obj in o)
                {
                    if (obj.RenderLocationIntersects(location, size))
                    {
                        objs.Add(obj);
                    }
                }
                return objs;
            });
        }

        public List<SpriteBase> RenderLocationIntersections(SiPoint location, SiPoint size)
        {
            return _collection.Read(o =>
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
            _collection.Read(o => //Render PostScale sprites.
            {
                foreach (var sprite in o.Where(o => o.Visable == true && o.RenderScaleOrder == SiRenderScaleOrder.PostScale).OrderBy(o => o.ZOrder))
                {
                    sprite.Render(renderTarget);
                }
            });

            if (RenderRadar)
            {
                var radarBgImage = _engine.Assets.GetBitmap(@"Graphics\RadarTransparent.png");

                _engine.Rendering.DrawBitmapAt(renderTarget, radarBgImage,
                    _engine.Display.NatrualScreenSize.Width - radarBgImage.Size.Width,
                    _engine.Display.NatrualScreenSize.Height - radarBgImage.Size.Height, 0);

                float radarDistance = 8;

                if (_radarScale == null)
                {
                    float radarVisionWidth = _engine.Display.TotalCanvasSize.Width * radarDistance;
                    float radarVisionHeight = _engine.Display.TotalCanvasSize.Height * radarDistance;

                    _radarScale = new SiPoint(radarBgImage.Size.Width / radarVisionWidth, radarBgImage.Size.Height / radarVisionHeight);
                    _radarOffset = new SiPoint(radarBgImage.Size.Width / 2.0f, radarBgImage.Size.Height / 2.0f); //Best guess until player is visible.
                }

                if (_engine.Player.Sprite is not null && _engine.Player.Sprite.Visable)
                {
                    float centerOfRadarX = (int)(radarBgImage.Size.Width / 2.0f) - 2.0f; //Subtract half the dot size.
                    float centerOfRadarY = (int)(radarBgImage.Size.Height / 2.0f) - 2.0f; //Subtract half the dot size.

                    _radarOffset = new SiPoint(
                            _engine.Display.NatrualScreenSize.Width - radarBgImage.Size.Width + (centerOfRadarX - _engine.Player.Sprite.X * _radarScale.X),
                            _engine.Display.NatrualScreenSize.Height - radarBgImage.Size.Height + (centerOfRadarY - _engine.Player.Sprite.Y * _radarScale.Y)
                        );

                    _collection.Read(o =>
                    {
                        //Render radar:
                        foreach (var sprite in o.Where(o => o.Visable == true))
                        {
                            //SiPoint scale, SiPoint< float > offset
                            int x = (int)(_radarOffset.X + sprite.Location.X * _radarScale.X);
                            int y = (int)(_radarOffset.Y + sprite.Location.Y * _radarScale.Y);

                            if (x > _engine.Display.NatrualScreenSize.Width - radarBgImage.Size.Width
                                && x < _engine.Display.NatrualScreenSize.Width - radarBgImage.Size.Width + radarBgImage.Size.Width
                                && y > _engine.Display.NatrualScreenSize.Height - radarBgImage.Size.Height
                                && y < _engine.Display.NatrualScreenSize.Height - radarBgImage.Size.Height + radarBgImage.Size.Height
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
                    _engine.Rendering.FillEllipseAt(
                        renderTarget,
                        _engine.Display.NatrualScreenSize.Width - radarBgImage.Size.Width + centerOfRadarX,
                        _engine.Display.NatrualScreenSize.Height - radarBgImage.Size.Height + centerOfRadarY,
                        2, 2, _engine.Rendering.Materials.Colors.Green);
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
            _collection.Read(o => //Render PreScale sprites.
            {
                foreach (var sprite in o.Where(o => o.Visable == true && o.RenderScaleOrder == SiRenderScaleOrder.PreScale).OrderBy(o => o.ZOrder))
                {
                    if (sprite.IsWithinCurrentScaledScreenBounds)
                    {
                        sprite.Render(renderTarget);
                    }
                }
            });

            _engine.Player.Sprite?.Render(renderTarget);
            _engine.Menus.Render(renderTarget);

            if (_engine.Settings.HighlightNatrualBounds)
            {
                var natrualScreenBounds = _engine.Display.NatrualScreenBounds;
                var rawRectF = new RawRectangleF(natrualScreenBounds.Left, natrualScreenBounds.Top, natrualScreenBounds.Right, natrualScreenBounds.Bottom);

                //Highlight the 1:1 frame
                _engine.Rendering.DrawRectangleAt(renderTarget, rawRectF, 0, _engine.Rendering.Materials.Colors.Red, 0, 1);
            }
        }

        public void CreateFragmentsOf(SpriteBase sprite)
        {
            var image = sprite.GetImage();
            if (image == null)
            {
                return;
            }

            int fragmentCount = SiRandom.Between(2, 10);

            var fragmentImages = _engine.Rendering.GenerateIrregularFragments(image, fragmentCount);

            for (int index = 0; index < fragmentCount; index++)
            {
                var fragment = _engine.Sprites.GenericSprites.CreateAt(sprite, fragmentImages[index]);
                //TODO: Can we implement this.
                fragment.CleanupMode = ParticleCleanupMode.DistanceOffScreen;
                fragment.FadeToBlackReductionAmount = SiRandom.Between(0.001f, 0.01f);

                fragment.Velocity.ForwardAngle.Degrees = SiRandom.Between(0.0f, 359.0f);
                fragment.Velocity.MaximumSpeed = SiRandom.Between(1, 3.5f);
                fragment.Velocity.ForwardVelocity = 1;
                fragment.VectorType = ParticleVectorType.Independent;
            }
        }
    }
}
