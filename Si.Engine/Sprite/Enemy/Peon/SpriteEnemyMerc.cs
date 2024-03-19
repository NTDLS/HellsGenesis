using Si.Engine.Sprite.Enemy.Peon._Superclass;

namespace Si.Engine.Sprite.Enemy.Peon
{
    internal class SpriteEnemyMerc : SpriteEnemyPeonBase
    {
        public SpriteEnemyMerc(EngineCore engine)
            : base(engine)
        {
            SetImageAndLoadMetadata(@"Sprites\Enemy\Peon\Merc\Hull.png");
        }
    }
}
