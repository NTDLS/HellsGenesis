﻿using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Managers;
using StrikeforceInfinity.Game.Sprites;
using StrikeforceInfinity.Game.TickControllers.BaseClasses;
using StrikeforceInfinity.Game.Utility;

namespace StrikeforceInfinity.Game.Controller
{
    internal class ParticleSpriteTickController : SpriteTickControllerBase<SpriteParticleBase>
    {
        public ParticleSpriteTickController(EngineCore gameCore, EngineSpriteManager manager)
            : base(gameCore, manager)
        {
        }

        public override void ExecuteWorldClockTick(SiPoint displacementVector)
        {
            foreach (var particle in Visible())
            {
                particle.ApplyMotion(displacementVector);
            }
        }

        public void CreateRandomShipPartParticlesAt(double x, double y, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var obj = GameCore.Sprites.Particles.CreateRandomShipPartParticleAt(
                    x + HgRandom.Between(-20, 20), y + HgRandom.Between(-20, 20));
                obj.Visable = true;
            }
        }

        public void CreateRandomShipPartParticlesAt(SpriteBase sprite, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var obj = GameCore.Sprites.Particles.CreateRandomShipPartParticleAt(
                    sprite.X + HgRandom.Between(-20, 20), sprite.Y + HgRandom.Between(-20, 20));
                obj.Visable = true;
            }
        }

        public SpriteRandomShipPartParticle CreateRandomShipPartParticleAt(SpriteBase sprite)
        {
            lock (SpriteManager.Collection)
            {
                var obj = new SpriteRandomShipPartParticle(GameCore, sprite.X, sprite.Y);
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }

        public SpriteRandomShipPartParticle CreateRandomShipPartParticleAt(double x, double y)
        {
            lock (SpriteManager.Collection)
            {
                var obj = new SpriteRandomShipPartParticle(GameCore, x, y);
                SpriteManager.Collection.Add(obj);
                return obj;
            }
        }
    }
}
