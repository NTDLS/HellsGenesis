using HG.Engine;
using HG.Types;
using System.Drawing;

namespace HG.Actors
{
    internal class ActorAttachment : ActorShipBase
    {
        public bool TakesDamage { get; set; }

        public ActorAttachment(Core core, string imagePath, Size? size = null)
            : base(core)
        {
            Initialize(imagePath, size);

            X = 0;
            Y = 0;
            Velocity = new HgVelocity<double>();
        }
    }
}
