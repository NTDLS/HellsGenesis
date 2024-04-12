using Si.Engine.Sprite.Enemy._Superclass;

namespace Si.Engine.Sprite.Enemy.Starbase._Superclass
{
    /// <summary>
    /// Base class for "starbase" enemies.
    /// </summary>
    internal class SpriteEnemyStarbase : SpriteEnemyBase
    {
        public SpriteEnemyStarbase(EngineCore engine, string imagePath)
            : base(engine)
        {
            SetImageAndLoadMetadata(imagePath);
            RecalculateMovementVector();
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }
    }
}
