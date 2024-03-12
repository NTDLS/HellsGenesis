using Si.Engine.Manager;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Engine.TickController._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System;

namespace Si.Engine.TickController.VectoredTickControllerBase
{
    public class EnemiesSpriteTickController : VectoredTickControllerBase<SpriteEnemyBase>
    {
        private readonly EngineCore _engine;

        public EnemiesSpriteTickController(EngineCore engine, EngineSpriteManager manager)
            : base(engine, manager)
        {
            _engine = engine;
        }

        public override void ExecuteWorldClockTick(float epoch, SiPoint displacementVector)
        {
            foreach (var enemy in Visible())
            {
                enemy.ApplyIntelligence(epoch, displacementVector);
                enemy.ApplyMotion(epoch, displacementVector);
                enemy.RenewableResources.RenewAllResources(epoch);
            }
        }

        public T Create<T>() where T : SpriteEnemyBase
        {
            object[] param = { Engine };
            SpriteEnemyBase obj = (SpriteEnemyBase)Activator.CreateInstance(typeof(T), param);

            obj.Location = Engine.Display.RandomOffScreenLocation();
            obj.Velocity.ForwardAngle.Degrees = SiRandom.Between(0, 359);

            obj.BeforeCreate();
            SpriteManager.Add(obj);
            obj.AfterCreate();

            return (T)obj;
        }
    }
}
