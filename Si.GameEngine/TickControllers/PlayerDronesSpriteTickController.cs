using Si.GameEngine.Engine;
using Si.GameEngine.Managers;
using Si.GameEngine.Sprites.Player.BasesAndInterfaces;
using Si.GameEngine.TickControllers.BasesAndInterfaces;
using Si.Shared.Types.Geometry;
using Si.Sprites.BasesAndInterfaces;
using System.Linq;

namespace Si.GameEngine.Controller
{
    /// <summary>
    /// This controller allows for the manipulation of multiplay drones.
    /// A multiplay drone is the local clone of a remote human player ship.
    /// </summary>
    public class PlayerDronesSpriteTickController : SpriteTickControllerBase<SpritePlayerBase>
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
                //drone.ApplyMotion(displacementVector);
            }
        }
    }
}
