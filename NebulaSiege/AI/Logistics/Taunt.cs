using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Utility;
using NebulaSiege.Utility.ExtensionMethods;
using NTDLS.Determinet;
using NTDLS.Determinet.Types;
using System;

namespace NebulaSiege.AI.Logistics
{

    /// <summary>
    /// Finite-state-machine where AI decides the states. "Taunt" keeps an object swooping in and out. Very near and somewhat aggressively.
    /// </summary>
    internal class Taunt : IAIController
    {
        private const string _assetPath = @"Data\AI\Logistics\Taunt.txt";

        private readonly EngineCore _core;
        private readonly _SpriteShipBase _owner;
        private readonly _SpriteBase _observedObject;

        #region "Enumerations".

        private static class RenewableResources
        {
            public static string Boost = "IAIController_Taunt_Boost";
        }

        private enum ActionState
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
            DistanceFromObservedObject,
            /// <summary>
            /// This should be the angle to the ObservedObject (likely the player) in relation to the nose of the enemy ship expressed in 1/6th radians split in half.
            /// 0 is pointing at the player, ~0.26 is 90° to the right, ~0.523 is 180° and -0.26 is 270° to the left.
            /// </summary>
            AngleToObservedObjectIn6thRadians
        }

        public enum AIOutputs
        {
            TransitionToObservedObject,
            TransitionFromObservedObject,
            SpeedAdjust
        }

        #endregion

        #region Instance parameters.

        private ActionState _currentAction = ActionState.None;
        private readonly double _idealMaxDistance = 1000;
        private readonly double _idealMinDistance = 500;
        private DateTime? _lastDecisionTime = DateTime.Now.AddHours(-1);
        private readonly int _millisecondsBetweenDecisions = 2000;
        private NsNormalizedAngle _evasiveLoopTargetAngle = new();
        private HgRelativeDirection _evasiveLoopDirection;

        #endregion

        public DniNeuralNetwork Network { get; set; }

        /// <summary>
        /// Creates a new instance of the intelligence object.
        /// </summary>
        /// <param name="core">Engine core instance.</param>
        /// <param name="owner">The object which is intelligent.</param>
        /// <param name="observedObject">The object for which the intelligent object will be observing for inputs.</param>
        public Taunt(EngineCore core, _SpriteShipBase owner, _SpriteBase observedObject)
        {
            _core = core;
            _owner = owner;
            _observedObject = observedObject;

            owner.OnHit += Owner_OnHit;

            _owner.RenewableResources.CreateResource(RenewableResources.Boost, 800, 0, 10);

            SetNeuralNetwork();
        }

        private void Owner_OnHit(_SpriteBase sender, HgDamageType damageType, int damageAmount)
        {
            if (sender.HullHealth <= 10)
            {
                AlterActionState(ActionState.EvasiveLoop);
            }
        }

        private void AlterActionState(ActionState state)
        {
            switch (state)
            {
                case ActionState.EvasiveLoop:
                    _evasiveLoopTargetAngle.Degrees = _owner.Velocity.Angle.Degrees + 180;
                    _evasiveLoopDirection = HgRandom.FlipCoin() ? HgRelativeDirection.Left : HgRelativeDirection.Right;
                    _owner.Velocity.ThrottlePercentage = 1.0;
                    _owner.Velocity.AvailableBoost = _owner.RenewableResources.Consume(RenewableResources.Boost, 250);
                    break;
                case ActionState.TransitionToDepart:
                    {
                        _owner.Velocity.AvailableBoost = _owner.RenewableResources.Consume(RenewableResources.Boost, 100);
                        break;
                    }
            }

            _currentAction = state;
        }

        public void ApplyIntelligence(NsPoint displacementVector)
        {
            var now = DateTime.UtcNow;
            var elapsedTimeSinceLastDecision = (now - (DateTime)_lastDecisionTime).TotalMilliseconds;

            //If its been awhile since we thought about anything, do some thinking.
            if (elapsedTimeSinceLastDecision >= _millisecondsBetweenDecisions && _currentAction == ActionState.None)
            {
                var decidingFactors = GatherInputs(); //Gather inputs.
                var decisions = Network.FeedForward(decidingFactors); //Make decisions.
                                                                      //Execute on those ↑ decisions:...
                var speedAdjust = decisions.Get(AIOutputs.SpeedAdjust);
                if (speedAdjust >= 0.5)
                {
                    _owner.Velocity.ThrottlePercentage = (_owner.Velocity.ThrottlePercentage + 0.01).Box(0.5, 1);
                }
                else if (speedAdjust < 0.5)
                {
                    _owner.Velocity.ThrottlePercentage = (_owner.Velocity.ThrottlePercentage - 0.01).Box(0.5, 1);
                }

                if (_currentAction == ActionState.None) //We're just cruising, make a decision about the next state.
                {
                    bool transitionToObservedObject = decisions.Get(AIOutputs.TransitionToObservedObject) > 0.99;
                    bool transitionFromObservedObject = decisions.Get(AIOutputs.TransitionFromObservedObject) > 0.99;

                    if (transitionToObservedObject && transitionFromObservedObject)
                    {
                        AlterActionState(ActionState.None);
                    }
                    else if (transitionToObservedObject)
                    {
                        AlterActionState(ActionState.TransitionToApproach);
                    }
                    else if (transitionFromObservedObject)
                    {
                        AlterActionState(ActionState.TransitionToDepart);
                    }
                    else
                    {
                        AlterActionState(ActionState.None);
                    }
                }
                _lastDecisionTime = now;
            }

            //We are evading, dont make any other decisions until evasion is complete.
            if (_currentAction == ActionState.EvasiveLoop)
            {
                if (_owner.RotateTo(_evasiveLoopTargetAngle.Degrees, _evasiveLoopDirection, 1, 30) == false)
                {
                    AlterActionState(ActionState.Escape);
                }
                return;
            }
            if (_currentAction == ActionState.Escape)
            {
                double distanceToPlayer = _owner.DistanceTo(_observedObject);
                if (distanceToPlayer < 500)
                {
                    if (HgRandom.FlipCoin())
                    {
                        _owner.Velocity.Angle.Degrees++;
                    }
                    else
                    {
                        _owner.Velocity.Angle.Degrees--;
                    }
                }

                //Just get away.
                return;
            }
            else if (_currentAction == ActionState.TransitionToApproach)
            {
                if (_owner.RotateTo(_observedObject, 1, 10) == false)
                {
                    AlterActionState(ActionState.Approaching);
                }
            }
            else if (_currentAction == ActionState.TransitionToDepart)
            {
                var distanceToObservedObject = _owner.DistanceTo(_observedObject);
                double augmentationDegrees = 0.2;

                //We are making the transition to our depart angle, but if we get too close then make the angle more agressive.
                double percentOfAllowableCloseness = (100 / distanceToObservedObject);
                if (percentOfAllowableCloseness.IsBetween(0, 1))
                {
                    augmentationDegrees += percentOfAllowableCloseness;
                }

                _owner.RotateTo(_observedObject, augmentationDegrees, 20);

                if (distanceToObservedObject > _idealMinDistance)
                {
                    AlterActionState(ActionState.Departing);
                }
            }
            else if (_currentAction == ActionState.Approaching)
            {
                var distanceTo = _owner.DistanceTo(_observedObject);

                if (distanceTo > _idealMaxDistance)
                {
                    _owner.Velocity.AvailableBoost = 100;
                }

                if (distanceTo < _idealMinDistance)
                {
                    AlterActionState(ActionState.None);
                }
            }
            else if (_currentAction == ActionState.Departing)
            {
                if (_owner.DistanceTo(_observedObject) > _idealMaxDistance)
                {
                    AlterActionState(ActionState.None);
                }
            }
        }

        private void SetNeuralNetwork()
        {
            Network = _core.Assets.GetNeuralNetwork(_assetPath);

            if (Network == null)
            {
                Network = new DniNeuralNetwork
                {
                    LearningRate = 0.01
                };

                #region New neural network and training.

                //Vision inputs.
                Network.Layers.AddInput(ActivationType.LeakyReLU,
                    new object[] {
                        AIInputs.DistanceFromObservedObject,
                        AIInputs.AngleToObservedObjectIn6thRadians
                    });

                //Where the magic happens.
                Network.Layers.AddIntermediate(ActivationType.Sigmoid, 8);

                //Decision outputs
                Network.Layers.AddOutput(
                    new object[] {
                        AIOutputs.TransitionToObservedObject,
                        AIOutputs.TransitionFromObservedObject,
                        AIOutputs.SpeedAdjust
                    });

                for (int epoch = 0; epoch < 5000; epoch++)
                {
                    //Very close to observed object, get away.
                    Network.BackPropagate(TrainingScenerio(0, 0), TrainingDecision(0, 1, 1));
                    Network.BackPropagate(TrainingScenerio(0, -1), TrainingDecision(0, 1, 1));
                    Network.BackPropagate(TrainingScenerio(0, 1), TrainingDecision(0, 1, 1));
                    Network.BackPropagate(TrainingScenerio(0, 0.5), TrainingDecision(0, 1, 1));
                    Network.BackPropagate(TrainingScenerio(0, -0.5), TrainingDecision(0, 1, 1));

                    //Pretty close to observed object, get away.
                    Network.BackPropagate(TrainingScenerio(0.25, 0), TrainingDecision(0, 1, 0.6));
                    Network.BackPropagate(TrainingScenerio(0.25, -1), TrainingDecision(0, 1, 0.6));
                    Network.BackPropagate(TrainingScenerio(0.25, 1), TrainingDecision(0, 1, 0.6));
                    Network.BackPropagate(TrainingScenerio(0.25, 0.5), TrainingDecision(0, 1, 0.6));
                    Network.BackPropagate(TrainingScenerio(0.25, -0.5), TrainingDecision(0, 1, 0.6));

                    //Very far from observed object, get closer.
                    Network.BackPropagate(TrainingScenerio(1, 0), TrainingDecision(2, 0, 0));
                    Network.BackPropagate(TrainingScenerio(1, -1), TrainingDecision(2, 0, 0));
                    Network.BackPropagate(TrainingScenerio(1, 1), TrainingDecision(2, 0, 0));
                    Network.BackPropagate(TrainingScenerio(1, 0.5), TrainingDecision(2, 0, 0));
                    Network.BackPropagate(TrainingScenerio(1, -0.5), TrainingDecision(2, 0, 0));

                    //Pretty far from observed object, get closer.
                    Network.BackPropagate(TrainingScenerio(0.75, 0), TrainingDecision(1, 0, 0));
                    Network.BackPropagate(TrainingScenerio(0.75, -1), TrainingDecision(1, 0, 0));
                    Network.BackPropagate(TrainingScenerio(0.75, 1), TrainingDecision(1, 0, 0));
                    Network.BackPropagate(TrainingScenerio(0.75, 0.5), TrainingDecision(1, 0, 0));
                    Network.BackPropagate(TrainingScenerio(0.75, -0.5), TrainingDecision(1, 0, 0));
                }

                static DniNamedInterfaceParameters TrainingScenerio(double distanceFromObservedObject, double angleToObservedObjectIn10thRadians)
                {
                    var param = new DniNamedInterfaceParameters();

                    param.Set(AIInputs.DistanceFromObservedObject, distanceFromObservedObject);
                    param.Set(AIInputs.AngleToObservedObjectIn6thRadians, angleToObservedObjectIn10thRadians);
                    return param;
                }

                static DniNamedInterfaceParameters TrainingDecision(double transitionToObservedObject,
                    double transitionFromObservedObject, double speedAdjust)
                {
                    var param = new DniNamedInterfaceParameters();
                    param.Set(AIOutputs.TransitionToObservedObject, transitionToObservedObject);
                    param.Set(AIOutputs.TransitionFromObservedObject, transitionFromObservedObject);
                    param.Set(AIOutputs.SpeedAdjust, speedAdjust);
                    return param;
                }
                #endregion

                _core.Assets.PutText(_assetPath, Network.Serialize());
            }
        }

        private DniNamedInterfaceParameters GatherInputs()
        {
            var aiParams = new DniNamedInterfaceParameters();

            var distance = _owner.DistanceTo(_observedObject);
            var percentageOfCloseness = (distance / _idealMaxDistance).Box(0, 1);

            aiParams.Set(AIInputs.DistanceFromObservedObject, percentageOfCloseness);

            var deltaAngle = _owner.DeltaAngle(_observedObject);

            var angleToIn6thRadians = NsAngle.DegreesToRadians(deltaAngle) / 6.0;

            aiParams.Set(AIInputs.AngleToObservedObjectIn6thRadians,
                angleToIn6thRadians.SplitToNegative(Math.PI / 6));

            return aiParams;
        }
    }
}
