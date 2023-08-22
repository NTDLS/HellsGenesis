using AI2D.Engine;
using AI2D.Types;
using System.Drawing;

namespace AI2D.Actors
{
    internal class ActorDebug : ActorBase
    {
        public ActorDebug(Core core)
            : base(core)
        {
            Initialize(@"..\..\..\Assets\Graphics\Debug.png", new Size(64, 64));
            X = 0;
            Y = 0;
            Velocity = new Velocity<double>();
        }
        public ActorDebug(Core core, double x, double y)
            : base(core)
        {
            Initialize(@"..\..\..\Assets\Graphics\Debug.png", new Size(64, 64));
            X = x;
            Y = y;
            Velocity = new Velocity<double>();
        }
    }
}
