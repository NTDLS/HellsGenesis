using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.PowerUp;
using Si.Engine.Sprite.PowerUp._Superclass;
using Si.GameEngine.AI._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;
using System;
using System.Collections.Generic;

namespace Si.Engine.Sprite.Enemy._Superclass
{
    /// <summary>
    /// The enemy base is a sub-class of the ship base. It is used by Peon and Boss enemies.
    /// </summary>
    public class SpriteEnemyBase : SpriteShipBase
    {
        public SpriteEnemyBase(EngineCore engine)
                : base(engine)
        {
            Velocity.ForwardVelocity = 1;

            RadarPositionIndicator = _engine.Sprites.RadarPositions.Add();
            RadarPositionIndicator.Visable = false;

            RadarPositionText = _engine.Sprites.TextBlocks.CreateRadarPosition(
                engine.Rendering.TextFormats.RadarPositionIndicator,
                engine.Rendering.Materials.Brushes.Red, new SiPoint());
        }

        public virtual void BeforeCreate() { }

        public virtual void AfterCreate() { }

        public override void RotationChanged() => LocationChanged();

        #region Artificial Intelligence.

        public IAIController CurrentAIController { get; set; }
        private readonly Dictionary<Type, IAIController> _aiControllers = new();

        public void AddAIController(IAIController controller)
            => _aiControllers.Add(controller.GetType(), controller);

        public IAIController GetAIController<T>() where T : IAIController => _aiControllers[typeof(T)];

        public void SetCurrentAIController<T>() where T : IAIController
        {
            CurrentAIController = GetAIController<T>();
        }

        #endregion

        public override void Explode()
        {
            _engine.Player.Sprite.Bounty += Bounty;

            if (SiRandom.PercentChance(10))
            {
                var powerup = SiRandom.Between(0, 4) switch
                {
                    0 => new SpritePowerupAmmo(_engine),
                    1 => new SpritePowerupBoost(_engine),
                    2 => new SpritePowerupBounty(_engine),
                    3 => new SpritePowerupRepair(_engine),
                    4 => new SpritePowerupShield(_engine),
                    _ => null as SpritePowerupBase
                };

                if (powerup != null)
                {
                    powerup.Location = Location;
                    _engine.Sprites.Powerups.Add(powerup);
                }
            }
            base.Explode();
        }

        /// <summary>
        /// Moves the sprite based on its velocity/boost (velocity) taking into account the background scroll.
        /// </summary>
        /// <param name="displacementVector"></param>
        public override void ApplyMotion(float epoch, SiPoint displacementVector)
        {
            //When an enemy has boost available, it will use it.
            if (Velocity.AvailableBoost > 0)
            {
                if (Velocity.ForwardBoostVelocity < 1.0) //Ramp up the boost until it is at 100%
                {
                    Velocity.ForwardBoostVelocity += _engine.Settings.EnemyVelocityRampUp;
                }
                Velocity.AvailableBoost -= Velocity.MaximumSpeedBoost * Velocity.ForwardBoostVelocity; //Consume boost.

                if (Velocity.AvailableBoost < 0) //Sanity check available boost.
                {
                    Velocity.AvailableBoost = 0;
                }
            }
            else if (Velocity.ForwardBoostVelocity > 0) //Ramp down the boost.
            {
                Velocity.ForwardBoostVelocity -= _engine.Settings.EnemyVelocityRampDown;
                if (Velocity.ForwardBoostVelocity < 0)
                {
                    Velocity.ForwardBoostVelocity = 0;
                }
            }

            base.ApplyMotion(epoch, displacementVector);

            FixRadarPositionIndicator();
        }

        public virtual void ApplyIntelligence(float epoch, SiPoint displacementVector)
        {
            CurrentAIController?.ApplyIntelligence(epoch, displacementVector);

            if (Weapons != null)
            {
                foreach (var weapon in Weapons)
                {
                    weapon.ApplyIntelligence(epoch);
                }
            }
        }
    }
}
