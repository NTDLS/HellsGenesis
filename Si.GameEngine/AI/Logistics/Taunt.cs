using Si.GameEngine.AI.Logistics._Superclass;
using Si.GameEngine.Core;
using Si.GameEngine.Sprites._Superclass;
using Si.Library;
using Si.Library.Types.Geometry;
using System.Diagnostics;
using static Si.Library.SiConstants;

namespace Si.GameEngine.AI.Logistics
{
    /// <summary>
    /// Finite-state-machine where AI decides the states. "Taunt" keeps an object swooping in and out. Very near and somewhat aggressively.
    /// </summary>
    internal class Taunt : IIANeuralNetworkStateMachine
    {
        private readonly GameEngineCore _gameEngine;
        private readonly SpriteShipBase _owner;
        private readonly SpriteBase _observedObject;

        #region "Enumerations".

        private static class RenewableResources
        {
            public static string Boost = "IIAController_Taunt_Boost";
        }

        private enum AIActivity
        {
            None,
            TransitionToApproach,
            TransitionToDepart,
            Approaching,
            Departing,
            EvasiveLoop,
            Escape
        }

        #endregion

        #region Instance parameters.

        private readonly double _approachAngle = SiRandom.Variance(45, 0.2);
        private AIActivity _currentActivity = AIActivity.None;
        private readonly double _idealMaxDistance = SiRandom.Variance(2500, 0.20);
        private readonly double _idealMinDistance = SiRandom.Variance(800, 0.10);
        private readonly SiNormalizedAngle _evasiveLoopTargetAngle = new();
        private SiRelativeDirection _rotationDirection;

        #endregion

        /// <summary>
        /// Creates a new instance of the intelligence object.
        /// </summary>
        /// <param name="core">Engine core instance.</param>
        /// <param name="owner">The object which is intelligent.</param>
        /// <param name="observedObject">The object for which the intelligent object will be observing for inputs.</param>
        public Taunt(GameEngineCore gameEngine, SpriteShipBase owner, SpriteBase observedObject)
        {
            _gameEngine = gameEngine;
            _owner = owner;
            _observedObject = observedObject;

            owner.OnHit += Owner_OnHit;

            _owner.RenewableResources.Create(RenewableResources.Boost, 800, 0, 10);

            SetCurrentActivity(AIActivity.Departing);
            _owner.Velocity.ThrottlePercentage = 1.0;
        }

        private void Owner_OnHit(SpriteBase sender, SiDamageType damageType, int damageAmount)
        {
            if (sender.HullHealth <= 10)
            {
                SetCurrentActivity(AIActivity.EvasiveLoop);
            }
        }

        private void SetCurrentActivity(AIActivity state)
        {
            Debug.WriteLine($"Activity: {state}");

            _rotationDirection = SiRandom.FlipCoin() ? SiRelativeDirection.Left : SiRelativeDirection.Right;

            switch (state)
            {
                case AIActivity.EvasiveLoop:
                    _evasiveLoopTargetAngle.Degrees = _owner.Velocity.Angle.Degrees + 180;
                    _owner.Velocity.ThrottlePercentage = 1.0;
                    _owner.Velocity.AvailableBoost = _owner.RenewableResources.Consume(RenewableResources.Boost, 250);
                    break;
                case AIActivity.TransitionToDepart:
                    {
                        _owner.Velocity.AvailableBoost = _owner.RenewableResources.Consume(RenewableResources.Boost, 100);
                        break;
                    }
            }

            _currentActivity = state;
        }

        public void ApplyIntelligence(SiPoint displacementVector)
        {
            if (_currentActivity == AIActivity.Departing)
            {
                //Just cruise on the current vector.

                if (_owner.DistanceTo(_observedObject) > _idealMaxDistance)
                {
                    SetCurrentActivity(AIActivity.TransitionToApproach);
                }
            }
            else if (_currentActivity == AIActivity.TransitionToApproach)
            {
                //Turn until pointing at the observed object.

                if (_owner.RotateIfNotPointingAt(_observedObject, _rotationDirection, 1, _approachAngle) == false)
                {
                    SetCurrentActivity(AIActivity.Approaching);
                }
            }
            if (_currentActivity == AIActivity.Approaching)
            {
                //Follow the observed object until we get to the _idealMinDistance
                _owner.RotateIfNotPointingAt(_observedObject, 1, _approachAngle);

                if (_owner.DistanceTo(_observedObject) < _idealMinDistance)
                {
                    SetCurrentActivity(AIActivity.TransitionToDepart);
                }
            }
            else if (_currentActivity == AIActivity.TransitionToDepart)
            {
                var distanceToObservedObject = _owner.DistanceTo(_observedObject);

                //As we get closer, make the angle more agressive.
                double rotationAmount = (1 - (distanceToObservedObject / _idealMinDistance)) * 2.0;

                //Rotate as long as we are facing the observed object. If we are no longer facing, then depart.
                if (_owner.RotateIfPointingAt(_observedObject, _rotationDirection, rotationAmount, _approachAngle) == false)
                {
                    SetCurrentActivity(AIActivity.Departing);
                }
            }
            if (_currentActivity == AIActivity.EvasiveLoop)
            {
                if (_owner.RotateIfNotPointingAt(_evasiveLoopTargetAngle.Degrees, _rotationDirection, 1, _approachAngle) == false)
                {
                    SetCurrentActivity(AIActivity.Escape);
                }
            }
            else if (_currentActivity == AIActivity.Escape)
            {
                if (_owner.RenewableResources.Observe(RenewableResources.Boost) > 250)
                {
                    _owner.Velocity.AvailableBoost = _owner.RenewableResources.Consume(RenewableResources.Boost, SiRandom.Variance(250, 0.5));
                }

                double distanceToObservedObject = _owner.DistanceTo(_observedObject);
                if (distanceToObservedObject > _idealMaxDistance)
                {
                    SetCurrentActivity(AIActivity.Departing);
                }
                else if (distanceToObservedObject < _idealMinDistance)
                {
                    SetCurrentActivity(AIActivity.EvasiveLoop);
                }
            }
        }
    }
}
