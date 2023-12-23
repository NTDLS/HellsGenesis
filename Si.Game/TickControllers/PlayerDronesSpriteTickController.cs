using Si.Game.Engine;
using Si.Game.Engine.Types.Geometry;
using Si.Game.Managers;
using Si.Game.Sprites.Player.BasesAndInterfaces;
using Si.Game.TickControllers.BasesAndInterfaces;
using Si.Sprites.BasesAndInterfaces;
using System.Linq;

namespace Si.Game.Controller
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
