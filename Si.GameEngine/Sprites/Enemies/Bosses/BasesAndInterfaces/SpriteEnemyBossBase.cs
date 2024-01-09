using Si.GameEngine.Engine;
using Si.GameEngine.Sprites.Enemies.BasesAndInterfaces;

namespace Si.GameEngine.Sprites.Enemies.Bosses.BasesAndInterfaces
{
    /// <summary>
    /// Boss enemies are specialized enemy types, typically have destructible/moving components.
    /// </summary>
    internal class SpriteEnemyBossBase : SpriteEnemyBase
    {
        public SpriteEnemyBossBase(EngineCore gameCore, int hullHealth, int bountyMultiplier)
            : base(gameCore, hullHealth, bountyMultiplier)
        {
            Velocity.ThrottlePercentage = 1;
            Initialize();
        }

        public override void Explode()
        {
            _explodeSound?.Play();
            _explosionAnimation?.Reset();
            _gameCore.Sprites.Animations.AddAt(_explosionAnimation, this);

            CreateParticlesExplosion();

            base.Explode();
        }
    }
}
