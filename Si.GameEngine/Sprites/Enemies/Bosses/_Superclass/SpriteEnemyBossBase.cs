using Si.GameEngine.Core;
using Si.GameEngine.Sprites.Enemies._Superclass;

namespace Si.GameEngine.Sprites.Enemies.Bosses._Superclass
{
    /// <summary>
    /// Boss enemies are specialized enemy types, typically have destructible/moving components.
    /// </summary>
    internal class SpriteEnemyBossBase : SpriteEnemyBase
    {
        public SpriteEnemyBossBase(GameEngineCore gameEngine, int hullHealth, int bountyMultiplier)
            : base(gameEngine, hullHealth, bountyMultiplier)
        {
            Velocity.ThrottlePercentage = 1;
            Initialize();
        }

        public override void Explode()
        {
            _explodeSound?.Play();
            _explosionAnimation?.Reset();
            _gameEngine.Sprites.Animations.AddAt(_explosionAnimation, this);

            CreateParticlesExplosion();

            base.Explode();
        }
    }
}
