using Si.Library.Mathematics;

namespace Si.GameEngine.AI._Superclass
{
    /// <summary>
    /// A sprite that is controlled by an AI (either a state machine (IIAStateMachine) or later a more advanced AI).
    /// </summary>
    public interface IAIController
    {
        public void ApplyIntelligence(float epoch, SiVector displacementVector);
    }
}
