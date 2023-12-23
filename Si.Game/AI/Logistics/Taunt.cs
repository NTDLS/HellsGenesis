using NTDLS.Determinet;
using NTDLS.Determinet.Types;
using Si.Game.Engine;
using Si.Game.Sprites;
using Si.Shared;
using Si.Shared.ExtensionMethods;
using Si.Shared.Types.Geometry;
using System;
using System.Diagnostics;
using static Si.Shared.SiConstants;

namespace Si.Game.AI.Logistics
{
    /// <summary>
    /// Finite-state-machine where AI decides the states. "Taunt" keeps an object swooping in and out. Very near and somewhat aggressively.
    /// </summary>
    internal class Taunt : IAIController
    {
        private const string _assetPath = @"Data\AI\Logistics\Taunt.txt";

        private readonly EngineCore _gameCore;
        private readonly _SpriteShipBase _owner;
        private readonly SpriteBase _observedObject;

        #region "Enumerations".

        private static class RenewableResources
        {
            public static string Boost = "IAIController_Taunt_Boost";
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

        public enum AIInputs
        {
            DistanceFromObject,
            /// <summary>
            /// This should be the angle to the ObservedObject (likely the player) in relation to the nose of the enemy ship expressed in 1/6th radians split in half.
            /// 0 is pointing at the player, ~0.26 is 90° to the right, ~0.523 is 180° and -0.26 is 270° to the left.
            /// </summary>
            AngleToObject
        }

        public enum AIOutputs
        {
            TransitionToObservedObject,
            TransitionFromObservedObject,
            SpeedAdjust
        }

        #endregion

        #region Instance parameters.

        private AIActivity _currentActivity = AIActivity.None;
        private readonly double _idealMaxDistance = 1500;
        private readonly double _idealMinDistance = 500;
        private DateTime? _lastDecisionTime = DateTime.Now.AddHours(-1);
        private readonly int _millisecondsBetweenDecisions = 2000;
        private readonly SiNormalizedAngle _evasiveLoopTargetAngle = new();
        private SiRelativeDirection _evasiveLoopDirection;
        private double _departureAngle = 0;

        #endregion

        public DniNeuralNetwork Network { get; set; }

        /// <summary>
        /// Creates a new instance of the intelligence object.
        /// </summary>
        /// <param name="core">Engine core instance.</param>
        /// <param name="owner">The object which is intelligent.</param>
        /// <param name="observedObject">The object for which the intelligent object will be observing for inputs.</param>
        public Taunt(EngineCore gameCore, _SpriteShipBase owner, SpriteBase observedObject)
        {
            _gameCore = gameCore;
            _owner = owner;
            _observedObject = observedObject;

            owner.OnHit += Owner_OnHit;

            _owner.RenewableResources.CreateResource(RenewableResources.Boost, 800, 0, 10);

            Network = _gameCore.Assets.GetNeuralNetwork(_assetPath) ?? TrainNetwork();
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

            switch (state)
            {
                case AIActivity.EvasiveLoop:
                    _evasiveLoopTargetAngle.Degrees = _owner.Velocity.Angle.Degrees + 180;
                    _evasiveLoopDirection = SiRandom.FlipCoin() ? SiRelativeDirection.Left : SiRelativeDirection.Right;
                    _owner.Velocity.ThrottlePercentage = 1.0;
                    _owner.Velocity.AvailableBoost = _owner.RenewableResources.Consume(RenewableResources.Boost, 250);
                    break;
                case AIActivity.TransitionToDepart:
                    {
                        var deltaAngle = _owner.DeltaAngle(_observedObject);
                        if (deltaAngle < 0) _departureAngle = -0.2;
                        if (deltaAngle > 0) _departureAngle = 0.2;

                        _owner.Velocity.AvailableBoost = _owner.RenewableResources.Consume(RenewableResources.Boost, 100);
                        break;
                    }
            }

            _currentActivity = state;
        }

        public void ApplyIntelligence(SiPoint displacementVector)
        {
            var now = DateTime.UtcNow;
            var elapsedTimeSinceLastDecision = (now - (DateTime)_lastDecisionTime).TotalMilliseconds;

            //If its been awhile since we thought about anything, do some thinking.
            if (elapsedTimeSinceLastDecision >= _millisecondsBetweenDecisions && _currentActivity == AIActivity.None)
            {
                var decidingFactors = GatherInputs(); //Gather inputs.
                var decisions = Network.FeedForward(decidingFactors); //Make decisions.

                var speedAdjust = decisions.Get(AIOutputs.SpeedAdjust);
                if (speedAdjust >= 0.5)
                {
                    _owner.Velocity.ThrottlePercentage = (_owner.Velocity.ThrottlePercentage + 0.01).Box(0.5, 1);
                }
                else if (speedAdjust < 0.5)
                {
                    _owner.Velocity.ThrottlePercentage = (_owner.Velocity.ThrottlePercentage - 0.01).Box(0.5, 1);
                }

                if (_currentActivity == AIActivity.None) //We're just cruising, make a decision about the next state.
                {
                    double positiveThreshold = 0.9;

                    bool transitionToObservedObject = decisions.Get(AIOutputs.TransitionToObservedObject) >= positiveThreshold;
                    bool transitionFromObservedObject = decisions.Get(AIOutputs.TransitionFromObservedObject) >= positiveThreshold;

                    if (transitionToObservedObject && transitionFromObservedObject)
                    {
                        SetCurrentActivity(AIActivity.None); //Indecision... just cruise I suppose.
                    }
                    else if (transitionToObservedObject)
                    {
                        SetCurrentActivity(AIActivity.TransitionToApproach);
                    }
                    else if (transitionFromObservedObject)
                    {
                        SetCurrentActivity(AIActivity.TransitionToDepart);
                    }
                    else
                    {
                        SetCurrentActivity(AIActivity.None);
                    }
                }
                _lastDecisionTime = now;
            }

            if (_currentActivity == AIActivity.EvasiveLoop)
            {
                if (_owner.RotateTo(_evasiveLoopTargetAngle.Degrees, _evasiveLoopDirection, 1, 30) == false)
                {
                    SetCurrentActivity(AIActivity.Escape);
                }
            }
            else if (_currentActivity == AIActivity.Escape)
            {
                double distanceToPlayer = _owner.DistanceTo(_observedObject);
                if (distanceToPlayer < 500)
                {
                    if (SiRandom.FlipCoin())
                    {
                        _owner.Velocity.Angle.Degrees++;
                    }
                    else
                    {
                        _owner.Velocity.Angle.Degrees--;
                    }
                }
            }
            else if (_currentActivity == AIActivity.TransitionToApproach)
            {
                if (_owner.RotateTo(_observedObject, 1, 10) == false)
                {
                    SetCurrentActivity(AIActivity.Approaching);
                }
            }
            else if (_currentActivity == AIActivity.TransitionToDepart)
            {
                var distanceToObservedObject = _owner.DistanceTo(_observedObject);
                double augmentationDegrees = _departureAngle;

                //We are making the transition to our depart angle, but if we get too close then make the angle more agressive.
                double percentOfAllowableCloseness = (100 / distanceToObservedObject);
                if (percentOfAllowableCloseness.IsBetween(0, 1))
                {
                    augmentationDegrees += _departureAngle;
                }

                _owner.RotateTo(_observedObject, augmentationDegrees, 20);

                if (distanceToObservedObject > _idealMinDistance)
                {
                    SetCurrentActivity(AIActivity.Departing);
                }
            }
            else if (_currentActivity == AIActivity.Approaching)
            {
                var distanceTo = _owner.DistanceTo(_observedObject);

                if (distanceTo > _idealMaxDistance)
                {
                    _owner.Velocity.AvailableBoost = 100;
                }

                //The player has evaded the aproach, try TransitionToApproach again.
                if (_owner.IsPointingAway(_observedObject, 45))
                {
                    SetCurrentActivity(AIActivity.TransitionToApproach);
                }

                if (distanceTo < _idealMinDistance)
                {
                    SetCurrentActivity(AIActivity.None);
                }
            }
            else if (_currentActivity == AIActivity.Departing)
            {
                if (_owner.DistanceTo(_observedObject) > _idealMaxDistance)
                {
                    SetCurrentActivity(AIActivity.None);
                }
            }
        }

        private DniNamedInterfaceParameters GatherInputs()
        {
            var aiParams = new DniNamedInterfaceParameters();

            var distance = _owner.DistanceTo(_observedObject);
            var percentageOfCloseness = (distance / _idealMaxDistance).Box(0, 1);
            aiParams.Set(AIInputs.DistanceFromObject, percentageOfCloseness);

            var deltaAngle = _owner.DeltaAngle(_observedObject);

            Debug.WriteLine($"Distance:{percentageOfCloseness:n2}, Angle: {deltaAngle / 180.0:n2}");

            aiParams.Set(AIInputs.AngleToObject, deltaAngle / 180.0);


            return aiParams;
        }

        private DniNeuralNetwork TrainNetwork()
        {
            var network = new DniNeuralNetwork()
            {
                LearningRate = 0.025
            };

            #region New neural network and training.

            //Vision inputs.
            network.Layers.AddInput(ActivationType.LeakyReLU,
                new object[] {
                        AIInputs.DistanceFromObject,
                        AIInputs.AngleToObject
                });

            //Where the magic happens.
            network.Layers.AddIntermediate(ActivationType.Sigmoid, 16);

            //Decision outputs
            network.Layers.AddOutput(
                new object[] {
                        AIOutputs.TransitionToObservedObject,
                        AIOutputs.TransitionFromObservedObject,
                        AIOutputs.SpeedAdjust
                });

            for (int epoch = 0; epoch < 10000; epoch++)
            {
                //Object in front of observed object:
                for (double angleToObject = -0.25; angleToObject < 0.25; angleToObject += 0.1)
                {
                    //Somewhat close and very close to observed object, get away.
                    network.BackPropagate(TrainingScenerio(0.25, angleToObject), TrainingDecision(0, 1, 0.6));
                    network.BackPropagate(TrainingScenerio(0, angleToObject), TrainingDecision(0, 1, 1));
                }

                //Object behind observed object:
                for (double angleToObject = -0.75; angleToObject < 0.75; angleToObject += 0.1)
                {
                    //Somewhat close and very close to observed object, get away.
                    network.BackPropagate(TrainingScenerio(0.25, angleToObject), TrainingDecision(0, 1, 0.6));
                    network.BackPropagate(TrainingScenerio(0, angleToObject), TrainingDecision(0, 1, 1));
                }

                //Object at any angle from observed object, but far away:
                for (double angleToObject = -1; angleToObject < 1; angleToObject += 0.1)
                {
                    //Somewhat far and very far to observed object, get closer.
                    network.BackPropagate(TrainingScenerio(0.75, angleToObject), TrainingDecision(1, 0, 0));
                    network.BackPropagate(TrainingScenerio(1, angleToObject), TrainingDecision(1, 0, 0));
                }
            }

            static DniNamedInterfaceParameters TrainingScenerio(double distanceFromObject, double angleToObject)
            {
                var param = new DniNamedInterfaceParameters();

                param.Set(AIInputs.DistanceFromObject, distanceFromObject);
                param.Set(AIInputs.AngleToObject, angleToObject);
                return param;
            }

            static DniNamedInterfaceParameters TrainingDecision(double transitionToObject,
                double transitionFromObject, double speedAdjust)
            {
                var param = new DniNamedInterfaceParameters();
                param.Set(AIOutputs.TransitionToObservedObject, transitionToObject);
                param.Set(AIOutputs.TransitionFromObservedObject, transitionFromObject);
                param.Set(AIOutputs.SpeedAdjust, speedAdjust);
                return param;
            }
            #endregion

            _gameCore.Assets.PutText(_assetPath, network.Serialize());

            return network;
        }
    }
}
