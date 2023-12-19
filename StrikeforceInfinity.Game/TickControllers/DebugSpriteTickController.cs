using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Managers;
using StrikeforceInfinity.Game.Sprites;
using StrikeforceInfinity.Game.TickControllers.BaseClasses;

namespace StrikeforceInfinity.Game.Controller
{
    internal class DebugSpriteTickController : SpriteTickControllerBase<SpriteDebug>
    {
        public DebugSpriteTickController(EngineCore core, EngineSpriteManager manager)
            : base(core, manager)
        {
        }

        public override void ExecuteWorldClockTick(SiPoint displacementVector)
        {
            /*
            if (Core.Player.Sprite != null)
            {
                var anchor = Core.Sprites.Debugs.ByTag("Anchor");
                if (anchor == null)
                {
                    Core.Sprites.Debugs.CreateAtCenterScreen("Anchor");
                    anchor = Core.Sprites.Debugs.ByTag("Anchor");
                }

                var pointer = Core.Sprites.Debugs.ByTag("Pointer");
                if (pointer == null)
                {
                    Core.Sprites.Debugs.CreateAtCenterScreen("Pointer");
                    pointer = Core.Sprites.Debugs.ByTag("Pointer");
                }

                double requiredAngle = Core.Player.Sprite.AngleTo(anchor);
                var offset = HgMath.AngleFromPointAtDistance(new HgAngle(requiredAngle), new HgPoint(200, 200));
                pointer.Velocity.Angle.Degrees = requiredAngle;
                pointer.Location = Core.Player.Sprite.Location + offset;
                anchor.Velocity.Angle.Degrees = anchor.AngleTo(Core.Player.Sprite);
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
