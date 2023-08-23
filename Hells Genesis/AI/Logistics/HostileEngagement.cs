using HG.Actors.Objects;
using HG.Engine;
using HG.Types;
using Determinet;
using Determinet.Types;
using System;
using System.IO;

namespace HG.AI.Logistics
{
    internal class HostileEngagement : IAIController
    {
        private readonly Core _core;
        private readonly ActorBase _owner;
        private readonly ActorBase _observedObject;

        #region I/O Enumerations.

        public enum Inputs
        {
            At0Degrees,
            At45Degrees,
            At90Degrees,
            At270Degrees,
            At315Degrees
        }

        public enum Outputs
        {
            ChangeDirection,
            RotateDirection,
            RotationAmount,
            ChangeSpeed,
            ChangeSpeedAmount
        }

        #endregion

        #region Instance parameters.

        public double MaxObserveDistance { get; set; } = 500;
        public double VisionToleranceDegrees { get; set; } = 25;
        public DateTime? LastDecisionTime { get; set; }
        public int MillisecondsBetweenDecisions { get; set; } = 50;
        public float DecisionSensitivity { get; set; } = (float)HGRandom.RandomNumber(0.25, 0.55);

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
        public HostileEngagement(Core core, ActorBase owner, ActorBase observedObject, string pretrainedModelFile = null)
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
                        Inputs.At0Degrees,
                        Inputs.At45Degrees,
                        Inputs.At90Degrees,
                        Inputs.At270Degrees,
                        Inputs.At315Degrees
                });

            //Where the magic happens.
            newNetwork.Layers.AddIntermediate(ActivationType.Sigmoid, 8);

            //Decision outputs
            newNetwork.Layers.AddOutput(
                new object[] {
                        Outputs.ChangeDirection,
                        Outputs.RotateDirection,
                        Outputs.RotationAmount,
                        Outputs.ChangeSpeed,
                        Outputs.ChangeSpeedAmount
                });

            for (int i = 0; i < 5000; i++)
            {
                //Left side detection, go right.
                newNetwork.BackPropagate(TrainingScenerio(0, 0, 0, 2, 0), TrainingDecision(2, 2, 2, 2, 0));
                newNetwork.BackPropagate(TrainingScenerio(0, 0, 0, 0, 2), TrainingDecision(2, 2, 2, 2, 0));
                newNetwork.BackPropagate(TrainingScenerio(0, 0, 0, 2, 2), TrainingDecision(2, 2, 2, 2, 0));

                //Right side detection, go left.
                newNetwork.BackPropagate(TrainingScenerio(0, 0, 2, 0, 0), TrainingDecision(2, 0, 2, 2, 0));
                newNetwork.BackPropagate(TrainingScenerio(0, 2, 0, 0, 0), TrainingDecision(2, 0, 2, 2, 0));
                newNetwork.BackPropagate(TrainingScenerio(0, 2, 2, 0, 0), TrainingDecision(2, 0, 2, 2, 0));

                //Front side detection, so left or right.
                newNetwork.BackPropagate(TrainingScenerio(2, 0, 0, 0, 0), TrainingDecision(2, 0, 2, 2, 0));
                newNetwork.BackPropagate(TrainingScenerio(2, 2, 0, 0, 2), TrainingDecision(2, 2, 2, 2, 0));
                newNetwork.BackPropagate(TrainingScenerio(2, 2, 2, 2, 2), TrainingDecision(2, 2, 2, 2, 0));

                //No objects dection, speed up and cruise.
                newNetwork.BackPropagate(TrainingScenerio(0, 0, 0, 0, 0), TrainingDecision(0.4f, 0.4f, 0.4f, 0.9f, 0.9f));
            }

            static DniNamedInterfaceParameters TrainingScenerio(double at0Degrees, double at45Degrees, double at90Degrees, double at270Degrees, double at315Degrees)
            {
                var param = new DniNamedInterfaceParameters();
                param.Set(Inputs.At0Degrees, at0Degrees);
                param.Set(Inputs.At45Degrees, at45Degrees);
                param.Set(Inputs.At90Degrees, at90Degrees);
                param.Set(Inputs.At270Degrees, at270Degrees);
                param.Set(Inputs.At315Degrees, at315Degrees);
                return param;
            }

            static DniNamedInterfaceParameters TrainingDecision(double changeDirection, double rotateDirection, double rotationAmount, double changeSpeed, double changeSpeedAmount)
            {
                var param = new DniNamedInterfaceParameters();
                param.Set(Outputs.ChangeDirection, changeDirection);
                param.Set(Outputs.RotateDirection, rotateDirection);
                param.Set(Outputs.RotationAmount, rotationAmount);
                param.Set(Outputs.ChangeSpeed, changeSpeed);
                param.Set(Outputs.ChangeSpeedAmount, changeSpeedAmount);
                return param;
            }

            //newNetwork.Save(fileName);

            #endregion

            _singletonNetwork = newNetwork;
            Network = newNetwork.Clone();//.Mutate(0.2, 0.1)
        }

        public void ApplyIntelligence(HGPoint<double> frameAppliedOffset)
        {
            var now = DateTime.UtcNow;

            if (LastDecisionTime == null || (now - (DateTime)LastDecisionTime).TotalMilliseconds >= MillisecondsBetweenDecisions)
            {
                var decidingFactors = GatherInputs();

                var decisions = Network.FeedForward(decidingFactors);

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
                    _owner.Velocity.ThrottlePercentage += speedFactor / 5.0;
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
                }

                LastDecisionTime = now;
            }
        }

        private DniNamedInterfaceParameters GatherInputs()
        {
            var aiParams = new DniNamedInterfaceParameters();

            double distance = _owner.DistanceTo(_observedObject);
            double percentageOfCloseness = 1 - distance / MaxObserveDistance;

            if (_owner.IsPointingAt(_observedObject, VisionToleranceDegrees, MaxObserveDistance, -90))
            {
                aiParams.SetIfLess(Inputs.At270Degrees, percentageOfCloseness);
            }

            if (_owner.IsPointingAt(_observedObject, VisionToleranceDegrees, MaxObserveDistance, -45))
            {
                aiParams.SetIfLess(Inputs.At315Degrees, percentageOfCloseness);
            }

            if (_owner.IsPointingAt(_observedObject, VisionToleranceDegrees, MaxObserveDistance, 0))
            {
                aiParams.SetIfLess(Inputs.At0Degrees, percentageOfCloseness);
            }

            if (_owner.IsPointingAt(_observedObject, VisionToleranceDegrees, MaxObserveDistance, +45))
            {
                aiParams.SetIfLess(Inputs.At45Degrees, percentageOfCloseness);
            }

            if (_owner.IsPointingAt(_observedObject, VisionToleranceDegrees, MaxObserveDistance, +90))
            {
                aiParams.SetIfLess(Inputs.At90Degrees, percentageOfCloseness);
            }

            return aiParams;
        }
    }
}
