using HG.Actors.BaseClasses;
using HG.Engine;
using HG.Types;
using HG.Types.Geometry;
using System.Drawing;

namespace HG.Actors.Ordinary
{
    internal class ActorDebug : ActorShipBase
    {
        public ActorDebug(Core core)
            : base(core)
        {
            Initialize(@"Graphics\Debug.png", new Size(64, 64));
            X = 0;
            Y = 0;
            Velocity = new HgVelocity();
        }

        public ActorDebug(Core core, double x, double y)
            : base(core)
        {
            Initialize(@"Graphics\Debug.png", new Size(64, 64));
            X = x;
            Y = y;
            Velocity = new HgVelocity();
        }

        public ActorDebug(Core core, double x, double y, string imagePath)
            : base(core)
        {
            Initialize(imagePath);
            X = x;
            Y = y;
            Velocity = new HgVelocity();
        }

        public override void ApplyMotion(HgPoint displacementVector)
        {
            this.Velocity.Angle.Degrees = AngleTo(_core.Player.Actor);
            base.ApplyMotion(displacementVector);
        }
    }
}
