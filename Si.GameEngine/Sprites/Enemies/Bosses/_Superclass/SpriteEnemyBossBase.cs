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
            Velocity.ForwardMomentium = 1;
            Initialize();
        }

        public override void Explode()
        {
            _gameEngine.Audio.PlayRandomExplosion();
            _gameEngine.Sprites.Animations.AddRandomExplosionAt(this);

            CreateParticlesExplosion();

            base.Explode();
        }
    }
}
