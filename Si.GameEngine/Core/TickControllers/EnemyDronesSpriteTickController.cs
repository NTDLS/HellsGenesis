using Si.GameEngine.Core;
using Si.GameEngine.Core.Managers;
using Si.GameEngine.Core.TickControllers._Superclass;
using Si.GameEngine.Sprites.Enemies._Superclass;
using Si.Shared;
using Si.Shared.Types.Geometry;
using System;
using System.Linq;

namespace Si.GameEngine.Core.TickControllers
{
    public class EnemyDronesSpriteTickController : SpriteTickControllerBase<SpriteEnemyBase>
    {
        private readonly Engine _gameCore;

        public EnemyDronesSpriteTickController(Engine gameCore, EngineSpriteManager manager)
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
                    _gameCore.Multiplay.RecordDroneActionVector(multiplayVector);
                }

                enemy.ApplyMotion(displacementVector);
                enemy.RenewableResources.RenewAllResources();
            }
        }

        public T Create<T>() where T : SpriteEnemyBase
        {
            object[] param = { GameCore };
            SpriteEnemyBase obj = (SpriteEnemyBase)Activator.CreateInstance(typeof(T), param);

            obj.Location = GameCore.Display.RandomOffScreenLocation();
            obj.Velocity.MaxSpeed = SiRandom.Generator.Next(GameCore.Settings.MinEnemySpeed, GameCore.Settings.MaxEnemySpeed);
            obj.Velocity.Angle.Degrees = SiRandom.Generator.Next(0, 360);

            obj.BeforeCreate();
            SpriteManager.Add(obj);
            obj.AfterCreate();

            return (T)obj;
        }
    }
}
