using HG.Engine;
using HG.Types;
using System.Drawing;

namespace HG.Actors
{
    internal class ActorDebug : ActorBase
    {
        public ActorDebug(Core core)
            : base(core)
        {
            InitializeGenericExplodable(@"Graphics\Debug.png", new Size(64, 64));
            X = 0;
            Y = 0;
            Velocity = new HgVelocity<double>();
        }

        public ActorDebug(Core core, double x, double y)
            : base(core)
        {
            InitializeGenericExplodable(@"Graphics\Debug.png", new Size(64, 64));
            X = x;
            Y = y;
            Velocity = new HgVelocity<double>();
        }

        public ActorDebug(Core core, double x, double y, string imagePath)
            : base(core)
        {
            InitializeGenericExplodable(imagePath);
            X = x;
            Y = y;
            Velocity = new HgVelocity<double>();
        }
    }
}
