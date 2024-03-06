using Si.Engine.Managers;
using Si.Engine.Sprites.Enemies._Superclass;
using Si.Engine.TickControllers._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System;

namespace Si.Engine.TickControllers.SpriteTickController
{
    public class EnemiesSpriteTickController : SpriteTickControllerBase<SpriteEnemyBase>
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
            object[] param = { GameEngine };
            SpriteEnemyBase obj = (SpriteEnemyBase)Activator.CreateInstance(typeof(T), param);

            obj.Location = GameEngine.Display.RandomOffScreenLocation();
            obj.Velocity.Angle.Degrees = SiRandom.Between(0, 359);

            obj.BeforeCreate();
            SpriteManager.Add(obj);
            obj.AfterCreate();

            return (T)obj;
        }
    }
}
