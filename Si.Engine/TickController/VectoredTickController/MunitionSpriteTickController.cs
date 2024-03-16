using NTDLS.DelegateThreadPooling;
using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Enemy.Boss._Superclass;
using Si.Engine.Sprite.Enemy.Peon._Superclass;
using Si.Engine.Sprite.Enemy.Starbase._Superclass;
using Si.Engine.Sprite.Player._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Engine.TickController._Superclass;
using Si.GameEngine.Manager;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System.Collections.Concurrent;
using System.Linq;
using static Si.Library.SiConstants;

namespace Si.Engine.TickController.VectoredTickControllerBase
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

        public override void ExecuteWorldClockTick(float epoch, SiPoint displacementVector)
        {
            var munitions = VisibleOfType<MunitionBase>();
            if (munitions.Count != 0)
            {
                var interactiveSprites = SpriteManager.VisibleOfType<SpriteInteractiveBase>()
                    .Where(o => o.TakesDamage == true).ToList<SpriteBase>();

                var objectsPlayerCanHit = interactiveSprites.OfTypes([
                    typeof(SpriteEnemyBossBase),
                    typeof(SpriteEnemyPeonBase),
                    typeof(SpriteAttachment),
                    typeof(SpriteDebug),
                    typeof(SpriteEnemyStarbase)
                ]);

                var objectsEnemyCanHit = interactiveSprites.OfTypes([
                    typeof(SpritePlayerBase)
                ]);

                //Create a collection of threads so we can wait on the ones that we start.
                var threadPoolTracker = _munitionTraversalThreadPool.CreateQueueStateTracker();

                var hitObjects = new ConcurrentBag<MunitionObjectHit>();

                foreach (var munition in munitions)
                {
                    if (munition.IsDeadOrExploded == false)
                    {
                        threadPoolTracker.Enqueue(() => //Enqueue an item into the thread pool.
                        {
                            munition.ApplyMotion(epoch, displacementVector); //Move the munition.
                            munition.ApplyIntelligence(epoch, displacementVector);

                            var dd = munition.FiredFromType == SiFiredFromType.Player ? objectsPlayerCanHit : objectsEnemyCanHit;

                            var hitObject = munition.FindFirstReverseCollisionAlongMovementVector(munition.FiredFromType == SiFiredFromType.Player ? objectsPlayerCanHit : objectsEnemyCanHit, epoch);
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

        public void Add(WeaponBase weapon, SiPoint location = null, float? angle = null)
        {
            var obj = weapon.CreateMunition(location, angle);
            SpriteManager.Add(obj);
        }

        /// <summary>
        /// Creates a munition that is locked on to another sprite.
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="lockedTarget"></param>
        /// <param name="xyOffset"></param>
        /// <returns></returns>
        public void AddLockedOnTo(WeaponBase weapon, SpriteInteractiveBase lockedTarget, SiPoint location = null, float? angle = null)
        {
            var obj = weapon.CreateMunition(location, angle, lockedTarget);
            SpriteManager.Add(obj);
        }
    }
}
