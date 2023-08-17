using Determinet;
using Determinet.Types;
using System.IO;

namespace AI2D.Actors.Enemies.AI
{
    /// <summary>
    /// This is a pre-trained bug brain with some basic intelligence on obstacle avoidance.
    /// </summary>
    public static class BrainBase
    {
        public enum AIBrainTypes
        {
            Logistics,
            WeaponSystems
        }

        public static class AIInputs
        {
            public const string In0Degrees = "In0Degrees";
            public const string In45Degrees = "In45Degrees";
            public const string In90Degrees = "In90Degrees";
            public const string In270Degrees = "In270Degrees";
            public const string In315Degrees = "In315Degrees";
        }

        public static class AIOutputs
        {
            public const string OutChangeDirection = "OutChangeDirection";
            public const string OutRotateDirection = "OutRotateDirection";
            public const string OutRotationAmount = "OutRotationAmount";
            public const string OutChangeSpeed = "OutChangeSpeed";
            public const string OutChangeSpeedAmount = "OutChangeSpeedAmount";
        }

        private static DniNeuralNetwork _trainedLogisticsBrain = null;

        public static DniNeuralNetwork GetLogisticsNoColisionBrain(string initialBrainFile = null)
        {
            if (_trainedLogisticsBrain != null)
            {
                return _trainedLogisticsBrain.Clone();
            }

            if (string.IsNullOrEmpty(initialBrainFile) == false && File.Exists(initialBrainFile))
            {
                _trainedLogisticsBrain = DniNeuralNetwork.Load(initialBrainFile);
                if (_trainedLogisticsBrain != null)
                {
                    return _trainedLogisticsBrain;
                }
            }

            if (_trainedLogisticsBrain == null)
            {
                _trainedLogisticsBrain = new DniNeuralNetwork()
                {
                    LearningRate = 0.01
                };
            }

            //Vision inputs.
            _trainedLogisticsBrain.Layers.AddInput(ActivationType.LeakyReLU,
                new string[] {
                        AIInputs.In0Degrees,
                        AIInputs.In45Degrees,
                        AIInputs.In90Degrees,
                        AIInputs.In270Degrees,
                        AIInputs.In315Degrees
                });

            //Where the magic happens.
            _trainedLogisticsBrain.Layers.AddIntermediate(ActivationType.Sigmoid, 8);

            //Decision outputs
            _trainedLogisticsBrain.Layers.AddOutput(
                new string[] {
                        AIOutputs.OutChangeDirection,
                        AIOutputs.OutRotateDirection,
                        AIOutputs.OutRotationAmount,
                        AIOutputs.OutChangeSpeed,
                        AIOutputs.OutChangeSpeedAmount
                });

            for (int i = 0; i < 5000; i++)
            {
                //Left side detection, go right.
                _trainedLogisticsBrain.BackPropagate(TrainingScenerio(0, 0, 0, 2, 0), TrainingDecision(2, 2, 2, 2, 0));
                _trainedLogisticsBrain.BackPropagate(TrainingScenerio(0, 0, 0, 0, 2), TrainingDecision(2, 2, 2, 2, 0));
                _trainedLogisticsBrain.BackPropagate(TrainingScenerio(0, 0, 0, 2, 2), TrainingDecision(2, 2, 2, 2, 0));

                //Right side detection, go left.
                _trainedLogisticsBrain.BackPropagate(TrainingScenerio(0, 0, 2, 0, 0), TrainingDecision(2, 0, 2, 2, 0));
                _trainedLogisticsBrain.BackPropagate(TrainingScenerio(0, 2, 0, 0, 0), TrainingDecision(2, 0, 2, 2, 0));
                _trainedLogisticsBrain.BackPropagate(TrainingScenerio(0, 2, 2, 0, 0), TrainingDecision(2, 0, 2, 2, 0));

                //Front side detection, so left or right.
                _trainedLogisticsBrain.BackPropagate(TrainingScenerio(2, 0, 0, 0, 0), TrainingDecision(2, 0, 2, 2, 0));
                _trainedLogisticsBrain.BackPropagate(TrainingScenerio(2, 2, 0, 0, 2), TrainingDecision(2, 2, 2, 2, 0));
                _trainedLogisticsBrain.BackPropagate(TrainingScenerio(2, 2, 2, 2, 2), TrainingDecision(2, 2, 2, 2, 0));

                //No objects dection, speed up and cruise.
                _trainedLogisticsBrain.BackPropagate(TrainingScenerio(0, 0, 0, 0, 0), TrainingDecision(0.4f, 0.4f, 0.4f, 0.9f, 0.9f));
            }

            //_trainedLogisticsBrain.Save(fileName);

            return _trainedLogisticsBrain.Clone();
        }

        private static DniNamedInterfaceParameters TrainingScenerio(double in0Degrees, double in45Degrees, double in90Degrees, double in270Degrees, double in315Degrees)
        {
            var param = new DniNamedInterfaceParameters();
            param.Set(AIInputs.In0Degrees, in0Degrees);
            param.Set(AIInputs.In45Degrees, in45Degrees);
            param.Set(AIInputs.In90Degrees, in90Degrees);
            param.Set(AIInputs.In270Degrees, in270Degrees);
            param.Set(AIInputs.In315Degrees, in315Degrees);
            return param;
        }

        private static DniNamedInterfaceParameters TrainingDecision(double outChangeDirection, double outRotateDirection, double outRotationAmount, double outChangeSpeed, double outChangeSpeedAmount)
        {
            var param = new DniNamedInterfaceParameters();
            param.Set(AIOutputs.OutChangeDirection, outChangeDirection);
            param.Set(AIOutputs.OutRotateDirection, outRotateDirection);
            param.Set(AIOutputs.OutRotationAmount, outRotationAmount);
            param.Set(AIOutputs.OutChangeSpeed, outChangeSpeed);
            param.Set(AIOutputs.OutChangeSpeedAmount, outChangeSpeedAmount);
            return param;
        }
    }
}
