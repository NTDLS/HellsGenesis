using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Managers;
using HG.Sprites.Enemies;
using HG.TickControllers;
using HG.Utility;
using System;

namespace HG.Controller
{
    internal class EnemySpriteTickController : VectoredTickControllerBase<SpriteEnemyBase>
    {
        public EnemySpriteTickController(EngineCore core, EngineSpriteManager manager)
            : base(core, manager)
        {
        }

        public override void ExecuteWorldClockTick(HgPoint displacementVector)
        {
            if (Core.Player.Sprite != null)
            {
                Core.Player.Sprite.SelectedSecondaryWeapon?.LockedOnObjects.Clear();
            }

            foreach (var enemy in Visible())
            {
                enemy.SelectedSecondaryWeapon?.LockedOnObjects.Clear();

                if (Core.Player.Sprite.Visable)
                {
                    enemy.ApplyIntelligence(displacementVector);

                    if (Core.Player.Sprite.SelectedSecondaryWeapon != null)
                    {
                        Core.Player.Sprite.SelectedSecondaryWeapon.ApplyIntelligence(displacementVector, enemy); //Player lock-on to enemy. :D
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
