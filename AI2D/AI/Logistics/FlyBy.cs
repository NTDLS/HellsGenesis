using AI2D.Actors;
using AI2D.Engine;
using AI2D.Types;
using AI2D.Types.ExtensionMethods;
using Determinet;
using Determinet.Types;
using System;
using System.IO;

namespace AI2D.AI.Logistics
{
    /// <summary>
    /// AI brain to keep an object swooping in and out. Very near and somewhat agressively.
    /// </summary>
    public class FlyBy : IAIController
    {
        private const string _assetPath = @"..\..\..\Assets\AI\Logistics\FlyBy.txt";

        private Core _core;
        private ActorBase _owner;
        private ActorBase _observedObject;

        #region I/O Enumerations.

        public enum Inputs
        {
            DistanceFromObservationObject,
            /// <summary>
            /// This should be the angle to the ObservationObject (likely the player) in relation to the nose of the enemy ship expressed in 1/6th radians split in half.
            /// 0 is pointing at the player, ~0.26 is 90° to the right, ~0.523 is 180° and -0.26 is 270° to the left.
            /// </summary>
            AngleToObservationObjectIn6thRadians
        }

        public enum Outputs
        {
            TransitionToObservationObject,
            TransitionFromObservationObject,
            SpeedAdjust
        }

        #endregion

        #region Instance parameters.

        public double DistanceToKeep { get; set; } = 1000;
        public DateTime? LastDecisionTime { get; set; } = DateTime.Now.AddHours(-1);
        public int MillisecondsBetweenDecisions { get; set; } = 50;

        #endregion

        public DniNeuralNetwork Network { get; set; }

        private static DniNeuralNetwork _singletonNetwork = null;

        /// <summary>
        /// Creates a new instance of the intelligence object.
        /// </summary>
        /// <param name="core">Engine core instance.</param>
        /// <param name="owner">The object which is intelligent.</param>
        /// <param name="observedObject">The object for which the intelligent object will be observing for inputs.</param>
        public FlyBy(Core core, ActorBase owner, ActorBase observedObject)
        {
            _core = core;
            _owner = owner;
            _observedObject = observedObject;

            if (_singletonNetwork != null)
            {
                Network = _singletonNetwork.Clone();//.Mutate(0.2, 0.1);
                return;
            }

            if (string.IsNullOrEmpty(_assetPath) == false && File.Exists(_assetPath))
            {
                var loadedNetwork = DniNeuralNetwork.Load(_assetPath);
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
                        Inputs.DistanceFromObservationObject,
                        Inputs.AngleToObservationObjectIn6thRadians
                });

            //Where the magic happens.
            newNetwork.Layers.AddIntermediate(ActivationType.Sigmoid, 8);

            //Decision outputs
            newNetwork.Layers.AddOutput(
                new object[] {
                        Outputs.TransitionToObservationObject,
                        Outputs.TransitionFromObservationObject,
                        Outputs.SpeedAdjust
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

            static DniNamedInterfaceParameters TrainingScenerio(double distanceFromObservationObject, double angleToObservationObjectIn10thRadians)
            {
                var param = new DniNamedInterfaceParameters();

                param.Set(Inputs.DistanceFromObservationObject, distanceFromObservationObject);
                param.Set(Inputs.AngleToObservationObjectIn6thRadians, angleToObservationObjectIn10thRadians);
                return param;
            }

            static DniNamedInterfaceParameters TrainingDecision(double transitionToObservationObject,
                double transitionFromObservationObject, double speedAdjust)
            {
                var param = new DniNamedInterfaceParameters();
                param.Set(Outputs.TransitionToObservationObject, transitionToObservationObject);
                param.Set(Outputs.TransitionFromObservationObject, transitionFromObservationObject);
                param.Set(Outputs.SpeedAdjust, speedAdjust);
                return param;
            }

            //newNetwork.Save(fileName);

            #endregion

            _singletonNetwork = newNetwork;
            Network = newNetwork.Clone();//.Mutate(0.2, 0.1)
        }

        enum ActionState
        {
            None,
            MovingToApproach,
            MovingToDepart,
            Approaching,
            Departing
        }

        private ActionState _currentAction = ActionState.None;
        private ActionState CurrentAction
        {
            get { return _currentAction; }
            set
            {
                if (_currentAction != value)
                {
                    //Debug.Print($"{value}");
                }
                _currentAction = value;
            }
        }

        public void ApplyIntelligence(Point<double> frameAppliedOffset)
        {
            var now = DateTime.UtcNow;

            var elapsedTimeSinceLastDecision = (now - (DateTime)LastDecisionTime).TotalMilliseconds;

            if (elapsedTimeSinceLastDecision >= MillisecondsBetweenDecisions)
            {
                var decidingFactors = GatherInputs();
                var decisions = Network.FeedForward(decidingFactors);

                var speedAdjust = decisions.Get(Outputs.SpeedAdjust);

                if (speedAdjust >= 0.5)
                {
                    _owner.Velocity.ThrottlePercentage = (_owner.Velocity.ThrottlePercentage + 0.01).Box(0.5, 1);
                }
                else if (speedAdjust < 0.5)
                {
                    _owner.Velocity.ThrottlePercentage = (_owner.Velocity.ThrottlePercentage - 0.01).Box(0.5, 1);
                }

                bool transitionToObservationObject = decisions.Get(Outputs.TransitionToObservationObject) > 0.9;
                bool transitionFromObservationObject = decisions.Get(Outputs.TransitionFromObservationObject) > 0.9;

                if (transitionToObservationObject && transitionFromObservationObject)
                {
                    CurrentAction = ActionState.None;
                }
                else if (transitionToObservationObject)
                {
                    CurrentAction = ActionState.MovingToApproach;
                }
                else if (transitionFromObservationObject)
                {
                    CurrentAction = ActionState.MovingToDepart;
                }
                else
                {
                    CurrentAction = ActionState.None;
                }

                LastDecisionTime = now;
            }

            if (CurrentAction == ActionState.MovingToApproach)
            {
                var deltaAngle = _owner.DeltaAngle(_observedObject);

                if (deltaAngle > 10)
                {
                    if (deltaAngle >= 180.0)
                    {
                        _owner.Velocity.Angle += 1;
                    }
                    else if (deltaAngle < 180.0)
                    {
                        _owner.Velocity.Angle -= 1;
                    }
                }
                else
                {
                    CurrentAction = ActionState.Approaching;
                }
            }
            else if (CurrentAction == ActionState.MovingToDepart)
            {
                var distanceToObservedObject = _owner.DistanceTo(_observedObject);

                if (_owner.Velocity.Angle.Degrees <= 180.0)
                {
                    _owner.Velocity.Angle += 0.8;
                }
                else if (_owner.Velocity.Angle.Degrees > 180.0)
                {
                    _owner.Velocity.Angle -= 0.8;
                }

                if (distanceToObservedObject > DistanceToKeep)
                {
                    CurrentAction = ActionState.None;
                }
            }
        }

        private DniNamedInterfaceParameters GatherInputs()
        {
            var aiParams = new DniNamedInterfaceParameters();

            var distance = _owner.DistanceTo(_observedObject);
            var percentageOfCloseness = (distance / DistanceToKeep).Box(0, 1);

            aiParams.Set(Inputs.DistanceFromObservationObject, percentageOfCloseness);

            var deltaAngle = _owner.DeltaAngle(_observedObject);

            var angleToIn6thRadians = Angle<double>.DegreesToRadians(deltaAngle) / 6.0;

            aiParams.Set(Inputs.AngleToObservationObjectIn6thRadians,
                angleToIn6thRadians.SplitToNegative(Math.PI / 6));

            return aiParams;
        }
    }
}
