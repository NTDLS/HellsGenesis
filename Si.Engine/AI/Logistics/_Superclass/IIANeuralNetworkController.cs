using NTDLS.Determinet;

namespace Si.Engine.AI.Logistics._Superclass
{
    public interface IIANeuralNetworkController : IIAController
    {
        public DniNeuralNetwork Network { get; set; }
    }
}
