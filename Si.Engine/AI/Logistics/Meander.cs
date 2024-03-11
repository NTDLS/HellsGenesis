using NTDLS.Determinet;
using NTDLS.Determinet.Types;
using Si.Engine.AI.Logistics._Superclass;
using Si.Engine.Sprite._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;
using System;
using static Si.Library.SiConstants;

namespace Si.Engine.AI.Logistics
{
    /// <summary>
    /// AI brain to keep an object close to another object, but at a generally safe distance.
    /// </summary>
    internal class Meander : IIAController
    {
        private const string _assetPath = @"Data\AI\Logistics\FlyBy.txt";

        private readonly EngineCore _engine;
        private readonly SpriteShipBase _owner;
        private readonly SpriteBase _observedObject;

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

        public float DistanceToKeep { get; set; } = 500;
        public DateTime? LastDecisionTime { get; set; } = DateTime.Now.AddHours(-1);
        public int MillisecondsBetweenDecisions { get; set; } = 50;
        public SiRelativeDirection FavorateDirection = SiRelativeDirection.None;

        #endregion

        public DniNeuralNetwork Network { get; set; }

        private static DniNeuralNetwork _singletonNetwork = null;

        /// <summary>
        /// Creates a new instance of the intelligence object.
        /// </summary>
        /// <param name="core">Engine core instance.</param>
        /// <param name="owner">The object which is intelligent.</param>
        /// <param name="observedObject">The object for which the intelligent object will be observing for inputs.</param>
        public Meander(EngineCore engine, SpriteShipBase owner, SpriteBase observedObject)
        {
            _engine = engine;
            _owner = owner;
            _observedObject = observedObject;
            FavorateDirection = SiRandom.FlipCoin() ? SiRelativeDirection.Left : SiRelativeDirection.Right;

            if (_singletonNetwork != null)
            {
                Network = _singletonNetwork.Clone();//.Mutate(0.2, 0.1);
                return;
            }

            var networkJson = _engine.Assets.GetText(_assetPath);
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
                newNetwork.BackPropagate(TrainingScenerio(0, 0.5f), TrainingDecision(0, 1, 1));
                newNetwork.BackPropagate(TrainingScenerio(0, -0.5f), TrainingDecision(0, 1, 1));

                //Pretty close to observed object, get away.
                newNetwork.BackPropagate(TrainingScenerio(0.25f, 0), TrainingDecision(0, 1, 0.5f));
                newNetwork.BackPropagate(TrainingScenerio(0.25f, -1), TrainingDecision(0, 1, 0.5f));
                newNetwork.BackPropagate(TrainingScenerio(0.25f, 1), TrainingDecision(0, 1, 0.5f));
                newNetwork.BackPropagate(TrainingScenerio(0.25f, 0.5f), TrainingDecision(0, 1, 0.5f));
                newNetwork.BackPropagate(TrainingScenerio(0.25f, -0.5f), TrainingDecision(0, 1, 0.5f));

                //Very far from observed object, get closer.
                newNetwork.BackPropagate(TrainingScenerio(1, 0), TrainingDecision(1, 0, 1));
                newNetwork.BackPropagate(TrainingScenerio(1, -1), TrainingDecision(1, 0, 1));
                newNetwork.BackPropagate(TrainingScenerio(1, 1), TrainingDecision(1, 0, 1));
                newNetwork.BackPropagate(TrainingScenerio(1, 0.5f), TrainingDecision(1, 0, 1));
                newNetwork.BackPropagate(TrainingScenerio(1, -0.5f), TrainingDecision(1, 0, 1));

                //Pretty far from observed object, get closer.
                newNetwork.BackPropagate(TrainingScenerio(0.75f, 0), TrainingDecision(1, 0, 0.5f));
                newNetwork.BackPropagate(TrainingScenerio(0.75f, -1), TrainingDecision(1, 0, 0.5f));
                newNetwork.BackPropagate(TrainingScenerio(0.75f, 1), TrainingDecision(1, 0, 0.5f));
                newNetwork.BackPropagate(TrainingScenerio(0.75f, 0.5f), TrainingDecision(1, 0, 0.5f));
                newNetwork.BackPropagate(TrainingScenerio(0.75f, -0.5f), TrainingDecision(1, 0, 0.5f));
            }

            static DniNamedInterfaceParameters TrainingScenerio(float distanceFromObservationObject, float angleToObservationObjectIn10thRadians)
            {
                var param = new DniNamedInterfaceParameters();
                param.Set(Inputs.DistanceFromObservationObject, distanceFromObservationObject);
                param.Set(Inputs.AngleToObservationObjectIn6thRadians, angleToObservationObjectIn10thRadians);
                return param;
            }

            static DniNamedInterfaceParameters TrainingDecision(float transitionToObservationObject, float transitionFromObservationObject, float speedAdjust)
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

        public void ApplyIntelligence(float epoch, SiPoint displacementVector)
        {
            var now = DateTime.UtcNow;

            var elapsedTimeSinceLastDecision = (now - (DateTime)LastDecisionTime).TotalMilliseconds;

            if (elapsedTimeSinceLastDecision >= MillisecondsBetweenDecisions)
            {
                if (elapsedTimeSinceLastDecision > 1000)
                {
                    FavorateDirection = SiRandom.FlipCoin() ? SiRelativeDirection.Left : SiRelativeDirection.Right;
                }

                var decidingFactors = GatherInputs();
                var decisions = Network.FeedForward(decidingFactors);

                var speedAdjust = (float)decisions.Get(Outputs.SpeedAdjust); //Update nuget to make these floats.

                _owner.Velocity.ForwardVelocity += (speedAdjust / 5.0f);

                bool transitionToObservationObject = decisions.Get(Outputs.TransitionToObservationObject) > 0.9;
                bool transitionFromObservationObject = decisions.Get(Outputs.TransitionFromObservationObject) > 0.9;

                if (transitionToObservationObject && transitionFromObservationObject)
                {
                }
                else if (transitionToObservationObject)
                {
                    _owner.Rotate((45 * 0.05f) * (FavorateDirection == SiRelativeDirection.Left ? 1 : -1));
                }
                else if (transitionFromObservationObject)
                {
                    _owner.Rotate((-45 * 0.05f) * (FavorateDirection == SiRelativeDirection.Left ? 1 : -1));
                }

                LastDecisionTime = now;
            }
        }

        private DniNamedInterfaceParameters GatherInputs()
        {
            var aiParams = new DniNamedInterfaceParameters();

            var distance = _owner.DistanceTo(_observedObject);
            var percentageOfCloseness = ((100.0f - ((distance / DistanceToKeep) * 100.0f)) / 100.0f).Clamp(0, 1);

            aiParams.Set(Inputs.DistanceFromObservationObject, percentageOfCloseness);

            var deltaAngle = _owner.DeltaAngleDegrees(_observedObject);

            var angleToIn6thRadians = SiAngle.DegreesToRadians(deltaAngle) / 6.0f;

            aiParams.Set(Inputs.AngleToObservationObjectIn6thRadians,
                angleToIn6thRadians.SplitToSigned((float)(Math.PI / 6)));

            return aiParams;
        }
    }
}
