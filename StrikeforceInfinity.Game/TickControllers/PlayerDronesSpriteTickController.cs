using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Managers;
using StrikeforceInfinity.Game.Sprites.Player.BaseClasses;
using StrikeforceInfinity.Game.TickControllers.BaseClasses;
using StrikeforceInfinity.Game.Utility;
using System;

namespace StrikeforceInfinity.Game.Controller
{
    /// <summary>
    /// This controller allows for the manipulation of multiplay drones.
    /// A multiplay drone is the local clone of a remote human player ship.
    /// </summary>
    internal class PlayerDronesSpriteTickController : SpriteTickControllerBase<SpritePlayerDroneBase>
    {
        private readonly EngineCore _gameCore;

        public PlayerDronesSpriteTickController(EngineCore gameCore, EngineSpriteManager manager)
            : base(gameCore, manager)
        {
            _gameCore = gameCore;
        }

        public override void ExecuteWorldClockTick(SiPoint displacementVector)
        {
            foreach (var drone in Visible())
            {
                drone.ApplyIntelligence(displacementVector);

                //drone.ApplyMotion(displacementVector);
            }
        }

        public T Create<T>() where T : SpritePlayerDroneBase
        {
            lock (SpriteManager.Collection)
            {
                object[] param = { GameCore };
                var obj = (SpritePlayerDroneBase)Activator.CreateInstance(typeof(T), param);
                obj.Location = GameCore.Display.RandomOffScreenLocation();
                obj.Velocity.MaxSpeed = HgRandom.Generator.Next(GameCore.Settings.MinEnemySpeed, GameCore.Settings.MaxEnemySpeed);
                obj.Velocity.Angle.Degrees = HgRandom.Generator.Next(0, 360);
                SpriteManager.Collection.Add(obj);

                return (T)obj;
            }
        }
    }
}
