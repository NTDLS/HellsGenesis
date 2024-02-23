using Si.GameEngine.Core.Managers;
using Si.GameEngine.Core.TickControllers._Superclass;
using Si.GameEngine.Sprites;
using Si.Library.Types.Geometry;

namespace Si.GameEngine.Core.TickControllers
{
    public class DebugsSpriteTickController : SpriteTickControllerBase<SpriteDebug>
    {
        public DebugsSpriteTickController(GameEngineCore gameEngine, EngineSpriteManager manager)
            : base(gameEngine, manager)
        {
        }

        public override void ExecuteWorldClockTick(double epochMilliseconds, SiPoint displacementVector)
        {
            /*
            if (GameEngine.Player.Sprite != null)
            {
                var anchor = GameEngine.Sprites.Debugs.ByTag("Anchor");
                if (anchor == null)
                {
                    GameEngine.Sprites.Debugs.CreateAtCenterScreen("Anchor");
                    anchor = GameEngine.Sprites.Debugs.ByTag("Anchor");
                }

                var pointer = GameEngine.Sprites.Debugs.ByTag("Pointer");
                if (pointer == null)
                {
                    GameEngine.Sprites.Debugs.CreateAtCenterScreen("Pointer");
                    pointer = GameEngine.Sprites.Debugs.ByTag("Pointer");
                }

                double requiredAngle = GameEngine.Player.Sprite.AngleTo(anchor);
                var offset = SiMath.AngleFromPointAtDistance(new SiAngle(requiredAngle), new SiPoint(200, 200));
                pointer.Velocity.Angle.Degrees = requiredAngle;
                pointer.Location = GameEngine.Player.Sprite.Location + offset;
                anchor.Velocity.Angle.Degrees = anchor.AngleTo(GameEngine.Player.Sprite);
            }
            */

            foreach (var debug in Visible())
            {
                debug.ApplyMotion(displacementVector);
                debug.RenewableResources.RenewAllResources();
            }
        }
    }
}
