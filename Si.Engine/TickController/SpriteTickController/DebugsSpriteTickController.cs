using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.TickController._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.TickController.SpriteTickController
{
    public class DebugsSpriteTickController : SpriteTickControllerBase<SpriteDebug>
    {
        public DebugsSpriteTickController(EngineCore engine, EngineSpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiPoint displacementVector)
        {
            /*
            if (Engine.Player.Sprite != null)
            {
                var anchor = Engine.Sprites.Debugs.ByTag("Anchor");
                if (anchor == null)
                {
                    Engine.Sprites.Debugs.CreateAtCenterScreen("Anchor");
                    anchor = Engine.Sprites.Debugs.ByTag("Anchor");
                }

                var pointer = Engine.Sprites.Debugs.ByTag("Pointer");
                if (pointer == null)
                {
                    Engine.Sprites.Debugs.CreateAtCenterScreen("Pointer");
                    pointer = Engine.Sprites.Debugs.ByTag("Pointer");
                }

                float requiredAngle = Engine.Player.Sprite.AngleTo(anchor);
                var offset = SiMath.AngleFromPointAtDistance(new SiAngle(requiredAngle), new SiPoint(200, 200));
                pointer.Velocity.Angle.Degrees = requiredAngle;
                pointer.Location = Engine.Player.Sprite.Location + offset;
                anchor.Velocity.Angle.Degrees = anchor.AngleTo(Engine.Player.Sprite);
            }
            */

            foreach (var debug in Visible())
            {
                debug.ApplyMotion(epoch, displacementVector);
                debug.RenewableResources.RenewAllResources(epoch);
            }
        }
    }
}
