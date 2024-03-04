using Si.GameEngine.Managers;
using Si.GameEngine.Sprites.Enemies._Superclass;
using Si.GameEngine.TickControllers._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System;
using System.Linq;

namespace Si.GameEngine.TickControllers.SpriteTickController
{
    public class EnemyDronesSpriteTickController : SpriteTickControllerBase<SpriteEnemyBase>
    {
        private readonly GameEngineCore _gameEngine;

        public EnemyDronesSpriteTickController(GameEngineCore gameEngine, EngineSpriteManager manager)
            : base(gameEngine, manager)
        {
            _gameEngine = gameEngine;
        }

        public override void ExecuteWorldClockTick(float epoch, SiPoint displacementVector)
        {
            foreach (var enemy in Visible().Where(o => o.IsDrone == true))
            {
                if (GameEngine.Player.Sprite.Visable)
                {
                    enemy.ApplyIntelligence(epoch, displacementVector);
                }

                var multiplayVector = enemy.GetMultiplayVector();
                if (multiplayVector != null)
                {
                    _gameEngine.Multiplay.RecordDroneActionVector(multiplayVector);
                }

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
