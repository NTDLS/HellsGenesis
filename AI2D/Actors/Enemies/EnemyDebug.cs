using AI2D.Actors.Enemies.AI;
using AI2D.Engine;
using AI2D.Types;
using AI2D.Weapons;
using Determinet.Types;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using static AI2D.Actors.Enemies.AI.BrainBase;

namespace AI2D.Actors.Enemies
{
    /// <summary>
    /// Debugging enemy uint.
    /// </summary>
    public class EnemyDebug : EnemyBase
    {
        public const int ScoreMultiplier = 15;
        private const string _assetPath = @"..\..\..\Assets\Graphics\Enemy\Debug\";
        private readonly int imageCount = 6;
        private readonly int selectedImageIndex = 0;

        public EnemyDebug(Core core)
            : base(core, EnemyBase.GetGenericHP(), ScoreMultiplier)
        {
            selectedImageIndex = Utility.Random.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"), new Size(32, 32));

            base.SetHitPoints(Utility.Random.Next(Constants.Limits.MinEnemyHealth, Constants.Limits.MaxEnemyHealth));

            Velocity.MaxSpeed = Utility.Random.Next(Constants.Limits.MaxSpeed - 2, Constants.Limits.MaxSpeed); //Upper end of the speed spectrum

            AddSecondaryWeapon(new WeaponVulcanCannon(_core)
            {
                RoundQuantity = 1000,
                FireDelayMilliseconds = 250
            });

            AddSecondaryWeapon(new WeaponDualVulcanCannon(_core)
            {
                RoundQuantity = 500,
                FireDelayMilliseconds = 500
            });

            SelectSecondaryWeapon(typeof(WeaponVulcanCannon));

            Brains.Add(AIBrainTypes.Logistics, GetLogisticsNoColisionBrain());
        }

        #region Artificial Intelligence.

        private double _maxObserveDistance { get; set; } = 500;
        private double _visionToleranceDegrees { get; set; } = 25;
        private DateTime? _lastDecisionTime = null;
        private int _millisecondsBetweenDecisions { get; set; } = 50;
        private float _decisionSensitivity { get; set; } = (float)Utility.RandomNumber(0.25, 0.55);

        public override void ApplyIntelligence(Point<double> frameAppliedOffset)
        {
            base.ApplyIntelligence(frameAppliedOffset);

            var now = DateTime.UtcNow;

            if (_lastDecisionTime == null || (now - (DateTime)_lastDecisionTime).TotalMilliseconds >= _millisecondsBetweenDecisions)
            {
                var decidingFactors = GetVisionInputs();

                var decisions = Brains[AIBrainTypes.Logistics].FeedForward(decidingFactors);

                if (decisions.Get(AIOutputs.OutChangeDirection) >= _decisionSensitivity)
                {
                    var rotateAmount = decisions.Get(AIOutputs.OutRotationAmount);

                    if (decisions.Get(AIOutputs.OutRotateDirection) >= _decisionSensitivity)
                    {
                        Rotate(45 * rotateAmount);
                    }
                    else
                    {
                        Rotate(-45 * rotateAmount);
                    }
                }

                if (decisions.Get(AIOutputs.OutChangeSpeed) >= _decisionSensitivity)
                {
                    var speedFactor = decisions.Get(AIOutputs.OutChangeSpeedAmount, 0);
                    Velocity.ThrottlePercentage += (speedFactor / 5.0);
                }
                else
                {
                    var speedFactor = decisions.Get(AIOutputs.OutChangeSpeedAmount, 0);
                    Velocity.ThrottlePercentage += -(speedFactor / 5.0);
                }

                if (Velocity.ThrottlePercentage < 0)
                {
                    Velocity.ThrottlePercentage = 0;
                }
                if (Velocity.ThrottlePercentage == 0)
                {
                    Velocity.ThrottlePercentage = 0.10;
                }

                _lastDecisionTime = now;
            }
        }

        /// <summary>
        /// Looks around and gets inputs for visible proximity objects.
        /// </summary>
        /// <returns></returns>
        private DniNamedInterfaceParameters GetVisionInputs()
        {
            var aiParams = new DniNamedInterfaceParameters();

            //The closeness is expressed as a percentage of how close to the other object they are. 100% being touching 0% being 1 pixel from out of range.
            foreach (var other in _core.Actors.Collection.Where(o => o is EnemyBase))
            {
                if (other == this)
                {
                    continue;
                }

                double distance = DistanceTo(other);
                double percentageOfCloseness = 1 - (distance / _maxObserveDistance);

                if (IsPointingAt(other, _visionToleranceDegrees, _maxObserveDistance, -90))
                {
                    aiParams.SetIfLess(AIInputs.In270Degrees, percentageOfCloseness);
                }

                if (IsPointingAt(other, _visionToleranceDegrees, _maxObserveDistance, -45))
                {
                    aiParams.SetIfLess(AIInputs.In315Degrees, percentageOfCloseness);
                }

                if (IsPointingAt(other, _visionToleranceDegrees, _maxObserveDistance, 0))
                {
                    aiParams.SetIfLess(AIInputs.In0Degrees, percentageOfCloseness);
                }

                if (IsPointingAt(other, _visionToleranceDegrees, _maxObserveDistance, +45))
                {
                    aiParams.SetIfLess(AIInputs.In45Degrees, percentageOfCloseness);
                }

                if (IsPointingAt(other, _visionToleranceDegrees, _maxObserveDistance, +90))
                {
                    aiParams.SetIfLess(AIInputs.In90Degrees, percentageOfCloseness);
                }
            }

            return aiParams;
        }

        #endregion
    }
}
