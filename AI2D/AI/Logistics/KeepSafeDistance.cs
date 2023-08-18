using AI2D.Actors;
using AI2D.Actors.Enemies;
using AI2D.Engine;
using AI2D.Types;
using Determinet;
using Determinet.Types;
using System;
using System.IO;
using System.Linq;

namespace AI2D.AI.Logistics
{
    public class KeepSafeDistance : IIntelligenceObject
    {
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
            AngleFromObservationObjectIn6thRadians
        }

        public enum Outputs
        {
            TransitionToObservationObject,
            TransitionFromObservationObject,
            Cruise,
            IncreaseSpeed,
            DecreaseSpeed
        }

        #endregion

        #region Instance parameters.

        public double MaxObserveDistance { get; set; } = 500;
        public double VisionToleranceDegrees { get; set; } = 25;
        public DateTime? LastDecisionTime { get; set; }
        public int MillisecondsBetweenDecisions { get; set; } = 50;
        public float DecisionSensitivity { get; set; } = (float)Utility.RandomNumber(0.25, 0.55);

        #endregion

        public DniNeuralNetwork Network { get; set; }

        private static DniNeuralNetwork _singletonNetwork = null;

        /// <summary>
        /// Creates a new instance of the intelligence object.
        /// </summary>
        /// <param name="core">Engine core instance.</param>
        /// <param name="owner">The object which is intelligent.</param>
        /// <param name="observedObject">The object for which the intelligent object will be observing for inputs.</param>
        /// <param name="pretrainedModelFile">If there is a pre-trained model, this would be the file.</param>
        public KeepSafeDistance(Core core, ActorBase owner, ActorBase observedObject, string pretrainedModelFile = null)
        {
            _core = core;
            _owner = owner;
            _observedObject = observedObject;

            if (_singletonNetwork != null)
            {
                Network = _singletonNetwork.Clone();//.Mutate(0.2, 0.1)
                return;
            }

            if (string.IsNullOrEmpty(pretrainedModelFile) == false && File.Exists(pretrainedModelFile))
            {
                var loadedNetwork = DniNeuralNetwork.Load(pretrainedModelFile);
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
                        Inputs.AngleFromObservationObjectIn6thRadians
                });

            //Where the magic happens.
            newNetwork.Layers.AddIntermediate(ActivationType.Sigmoid, 8);

            //Decision outputs
            newNetwork.Layers.AddOutput(
                new object[] {
                        Outputs.TransitionToObservationObject,
                        Outputs.TransitionFromObservationObject,
                        Outputs.Cruise,
                        Outputs.IncreaseSpeed,
                        Outputs.DecreaseSpeed
                });

            for (int i = 0; i < 5000; i++)
            {
                newNetwork.BackPropagate(TrainingScenerio(0, 0), TrainingDecision(2, 2, 2, 2, 0));

                //No objects dection, speed up and cruise.
                //newNetwork.BackPropagate(TrainingScenerio(0, 0), TrainingDecision(0.4f, 0.4f, 0.4f, 0.9f, 0.9f));
            }

            static DniNamedInterfaceParameters TrainingScenerio(double distanceFromObservationObject, double angleToObservationObjectIn10thRadians)
            {
                var param = new DniNamedInterfaceParameters();
                param.Set(Inputs.DistanceFromObservationObject, distanceFromObservationObject);
                param.Set(Inputs.AngleFromObservationObjectIn6thRadians, angleToObservationObjectIn10thRadians);
                return param;
            }

            static DniNamedInterfaceParameters TrainingDecision(double transitionToObservationObject, double transitionFromObservationObject, double cruise, double increaseSpeed, double decreaseSpeed)
            {
                var param = new DniNamedInterfaceParameters();
                param.Set(Outputs.TransitionToObservationObject, transitionToObservationObject);
                param.Set(Outputs.TransitionFromObservationObject, transitionFromObservationObject);
                param.Set(Outputs.Cruise, cruise);
                param.Set(Outputs.IncreaseSpeed, increaseSpeed);
                param.Set(Outputs.DecreaseSpeed, decreaseSpeed);
                return param;
            }

            //newNetwork.Save(fileName);

            #endregion

            _singletonNetwork = newNetwork;
            Network = newNetwork.Clone();//.Mutate(0.2, 0.1)
        }

        public void ApplyIntelligence(Point<double> frameAppliedOffset)
        {
            var now = DateTime.UtcNow;

            if (LastDecisionTime == null || (now - (DateTime)LastDecisionTime).TotalMilliseconds >= MillisecondsBetweenDecisions)
            {
                var decidingFactors = GetVisionInputs();

                var decisions = Network.FeedForward(decidingFactors);

                /*
                if (decisions.Get(Outputs.ChangeDirection) >= DecisionSensitivity)
                {
                    var rotateAmount = decisions.Get(Outputs.RotationAmount);

                    if (decisions.Get(Outputs.RotateDirection) >= DecisionSensitivity)
                    {
                        _owner.Rotate(45 * rotateAmount);
                    }
                    else
                    {
                        _owner.Rotate(-45 * rotateAmount);
                    }
                }

                if (decisions.Get(Outputs.ChangeSpeed) >= DecisionSensitivity)
                {
                    var speedFactor = decisions.Get(Outputs.ChangeSpeedAmount, 0);
                    _owner.Velocity.ThrottlePercentage += (speedFactor / 5.0);
                }
                else
                {
                    var speedFactor = decisions.Get(Outputs.ChangeSpeedAmount, 0);
                    _owner.Velocity.ThrottlePercentage += -(speedFactor / 5.0);
                }

                if (_owner.Velocity.ThrottlePercentage < 0)
                {
                    _owner.Velocity.ThrottlePercentage = 0;
                }
                if (_owner.Velocity.ThrottlePercentage == 0)
                {
                    _owner.Velocity.ThrottlePercentage = 0.10;
                }*/

                LastDecisionTime = now;
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
                if (other == _owner)
                {
                    continue;
                }

                double distance = _owner.DistanceTo(other);
                double percentageOfCloseness = 1 - (distance / MaxObserveDistance);

                if (_owner.IsPointingAt(other, VisionToleranceDegrees, MaxObserveDistance, -90))
                {
                    aiParams.SetIfLess(Inputs.DistanceFromObservationObject, percentageOfCloseness);
                }

                if (_owner.IsPointingAt(other, VisionToleranceDegrees, MaxObserveDistance, -45))
                {
                    aiParams.SetIfLess(Inputs.AngleFromObservationObjectIn6thRadians, percentageOfCloseness);
                }
            }

            return aiParams;
        }
    }
}

