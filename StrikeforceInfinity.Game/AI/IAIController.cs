using NTDLS.Determinet;
using StrikeforceInfinity.Game.Engine.Types.Geometry;

namespace StrikeforceInfinity.Game.AI
{
    internal interface IAIController
    {
        public DniNeuralNetwork Network { get; set; }
        public void ApplyIntelligence(SiPoint displacementVector);
    }
}
