using SharpDX;
using Si.GameEngine.Core.Managers;
using Si.GameEngine.Core.TickControllers._Superclass;
using Si.GameEngine.Sprites;
using Si.GameEngine.Sprites._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using Si.Rendering;
using System.Drawing;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Core.TickControllers
{
    public class ParticlesSpriteTickController : SpriteTickControllerBase<SpriteParticleBase>
    {
        public ParticlesSpriteTickController(GameEngineCore gameEngine, EngineSpriteManager manager)
            : base(gameEngine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector displacementVector)
        {
            foreach (var particle in Visible())
            {
                particle.ApplyMotion(epoch, displacementVector);
            }
        }

        public void CreateAt(SiVector location, Color4 color, int count, Size? size = null)
        {
            for (int i = 0; i < count; i++)
            {
                CreateAt(location + SiRandom.Between(-20, 20), color, size);
            }
        }

        public void CreateAt(SpriteBase sprite, Color4 color, int count, Size? size = null)
        {
            for (int i = 0; i < count; i++)
            {
                CreateAt(sprite.Location + SiRandom.Between(-20, 20), color, size);
            }
        }

        public SpriteParticle CreateAt(SpriteBase sprite, Color4 color, Size? size = null)
        {
            var obj = new SpriteParticle(GameEngine, sprite.Location, size ?? new Size(1, 1), color);
            SpriteManager.Add(obj);
            return obj;
        }

        public SpriteParticle CreateAt(SiVector location, Color4 color, Size? size = null)
        {
            var obj = new SpriteParticle(GameEngine, location, size ?? new Size(1, 1), color)
            {
                Visable = true
            };
            SpriteManager.Add(obj);
            return obj;
        }

        /// <summary>
        /// Creates a random number of blasts consiting of "hot" colored particles at a given location.
        /// </summary>
        /// <param name="particleCount"></param>
        /// <param name="at"></param>
        public void ParticleBlast(int particleCount, SpriteBase at)
        {
            int explosionCount = SiRandom.Between(1, 4);
            int particlesExplosion = particleCount / explosionCount;

            int triggerDelay = 0;

            for (int instance = 0; instance < explosionCount; instance++)
            {
                //Make sure the next delay is higher than the previous.
                GameEngine.Events.Create(triggerDelay, () =>
                {
                    for (int i = 0; i < particlesExplosion; i++)
                    {
                        var particle = CreateAt(at.Location, RenderingUtility.GetRandomHotColor(), new Size(SiRandom.Between(1, 2), SiRandom.Between(1, 2)));
                        particle.Shape = ParticleShape.FilledEllipse;
                        particle.CleanupMode = ParticleCleanupMode.FadeToBlack;
                        particle.FadeToBlackReductionAmount = SiRandom.Between(0.001f, 0.01f);
                        particle.Velocity.Speed *= SiRandom.Between(1, 3.5f);
                        particle.VectorType = ParticleVectorType.Independent;
                    }
                    GameEngine.Audio.PlayRandomExplosion();
                });

                triggerDelay = SiRandom.Between(triggerDelay + 10, 500 + triggerDelay / 4);
            }
        }

        public void ParticleCloud(int particleCount, SpriteBase at)
        {
            for (int i = 0; i < particleCount; i++)
            {
                var particle = CreateAt(at.Location, RenderingUtility.GetRandomHotColor(), new Size(5, 5));

                switch (SiRandom.Between(1, 3))
                {
                    case 1:
                        particle.Shape = ParticleShape.Triangle;
                        break;
                    case 2:
                        particle.Shape = ParticleShape.FilledEllipse;
                        break;
                    case 3:
                        particle.Shape = ParticleShape.HollowEllipse;
                        break;
                }

                particle.CleanupMode = ParticleCleanupMode.FadeToBlack;
                particle.FadeToBlackReductionAmount = 0.001f;
                particle.Velocity.Speed *= SiRandom.Between(1, 3.5f);
                particle.VectorType = ParticleVectorType.Native;
            }
        }
    }
}
