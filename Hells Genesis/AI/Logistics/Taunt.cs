using Determinet;
using Determinet.Types;
using HG.Actors.BaseClasses;
using HG.Engine;
using HG.Types;
using HG.Utility.ExtensionMethods;
using System;
using System.Diagnostics;
using static HG.Engine.Constants;

namespace HG.AI.Logistics
{
    /// <summary>
    /// Finite-state-machine where AI decides the states. "Taunt" keeps an object swooping in and out. Very near and somewhat aggressively.
    /// </summary>
    internal class Taunt : IAIController
    {
        private const string _assetPath = @"AI\Logistics\FlyBy.txt";

        private readonly Core _core;
        private readonly ActorBase _owner;
        private readonly ActorBase _observedObject;

        #region I/O Enumerations.

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
        private readonly double _idealMaxDistance = 2000;
        private readonly double _idealMinDistance = 500;
        private DateTime? _lastDecisionTime = DateTime.Now.AddHours(-1);
        private readonly int _millisecondsBetweenDecisions = 10000;
        private HgNormalizedAngle _evasiveLoopTargetAngle = new();
        private RelativeDirection _evasiveLoopDirection;

        #endregion

        public DniNeuralNetwork Network { get; set; }

        private static DniNeuralNetwork _singletonNetwork = null;

        /// <summary>
        /// Creates a new instance of the intelligence object.
        /// </summary>
        /// <param name="core">Engine core instance.</param>
        /// <param name="owner">The object which is intelligent.</param>
        /// <param name="observedObject">The object for which the intelligent object will be observing for inputs.</param>
        public Taunt(Core core, ActorBase owner, ActorBase observedObject)
        {
            _core = core;
            _owner = owner;
            _observedObject = observedObject;

            owner.OnHit += Owner_OnHit;

            SetNeuralNetwork();
        }

        private void Owner_OnHit(ActorBase sender, HgDamageType damageType, int damageAmount)
        {
            AlterActionState(ActionState.EvasiveLoop); //If you hit me, I will take off!
        }

        private void AlterActionState(ActionState state)
        {
            Debug.Print($"{state}");
            switch (state)
            {
                case ActionState.EvasiveLoop:
                    _evasiveLoopTargetAngle.Degrees = _owner.Velocity.Angle.Degrees + 180;
                    _evasiveLoopDirection = HgRandom.FlipCoin() ? RelativeDirection.Left : RelativeDirection.Right;
                    _owner.Velocity.ThrottlePercentage = 1.0;
                    _owner.Velocity.AvailableBoost = 250;
                    break;
                case ActionState.TransitionToDepart:
                    {
                        _owner.Velocity.AvailableBoost = 100;
                        break;
                    }
            }

            _currentAction = state;
        }

        private enum ActionState
        {
            None,
            TransitionToApproach,
            TransitionToDepart,
            Approaching,
            Departing,
            EvasiveLoop
        }

        public void ApplyIntelligence(HgPoint<double> displacementVector)
        {
            var now = DateTime.UtcNow;
            var elapsedTimeSinceLastDecision = (now - (DateTime)_lastDecisionTime).TotalMilliseconds;
            //If its been awhile since we thought about anything, do some thinking.
            if (elapsedTimeSinceLastDecision >= _millisecondsBetweenDecisions || _currentAction == ActionState.None)
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

                //if (_currentAction == ActionState.None) //We're just cruising, make a decision about the next state.
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
                    AlterActionState(ActionState.Departing);
                }
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
            if (_singletonNetwork != null)
            {
                Network = _singletonNetwork.Clone();//.Mutate(0.2, 0.1);
                return;
            }

            var networkJson = _core.Assets.GetText(_assetPath);
            if (string.IsNullOrEmpty(networkJson) == false)
            {
                var loadedNetwork = DniNeuralNetwork.LoadFromText(networkJson);
                if (loadedNetwork != null)
                {
                    _singletonNetwork = loadedNetwork;
                    Network = loadedNetwork.Clone();//.Mutate(0.2, 0.1)
                    return;
                }
            }

            #region New neural network and training.

            var newNetwork = new DniNeuralNetwork()
            {
                LearningRate = 0.01
            };

            //Vision inputs.
            newNetwork.Layers.AddInput(ActivationType.LeakyReLU,
                new object[] {
                        AIInputs.DistanceFromObservedObject,
                        AIInputs.AngleToObservedObjectIn6thRadians
                });

            //Where the magic happens.
            newNetwork.Layers.AddIntermediate(ActivationType.Sigmoid, 8);

            //Decision outputs
            newNetwork.Layers.AddOutput(
                new object[] {
                        AIOutputs.TransitionToObservedObject,
                        AIOutputs.TransitionFromObservedObject,
                        AIOutputs.SpeedAdjust
                });

            for (int epoch = 0; epoch < 5000; epoch++)
            {
                //Very close to observed object, get away.
                newNetwork.BackPropagate(TrainingScenerio(0, 0), TrainingDecision(0, 1, 1));
                newNetwork.BackPropagate(TrainingScenerio(0, -1), TrainingDecision(0, 1, 1));
                newNetwork.BackPropagate(TrainingScenerio(0, 1), TrainingDecision(0, 1, 1));
                newNetwork.BackPropagate(TrainingScenerio(0, 0.5), TrainingDecision(0, 1, 1));
                newNetwork.BackPropagate(TrainingScenerio(0, -0.5), TrainingDecision(0, 1, 1));

                //Pretty close to observed object, get away.
                newNetwork.BackPropagate(TrainingScenerio(0.25, 0), TrainingDecision(0, 1, 0.6));
                newNetwork.BackPropagate(TrainingScenerio(0.25, -1), TrainingDecision(0, 1, 0.6));
                newNetwork.BackPropagate(TrainingScenerio(0.25, 1), TrainingDecision(0, 1, 0.6));
                newNetwork.BackPropagate(TrainingScenerio(0.25, 0.5), TrainingDecision(0, 1, 0.6));
                newNetwork.BackPropagate(TrainingScenerio(0.25, -0.5), TrainingDecision(0, 1, 0.6));

                //Very far from observed object, get closer.
                newNetwork.BackPropagate(TrainingScenerio(1, 0), TrainingDecision(2, 0, 0));
                newNetwork.BackPropagate(TrainingScenerio(1, -1), TrainingDecision(2, 0, 0));
                newNetwork.BackPropagate(TrainingScenerio(1, 1), TrainingDecision(2, 0, 0));
                newNetwork.BackPropagate(TrainingScenerio(1, 0.5), TrainingDecision(2, 0, 0));
                newNetwork.BackPropagate(TrainingScenerio(1, -0.5), TrainingDecision(2, 0, 0));

                //Pretty far from observed object, get closer.
                newNetwork.BackPropagate(TrainingScenerio(0.75, 0), TrainingDecision(1, 0, 0));
                newNetwork.BackPropagate(TrainingScenerio(0.75, -1), TrainingDecision(1, 0, 0));
                newNetwork.BackPropagate(TrainingScenerio(0.75, 1), TrainingDecision(1, 0, 0));
                newNetwork.BackPropagate(TrainingScenerio(0.75, 0.5), TrainingDecision(1, 0, 0));
                newNetwork.BackPropagate(TrainingScenerio(0.75, -0.5), TrainingDecision(1, 0, 0));
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

            //newNetwork.Save(fileName);

            #endregion

            _singletonNetwork = newNetwork;
            Network = newNetwork.Clone();//.Mutate(0.2, 0.1)
        }


        private DniNamedInterfaceParameters GatherInputs()
        {
            var aiParams = new DniNamedInterfaceParameters();

            var distance = _owner.DistanceTo(_observedObject);
            var percentageOfCloseness = (distance / _idealMaxDistance).Box(0, 1);

            aiParams.Set(AIInputs.DistanceFromObservedObject, percentageOfCloseness);

            var deltaAngle = _owner.DeltaAngle(_observedObject);

            var angleToIn6thRadians = HgAngle<double>.DegreesToRadians(deltaAngle) / 6.0;

            aiParams.Set(AIInputs.AngleToObservedObjectIn6thRadians,
                angleToIn6thRadians.SplitToNegative(Math.PI / 6));

            return aiParams;
        }
    }
}
