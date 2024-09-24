using NTDLS.Helpers;
using Si.Engine;
using Si.Engine.Manager;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Engine.TickController._Superclass;
using Si.Library;
using Si.Library.Mathematics;
using System;

namespace Si.GameEngine.TickController.VectoredTickController.Collidable
{
    public class EnemySpriteTickController : VectoredCollidableTickControllerBase<SpriteEnemyBase>
    {
        private readonly EngineCore _engine;

        public EnemySpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
            _engine = engine;
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector displacementVector)
        {
            foreach (var enemy in Visible())
            {
                enemy.ApplyIntelligence(epoch, displacementVector);
                enemy.ApplyMotion(epoch, displacementVector);
                enemy.PerformCollisionDetection(epoch);
                enemy.RenewableResources.RenewAllResources(epoch);
            }
        }

        public T AddTypeOf<T>() where T : SpriteEnemyBase
        {
            object[] param = { Engine };
            SpriteEnemyBase obj = (SpriteEnemyBase)Activator.CreateInstance(typeof(T), param).EnsureNotNull();

            obj.Location = Engine.Display.RandomOffScreenLocation();
            obj.Orientation.Degrees = SiRandom.Between(0, 359);
            obj.RecalculateMovementVector();

            obj.BeforeCreate();
            SpriteManager.Add(obj);
            obj.AfterCreate();

            return (T)obj;
        }
    }
}
