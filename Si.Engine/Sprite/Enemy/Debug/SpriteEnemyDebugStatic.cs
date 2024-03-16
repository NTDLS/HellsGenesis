using Si.Engine.Sprite.Enemy.Peon._Superclass;

namespace Si.Engine.Sprite.Enemy.Debug
{
    /// <summary>
    /// Debugging enemy unit - a scary sight to see.
    /// </summary>
    internal class SpriteEnemyDebugStatic : SpriteEnemyPeonBase
    {
        public const int hullHealth = 10;
        public const int bountyMultiplier = 15;

        public SpriteEnemyDebugStatic(EngineCore engine)
            : base(engine)
        {
            InitializeSpriteFromMetadata(@"Sprites\Enemy\Debug\Hull.png");
        }
    }
}
