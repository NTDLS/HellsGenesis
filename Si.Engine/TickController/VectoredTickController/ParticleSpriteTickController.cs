using SharpDX;
using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.Sprite._Superclass;
using Si.Engine.TickController._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using Si.Rendering;
using System.Drawing;
using static Si.Library.SiConstants;

namespace Si.Engine.TickController.VectoredTickControllerBase
{
    public class ParticleSpriteTickController : VectoredTickControllerBase<SpriteParticleBase>
    {
        public ParticleSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiPoint displacementVector)
        {
            foreach (var particle in Visible())
            {
                particle.ApplyMotion(epoch, displacementVector);
            }
        }

        public void AddAt(SiPoint location, Color4 color, int count, Size? size = null)
        {
            for (int i = 0; i < count; i++)
            {
                AddAt(location + SiRandom.Between(-20, 20), color, size);
            }
        }

        public void AddAt(SpriteBase sprite, Color4 color, int count, Size? size = null)
        {
            for (int i = 0; i < count; i++)
            {
                AddAt(sprite.Location + SiRandom.Between(-20, 20), color, size);
            }
        }

        public SpriteParticle AddAt(SpriteBase sprite, Color4 color, Size? size = null)
        {
            var obj = new SpriteParticle(Engine, sprite.Location, size ?? new Size(1, 1), color);
            SpriteManager.Add(obj);
            return obj;
        }

        public SpriteParticle AddAt(SiPoint location, Color4 color, Size? size = null)
        {
            var obj = new SpriteParticle(Engine, location, size ?? new Size(1, 1), color)
            {
                Visable = true
            };
            SpriteManager.Add(obj);
            return obj;
        }

        public SpriteParticle AddAt(SiPoint location, Size? size = null)
        {
            var obj = new SpriteParticle(Engine, location, size ?? new Size(1, 1))
            {
                Visable = true
            };
            SpriteManager.Add(obj);
            return obj;
        }

        public void ParticleBlastAt(int maxParticleCount, SpriteBase at)
        {
            Engine.Events.Add(() => ParticleBlastAt(maxParticleCount, at.Location));
        }

        /// <summary>
        /// Creates a random number of blasts consiting of "hot" colored particles at a given location.
        /// </summary>
        /// <param name="maxParticleCount"></param>
        /// <param name="at"></param>
        public void ParticleBlastAt(int maxParticleCount, SiPoint location)
        {
            for (int i = 0; i < SiRandom.Between(maxParticleCount / 2, maxParticleCount); i++)
            {
                var particle = AddAt(location, new Size(SiRandom.Between(1, 2), SiRandom.Between(1, 2)));
                particle.Shape = ParticleShape.FilledEllipse;
                particle.ColorType = ParticleColorType.SingleColor;
                //particle.GradientStartColor = SiRenderingUtility.GetRandomHotColor();
                //particle.GradientEndColor = SiRenderingUtility.GetRandomHotColor();
                particle.Color = SiRenderingUtility.GetRandomHotColor();
                particle.CleanupMode = ParticleCleanupMode.FadeToBlack;
                particle.FadeToBlackReductionAmount = SiRandom.Between(0.001f, 0.01f);
                particle.Velocity.MaximumSpeed *= SiRandom.Between(1, 3.5f);
                particle.VectorType = ParticleVectorType.UseTravelAngle;
            }
        }

        public void ParticleCloud(int particleCount, SpriteBase at)
            => ParticleCloud(particleCount, at.Location);

        public void ParticleCloud(int particleCount, SiPoint location)
        {
            for (int i = 0; i < particleCount; i++)
            {
                var particle = AddAt(location, SiRenderingUtility.GetRandomHotColor(), new Size(5, 5));

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
                particle.Velocity.MaximumSpeed *= SiRandom.Between(1, 3.5f);
                particle.VectorType = ParticleVectorType.UseNativeForwardAngle;
            }
        }
    }
}
