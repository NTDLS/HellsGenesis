using NebulaSiege.Game.Engine.Types.Geometry;
using NTDLS.Determinet;

namespace NebulaSiege.Game.AI
{
    internal interface IAIController
    {
        public DniNeuralNetwork Network { get; set; }
        public void ApplyIntelligence(NsPoint displacementVector);
    }
}
