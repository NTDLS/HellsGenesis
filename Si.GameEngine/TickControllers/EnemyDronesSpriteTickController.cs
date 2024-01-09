using Si.GameEngine.Engine;
using Si.GameEngine.Managers;
using Si.GameEngine.Sprites.Enemies.BasesAndInterfaces;
using Si.GameEngine.TickControllers.BasesAndInterfaces;
using Si.Shared;
using Si.Shared.Types.Geometry;
using System;
using System.Linq;

namespace Si.GameEngine.Controller
{
    public class EnemyDronesSpriteTickController : SpriteTickControllerBase<SpriteEnemyBase>
    {
        private readonly EngineCore _gameCore;

        public EnemyDronesSpriteTickController(EngineCore gameCore, EngineSpriteManager manager)
            : base(gameCore, manager)
        {
            _gameCore = gameCore;
        }

        public override void ExecuteWorldClockTick(SiPoint displacementVector)
        {
            foreach (var enemy in Visible().Where(o => o.IsDrone == true))
            {
                if (GameCore.Player.Sprite.Visable)
                {
                    enemy.ApplyIntelligence(displacementVector);
                    GameCore.Player.Sprite.SelectedSecondaryWeapon?.ApplyWeaponsLock(enemy); //Player lock-on to enemy. :D
                }

                var multiplayVector = enemy.GetMultiplayVector();
                if (multiplayVector != null)
                {
                    _gameCore.Multiplay.RecordSpriteVector(multiplayVector);
                }

                enemy.ApplyMotion(displacementVector);
                enemy.RenewableResources.RenewAllResources();
            }
        }

        public T Create<T>() where T : SpriteEnemyBase
        {
            object[] param = { GameCore };
            SpriteEnemyBase obj = (SpriteEnemyBase)Activator.CreateInstance(typeof(T), param);

            obj.LocalLocation = GameCore.Display.RandomOffScreenLocation();
            obj.Velocity.MaxSpeed = SiRandom.Generator.Next(GameCore.Settings.MinEnemySpeed, GameCore.Settings.MaxEnemySpeed);
            obj.Velocity.Angle.Degrees = SiRandom.Generator.Next(0, 360);

            obj.BeforeCreate();
            SpriteManager.Add(obj);
            obj.AfterCreate();

            return (T)obj;
        }
    }
}
