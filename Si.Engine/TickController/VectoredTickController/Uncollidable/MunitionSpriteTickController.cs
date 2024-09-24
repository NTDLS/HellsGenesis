using NTDLS.DelegateThreadPooling;
using Si.Engine;
using Si.Engine.Manager;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite._Superclass._Root;
using Si.Engine.Sprite.Player._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Engine.TickController._Superclass;
using Si.Library;
using Si.Library.Mathematics;
using System.Collections.Concurrent;
using System.Linq;
using static Si.Library.SiConstants;

namespace Si.GameEngine.TickController.VectoredTickController.Uncollidable
{
    public class MunitionSpriteTickController : VectoredTickControllerBase<MunitionBase>
    {
        #region Private Classes.

        private struct MunitionObjectHit
        {
            public SpriteBase Object { get; set; }
            public MunitionBase Munition { get; set; }

            public MunitionObjectHit(MunitionBase munition, SpriteBase obj)
            {
                Object = obj;
                Munition = munition;
            }
        }

        #endregion

        private readonly DelegateThreadPool _munitionTraversalThreadPool;

        public MunitionSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
            _munitionTraversalThreadPool = new(engine.Settings.MunitionTraversalThreads);

            engine.OnShutdown += (engine) =>
            {
                _munitionTraversalThreadPool.Stop();
            };
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector displacementVector)
        {
            var munitions = VisibleOfType<MunitionBase>();
            if (munitions.Count() != 0)
            {
                var interactiveSprites = SpriteManager.VisibleDamageable();
                var objectsPlayerCanHit = interactiveSprites.Where(o => o is not SpritePlayerBase).ToArray();
                var objectsEnemyCanHit = interactiveSprites.Where(o => o is SpritePlayerBase).ToArray();

                //Create a collection of threads so we can wait on the ones that we start.
                var threadPoolTracker = _munitionTraversalThreadPool.CreateChildQueue();

                var hitObjects = new ConcurrentBag<MunitionObjectHit>();

                foreach (var munition in munitions)
                {
                    if (munition.IsDeadOrExploded == false)
                    {
                        threadPoolTracker.Enqueue(() => //Enqueue an item into the thread pool.
                        {
                            munition.ApplyMotion(epoch, displacementVector); //Move the munition.
                            munition.ApplyIntelligence(epoch, displacementVector);

                            var hitObject = munition.FindFirstReverseCollisionAlongMovementVectorAABB(munition.FiredFromType == SiFiredFromType.Player ? objectsPlayerCanHit : objectsEnemyCanHit, epoch);
                            if (hitObject != null)
                            {
                                hitObjects.Add(new(munition, hitObject));
                            }
                        });
                    }
                }

                //Wait on all enqueued threads to complete.
                if (SiUtility.TryAndIgnore(() => threadPoolTracker.WaitForCompletion()) == false)
                {
                    return;
                }

                //Take actions with the munitions that hit objects.
                foreach (var hitObject in hitObjects)
                {
                    if (hitObject.Object.IsDeadOrExploded == false)
                    {
                        hitObject.Munition.Explode();
                        hitObject.Object.MunitionHit(hitObject.Munition);
                    }
                }
            }
        }

        public void Add(WeaponBase weapon)
        {
            var obj = weapon.CreateMunition();
            SpriteManager.Add(obj);
        }

        public void Add(WeaponBase weapon, SiVector? location = null)
        {
            var obj = weapon.CreateMunition(location);
            SpriteManager.Add(obj);
        }

        /// <summary>
        /// Creates a munition that is locked on to another sprite.
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="lockedTarget"></param>
        /// <param name="xyOffset"></param>
        /// <returns></returns>
        public void AddLockedOnTo(WeaponBase weapon, SpriteInteractiveBase lockedTarget, SiVector? location = null)
        {
            var obj = weapon.CreateMunition(location, lockedTarget);
            SpriteManager.Add(obj);
        }
    }
}
