using Si.GameEngine.Core;
using Si.GameEngine.Core.Managers;
using Si.GameEngine.Core.TickControllers._Superclass;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Player._Superclass;
using Si.Shared.Types.Geometry;
using System.Linq;

namespace Si.GameEngine.Core.TickControllers
{
    /// <summary>
    /// This controller allows for the manipulation of multiplay drones.
    /// A multiplay drone is the local clone of a remote human player ship.
    /// </summary>
    public class PlayerDronesSpriteTickController : SpriteTickControllerBase<SpritePlayerBase>
    {
        private readonly Engine _gameCore;

        public PlayerDronesSpriteTickController(Engine gameCore, EngineSpriteManager manager)
            : base(gameCore, manager)
        {
            _gameCore = gameCore;
        }

        public override void ExecuteWorldClockTick(SiPoint displacementVector)
        {
            foreach (var drone in Visible().OfType<ISpriteDrone>())
            {
                drone.ApplyMotion(displacementVector);
            }
        }
    }
}
