using AI2D.Engine;
using AI2D.Types;

namespace AI2D.GraphicObjects.Enemies
{
    public class BaseEnemy : BaseGraphicObject
    {
        public int CollisionDamage { get; set; } = 25;

        public BaseEnemy(Core core)
            : base(core)
        {
            Velocity.ThrottlePercentage = 1;
            Initialize();
        }

        public virtual void ApplyMotion(PointD frameAppliedOffset)
        {
            X += (Velocity.Angle.X * (Velocity.MaxSpeed * Velocity.ThrottlePercentage)) - frameAppliedOffset.X;
            Y += (Velocity.Angle.Y * (Velocity.MaxSpeed * Velocity.ThrottlePercentage)) - frameAppliedOffset.Y;
        }

        public virtual void ApplyIntelligence(PointD frameAppliedOffset)
        {
            if (CurrentWeapon != null && _core.Actors.Player != null)
            {
                CurrentWeapon.ApplyIntelligence(frameAppliedOffset, _core.Actors.Player); //Enemy lock-on to Player. :O
            }
        }
    }
}