using HG.Actors.BaseClasses;
using HG.Actors.Weapons.Bullets.BaseClasses;
using HG.Engine;
using HG.Types;
using System.Drawing;

namespace HG.Actors.Ordinary
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
            Velocity = new HgVelocity();
        }

        public override bool TestHit(HgPoint<double> displacementVector, BulletBase bullet, HgPoint<double> hitTestPosition)
        {
            if (bullet.FiredFromType == HgFiredFromType.Player)
            {
                if (Intersects(hitTestPosition))
                {
                    if (Hit(bullet))
                    {
                        if (HullHealth <= 0)
                        {
                            Explode();
                        }
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
