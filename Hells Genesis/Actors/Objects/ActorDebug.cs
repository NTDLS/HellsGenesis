using HG.Engine;
using HG.Types;
using System.Drawing;

namespace HG.Actors.Objects
{
    internal class ActorDebug : ActorBase
    {
        public ActorDebug(Core core)
            : base(core)
        {
            Initialize(@"..\..\..\Assets\Graphics\Debug.png", new Size(64, 64));
            X = 0;
            Y = 0;
            Velocity = new HGVelocity<double>();
        }

        public ActorDebug(Core core, double x, double y)
            : base(core)
        {
            Initialize(@"..\..\..\Assets\Graphics\Debug.png", new Size(64, 64));
            X = x;
            Y = y;
            Velocity = new HGVelocity<double>();
        }

        public ActorDebug(Core core, double x, double y, string imagePath)
            : base(core)
        {
            Initialize(imagePath);
            X = x;
            Y = y;
            Velocity = new HGVelocity<double>();
        }
    }
}
