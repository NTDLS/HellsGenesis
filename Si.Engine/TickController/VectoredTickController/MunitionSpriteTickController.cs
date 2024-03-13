using NTDLS.DelegateThreadPooling;
using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Enemy.Boss._Superclass;
using Si.Engine.Sprite.Enemy.Peon._Superclass;
using Si.Engine.Sprite.Player._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Engine.TickController._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

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
                var objectsThatCanBeHit = SpriteManager.VisibleOfTypes([
                    typeof(SpritePlayerBase),
                    typeof(SpriteEnemyBossBase),
                    typeof(SpriteEnemyPeonBase),
                    typeof(SpriteAttachment)
                    ]);

                //Create a collection of threads so we can wait on the ones that we start.
                var threadPoolTracker = _munitionTraversalThreadPool.CreateQueueStateTracker();

                var hitObjects = new ConcurrentBag<MunitionObjectHit>();

                foreach (var munition in munitions)
                {
                    munition.ApplyMotion(epoch, displacementVector); //Move the munition.
                    munition.ApplyIntelligence(epoch, displacementVector);

                    if (munition.IsDeadOrExploded == false)
                    {
                        //Enqueue an item into the thread pool.
                        threadPoolTracker.Enqueue(() =>
                        {
                            var hitObject = TestObjectCollisionsAlongMunitionPath(munition, objectsThatCanBeHit);
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

        /// <summary>
        /// Takes the position of a munition object after it has been moved and tests each location
        ///     betwwen where it ended up and where it should have come from given its velocity.
        /// </summary>
        /// <returns></returns>
        public SpriteBase TestObjectCollisionsAlongMunitionPath(MunitionBase munition, List<SpriteBase> objectsThatCanBeHit)
        {
            //Reverse the munition to its starting position.
            var hitTestPosition = new SiPoint(munition.Location - munition.Velocity.MovementVector);

            //Hit-test each position along the munitions path.
            for (int i = 0; i < munition.Velocity.MaximumSpeed; i++)
            {
                hitTestPosition += munition.Velocity.ForwardAngle;

                foreach (var obj in objectsThatCanBeHit)
                {
                    if (obj.TryMunitionHit(munition, hitTestPosition))
                    {
                        return obj;
                    }
                }
            }

            return null;
        }

        public void Create(WeaponBase weapon)
        {
            var obj = weapon.CreateMunition();
            SpriteManager.Add(obj);
        }

        public void Create(WeaponBase weapon, SiPoint location = null, float? angle = null)
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
        public void CreateLockedOnTo(WeaponBase weapon, SpriteBase lockedTarget, SiPoint location = null, float? angle = null)
        {
            var obj = weapon.CreateMunition(location, angle, lockedTarget);
            SpriteManager.Add(obj);
        }
    }
}
