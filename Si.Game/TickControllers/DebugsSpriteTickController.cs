using Si.Game.Engine;
using Si.Game.Engine.Types.Geometry;
using Si.Game.Managers;
using Si.Game.Sprites;
using Si.Game.TickControllers.BasesAndInterfaces;

namespace Si.Game.Controller
{
    internal class DebugsSpriteTickController : SpriteTickControllerBase<SpriteDebug>
    {
        public DebugsSpriteTickController(EngineCore gameCore, EngineSpriteManager manager)
            : base(gameCore, manager)
        {
        }

        public override void ExecuteWorldClockTick(SiPoint displacementVector)
        {
            /*
            if (GameCore.Player.Sprite != null)
            {
                var anchor = GameCore.Sprites.Debugs.ByTag("Anchor");
                if (anchor == null)
                {
                    GameCore.Sprites.Debugs.CreateAtCenterScreen("Anchor");
                    anchor = GameCore.Sprites.Debugs.ByTag("Anchor");
                }

                var pointer = GameCore.Sprites.Debugs.ByTag("Pointer");
                if (pointer == null)
                {
                    GameCore.Sprites.Debugs.CreateAtCenterScreen("Pointer");
                    pointer = GameCore.Sprites.Debugs.ByTag("Pointer");
                }

                double requiredAngle = GameCore.Player.Sprite.AngleTo(anchor);
                var offset = SiMath.AngleFromPointAtDistance(new SiAngle(requiredAngle), new SiPoint(200, 200));
                pointer.Velocity.Angle.Degrees = requiredAngle;
                pointer.Location = GameCore.Player.Sprite.Location + offset;
                anchor.Velocity.Angle.Degrees = anchor.AngleTo(GameCore.Player.Sprite);
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
