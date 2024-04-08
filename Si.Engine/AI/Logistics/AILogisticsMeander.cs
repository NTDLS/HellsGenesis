using Si.Engine.Sprite._Superclass;
using Si.GameEngine.AI._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System;
namespace Si.Engine.AI.Logistics
{
    /// <summary>
    /// Keeps an object at a generally safe distance from another object.
    /// </summary>
    internal class AILogisticsMeander : AIStateMachine
    {
        private DateTime _lastDecisionTime = DateTime.UtcNow;
        private readonly float _millisecndsBetweenDecisions = SiRandom.Between(2000, 10000);
        private float _angleToAdd = SiRandom.Between(0.004f, 0.006f);
        private readonly float _varianceAngleForTravel = SiRandom.Between(5, 15);
        private readonly float _idealMaxDistance = SiRandom.Variance(8000, 0.20f);
        private readonly float _idealMinDistance = SiRandom.Variance(2500, 0.10f);

        public AILogisticsMeander(EngineCore engine, SpriteInteractiveShipBase owner, SpriteBase observedObject)
            : base(engine, owner, observedObject)
        {
            owner.RecalculateMovementVector();
            OnApplyIntelligence += AILogistics_OnApplyIntelligence;
        }

        private void AILogistics_OnApplyIntelligence(float epoch, SiVector displacementVector, AIState state)
        {
            var distanceToObservedObject = Owner.DistanceTo(ObservedObject);

            if (distanceToObservedObject > _idealMaxDistance)
            {
                if (Owner.IsPointingAt(ObservedObject, _varianceAngleForTravel) == false)
                {
                    Owner.RotateMovementVector(_angleToAdd * epoch);
                }
            }
            else if (distanceToObservedObject < _idealMinDistance)
            {
                if (Owner.IsPointingAway(ObservedObject, _varianceAngleForTravel) == false)
                {
                    Owner.RotateMovementVector(_angleToAdd * epoch);
                }
            }
            else
            {
                Owner.PointingAngle.Radians += (_angleToAdd * epoch); //Just do loops.

                if ((DateTime.UtcNow - _lastDecisionTime).TotalMilliseconds > _millisecndsBetweenDecisions) //Change directions from time to time.
                {
                    _angleToAdd = SiRandom.Between(0.004f, 0.006f);
                    if (SiRandom.PercentChance(50))
                    {
                        _angleToAdd *= -1;
                    }
                    _lastDecisionTime = DateTime.UtcNow;
                }
            }
        }
    }
}
