using NTDLS.Determinet;
using Si.Game.Engine.Types.Geometry;

namespace Si.Game.AI
{
    internal interface IAIController
    {
        public DniNeuralNetwork Network { get; set; }
        public void ApplyIntelligence(SiPoint displacementVector);
    }
}
