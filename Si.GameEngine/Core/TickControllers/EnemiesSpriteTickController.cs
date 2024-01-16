using Si.GameEngine.Core.Managers;
using Si.GameEngine.Core.TickControllers._Superclass;
using Si.GameEngine.Sprites.Enemies._Superclass;
using Si.Library;
using Si.Library.Types.Geometry;
using System;
using System.Linq;

namespace Si.GameEngine.Core.TickControllers
{
    public class EnemiesSpriteTickController : SpriteTickControllerBase<SpriteEnemyBase>
    {
        private readonly GameEngineCore _gameEngine;

        public EnemiesSpriteTickController(GameEngineCore gameEngine, EngineSpriteManager manager)
            : base(gameEngine, manager)
        {
            _gameEngine = gameEngine;
        }

        public override void ExecuteWorldClockTick(SiPoint displacementVector)
        {
            foreach (var enemy in Visible().Where(o => o.IsDrone == false))
            {
                if (GameEngine.Player.Sprite.Visable)
                {
                    enemy.ApplyIntelligence(displacementVector);
                    GameEngine.Player.Sprite.SelectedSecondaryWeapon?.ApplyWeaponsLock(enemy); //Player lock-on to enemy. :D
                }

                var multiplayVector = enemy.GetMultiplayVector();
                if (multiplayVector != null)
                {
                    _gameEngine.Multiplay.RecordDroneActionVector(multiplayVector);
                }

                enemy.ApplyMotion(displacementVector);
                enemy.RenewableResources.RenewAllResources();
            }
        }

        public T Create<T>() where T : SpriteEnemyBase
        {
            object[] param = { GameEngine };
            SpriteEnemyBase obj = (SpriteEnemyBase)Activator.CreateInstance(typeof(T), param);

            obj.Location = GameEngine.Display.RandomOffScreenLocation();
            obj.Velocity.MaxSpeed = SiRandom.Generator.Next(GameEngine.Settings.MinEnemySpeed, GameEngine.Settings.MaxEnemySpeed);
            obj.Velocity.Angle.Degrees = SiRandom.Generator.Next(0, 360);

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
