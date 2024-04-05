using Si.Engine;
using Si.Engine.Sprite._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.GameEngine.AI._Superclass
{
    /// <summary>
    /// A sprite that is controlled by an AI state-machine.
    /// </summary>
    public class AIStateMachine : IAIController
    {
        /// <summary>
        /// Reference to the engine core class.
        /// </summary>
        public EngineCore Engine { get; private set; }

        /// <summary>
        /// Reference to the sprite that is being controlled by this AI model.
        /// </summary>
        public SpriteShipBase Owner { get; private set; }

        /// <summary>
        /// Reference to the object that the sprite is observing.
        /// </summary>
        public SpriteBase ObservedObject { get; private set; }

        /// <summary>
        /// The current state that the AI is in.
        /// </summary>
        public AIState CurrentState { get; private set; }

        #region Events.

        /// <summary>
        /// Fired when the state is changed through a call to ChangeState().
        /// </summary>
        public event StateChanged OnStateChanged;
        public delegate void StateChanged(AIStateMachine sender);

        /// <summary>
        /// Fired when the engine wants the sprite to make a decision based on the current AI state.
        /// </summary>
        public event ApplyIntelligenceProc OnApplyIntelligence;
        public delegate void ApplyIntelligenceProc(float epoch, SiVector displacementVector, AIState state);

        #endregion

        /// <summary>
        /// A sprite that is controlled by an AI state-machine.
        /// </summary>
        /// <param name="engine">Reference to the engine core class.</param>
        /// <param name="owner">Reference to the sprite that is being controlled by this AI model.</param>
        /// <param name="observedObject">Reference to the object that the sprite is observing (probably the player, but can be other objects).</param>
        public AIStateMachine(EngineCore engine, SpriteShipBase owner, SpriteBase observedObject)
        {
            Engine = engine;
            Owner = owner;
            ObservedObject = observedObject;
        }

        void IAIController.ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            OnApplyIntelligence?.Invoke(epoch, displacementVector, CurrentState);
        }

        /// <summary>
        /// Sets a new AI state.
        /// </summary>
        /// <param name="state"></param>
        public void ChangeState(AIState state)
        {
            CurrentState = state;
            OnStateChanged?.Invoke(this);
        }
    }
}
