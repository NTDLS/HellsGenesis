using Si.Engine.Sprite.Enemy.Peon._Superclass;

namespace Si.Engine.Sprite.Enemy.Peon
{
    internal class SpriteEnemyScav : SpriteEnemyPeonBase
    {
        public SpriteEnemyScav(EngineCore engine)
            : base(engine)
        {
            InitializeSpriteFromMetadata(@"Sprites\Enemy\Peon\Scav\Hull.png");
        }
    }
}
