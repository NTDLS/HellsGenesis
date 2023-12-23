using Si.Game.Engine;
using Si.Game.Engine.Types.Geometry;
using Si.Game.Managers;
using Si.Game.Sprites.Enemies.BasesAndInterfaces;
using Si.Game.TickControllers.BasesAndInterfaces;
using Si.Game.Utility;
using System;

namespace Si.Game.Controller
{
    internal class EnemieDronesSpriteTickController : SpriteTickControllerBase<SpriteEnemyBase>
    {
        private readonly EngineCore _gameCore;

        public EnemieDronesSpriteTickController(EngineCore gameCore, EngineSpriteManager manager)
            : base(gameCore, manager)
        {
            _gameCore = gameCore;
        }

        public override void ExecuteWorldClockTick(SiPoint displacementVector)
        {
            if (GameCore.Player.Sprite != null)
            {
                GameCore.Player.Sprite.SelectedSecondaryWeapon?.LockedOnObjects.Clear();
            }

            foreach (var enemy in Visible())
            {
                foreach (var weapon in enemy.Weapons)
                {
                    weapon.LockedOnObjects.Clear();
                }

                if (GameCore.Player.Sprite.Visable)
                {
                    enemy.ApplyIntelligence(displacementVector);

                    if (GameCore.Player.Sprite.SelectedSecondaryWeapon != null)
                    {
                        GameCore.Player.Sprite.SelectedSecondaryWeapon.ApplyWeaponsLock(displacementVector, enemy); //Player lock-on to enemy. :D
                    }
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
            lock (SpriteManager.Collection)
            {
                object[] param = { GameCore };
                SpriteEnemyBase obj = (SpriteEnemyBase)Activator.CreateInstance(typeof(T), param);

                obj.Location = GameCore.Display.RandomOffScreenLocation();
                obj.Velocity.MaxSpeed = SiRandom.Generator.Next(GameCore.Settings.MinEnemySpeed, GameCore.Settings.MaxEnemySpeed);
                obj.Velocity.Angle.Degrees = SiRandom.Generator.Next(0, 360);

                obj.BeforeCreate();
                SpriteManager.Collection.Add(obj);
                obj.AfterCreate();

                return (T)obj;
            }
        }
    }
}
