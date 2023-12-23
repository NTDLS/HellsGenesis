using Si.Game.Engine;
using Si.Game.Sprites.Enemies.BasesAndInterfaces;

namespace Si.Game.Sprites.Enemies.Bosses.BasesAndInterfaces
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
            _gameCore.Sprites.Animations.InsertAt(_explosionAnimation, this);

            CreateParticlesExplosion();

            base.Explode();
        }
    }
}
