using AI2D.Engine;

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

        public virtual void ApplyIntelligence()
        {
        }
    }
}