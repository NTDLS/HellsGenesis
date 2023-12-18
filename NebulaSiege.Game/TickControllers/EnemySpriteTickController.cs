using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Engine.Types.Geometry;
using NebulaSiege.Game.Managers;
using NebulaSiege.Game.Sprites.Enemies.BaseClasses;
using NebulaSiege.Game.TickControllers.BaseClasses;
using NebulaSiege.Game.Utility;
using System;

namespace NebulaSiege.Game.Controller
{
    internal class EnemySpriteTickController : SpriteTickControllerBase<SpriteEnemyBase>
    {
        public EnemySpriteTickController(EngineCore core, EngineSpriteManager manager)
            : base(core, manager)
        {
        }

        public override void ExecuteWorldClockTick(NsPoint displacementVector)
        {
            if (Core.Player.Sprite != null)
            {
                Core.Player.Sprite.SelectedSecondaryWeapon?.LockedOnObjects.Clear();
            }

            foreach (var enemy in Visible())
            {
                foreach (var weapon in enemy.Weapons)
                {
                    weapon.LockedOnObjects.Clear();
                }

                if (Core.Player.Sprite.Visable)
                {
                    enemy.ApplyIntelligence(displacementVector);

                    if (Core.Player.Sprite.SelectedSecondaryWeapon != null)
                    {
                        Core.Player.Sprite.SelectedSecondaryWeapon.ApplyWeaponsLock(displacementVector, enemy); //Player lock-on to enemy. :D
                    }
                }

                enemy.ApplyMotion(displacementVector);
                enemy.RenewableResources.RenewAllResources();
            }
        }

        public T Create<T>() where T : SpriteEnemyBase
        {
            lock (SpriteManager.Collection)
            {
                object[] param = { Core };
                SpriteEnemyBase obj = (SpriteEnemyBase)Activator.CreateInstance(typeof(T), param);

                obj.Location = Core.Display.RandomOffScreenLocation();
                obj.Velocity.MaxSpeed = HgRandom.Generator.Next(Core.Settings.MinEnemySpeed, Core.Settings.MaxEnemySpeed);
                obj.Velocity.Angle.Degrees = HgRandom.Generator.Next(0, 360);

                obj.BeforeCreate();
                SpriteManager.Collection.Add(obj);
                obj.AfterCreate();

                return (T)obj;
            }
        }
    }
}
