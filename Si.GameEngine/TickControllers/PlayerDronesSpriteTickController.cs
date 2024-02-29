using Si.GameEngine.ResourceManagers;
using Si.GameEngine.Sprites.Player._Superclass;
using Si.GameEngine.TickControllers._Superclass;
using Si.Library.Mathematics.Geometry;
using Si.Library.Sprite;
using System.Linq;

namespace Si.GameEngine.TickControllers
{
    /// <summary>
    /// This controller allows for the manipulation of multiplay drones.
    /// A multiplay drone is the local clone of a remote human player ship.
    /// </summary>
    public class PlayerDronesSpriteTickController : SpriteTickControllerBase<SpritePlayerBase>
    {
        private readonly GameEngineCore _gameEngine;

        public PlayerDronesSpriteTickController(GameEngineCore gameEngine, EngineSpriteManager manager)
            : base(gameEngine, manager)
        {
            _gameEngine = gameEngine;
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector displacementVector)
        {
            foreach (var drone in Visible().OfType<ISpriteDrone>())
            {
                drone.ApplyMotion(epoch, displacementVector);
            }
        }
    }
}
