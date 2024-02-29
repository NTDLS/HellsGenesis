using Si.GameEngine.Managers;
using Si.GameEngine.Sprites.Enemies._Superclass;
using Si.GameEngine.TickControllers._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System;
using System.Linq;

namespace Si.GameEngine.TickControllers
{
    public class EnemiesSpriteTickController : SpriteTickControllerBase<SpriteEnemyBase>
    {
        private readonly GameEngineCore _gameEngine;

        public EnemiesSpriteTickController(GameEngineCore gameEngine, EngineSpriteManager manager)
            : base(gameEngine, manager)
        {
            _gameEngine = gameEngine;
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector displacementVector)
        {
            foreach (var enemy in Visible().Where(o => o.IsDrone == false))
            {
                enemy.ApplyIntelligence(epoch, displacementVector);

                var multiplayVector = enemy.GetMultiplayVector();
                if (multiplayVector != null)
                {
                    _gameEngine.Multiplay.RecordDroneActionVector(multiplayVector);
                }

                enemy.ApplyMotion(epoch, displacementVector);
                enemy.RenewableResources.RenewAllResources();
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

            if (obj.IsDrone == false)
            {
                SpriteManager.MultiplayNotifyOfSpriteCreation(obj);
            }

            return (T)obj;
        }
    }
}
