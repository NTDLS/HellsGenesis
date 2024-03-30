using Si.Engine.Sprite.Enemy._Superclass;

namespace Si.Engine.Sprite.Enemy.Boss._Superclass
{
    /// <summary>
    /// Boss enemies are specialized enemy types, typically have destructible/moving components.
    /// </summary>
    internal class SpriteEnemyBossBase : SpriteEnemyBase
    {
        public SpriteEnemyBossBase(EngineCore engine)
            : base(engine)
        {
            MovementVector = MakeMovementVector();
        }
    }
}
