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
    /// This controller allows for the manipulation of all players - which includes the
    /// local player as well as any multiplay drones.
    /// </summary>
    internal class PlayersSpriteTickController : SpriteTickControllerBase<SpritePlayerBase>
    {
        private readonly EngineCore _gameCore;

        public PlayersSpriteTickController(EngineCore gameCore, EngineSpriteManager manager)
            : base(gameCore, manager)
        {
            _gameCore = gameCore;
        }

        public override void ExecuteWorldClockTick(SiPoint displacementVector)
        {
            foreach (var player in Visible())
            {
                player.ApplyIntelligence(displacementVector);

                //player.ApplyMotion(displacementVector);
            }
        }

        public T Create<T>() where T : SpritePlayerBase
        {
            lock (SpriteManager.Collection)
            {
                object[] param = { GameCore };
                var obj = (SpritePlayerBase)Activator.CreateInstance(typeof(T), param);
                obj.Location = GameCore.Display.RandomOffScreenLocation();
                obj.Velocity.MaxSpeed = HgRandom.Generator.Next(GameCore.Settings.MinEnemySpeed, GameCore.Settings.MaxEnemySpeed);
                obj.Velocity.Angle.Degrees = HgRandom.Generator.Next(0, 360);
                SpriteManager.Collection.Add(obj);

                return (T)obj;
            }
        }
    }
}
