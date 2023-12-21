using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Managers;
using StrikeforceInfinity.Game.Sprites.Player.BasesAndInterfaces;
using StrikeforceInfinity.Game.TickControllers.BasesAndInterfaces;
using StrikeforceInfinity.Sprites.BasesAndInterfaces;
using System.Linq;

namespace StrikeforceInfinity.Game.Controller
{
    /// <summary>
    /// This controller allows for the manipulation of multiplay drones.
    /// A multiplay drone is the local clone of a remote human player ship.
    /// </summary>
    internal class PlayerDronesSpriteTickController : SpriteTickControllerBase<SpritePlayerBase>
    {
        private readonly EngineCore _gameCore;

        public PlayerDronesSpriteTickController(EngineCore gameCore, EngineSpriteManager manager)
            : base(gameCore, manager)
        {
            _gameCore = gameCore;
        }

        public override void ExecuteWorldClockTick(SiPoint displacementVector)
        {
            foreach (var drone in Visible().OfType<ISpriteDrone>())
            {
                //drone.ApplyIntelligence(displacementVector);
                //drone.ApplyMotion(displacementVector);
            }
        }
    }
}
