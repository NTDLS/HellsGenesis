using Si.Engine.Sprite._Superclass;
using Si.GameEngine.AI._Superclass;

namespace Si.Engine.AI.Logistics
{
    /// <summary>
    /// AI brain to keep an object close to another object, but at a generally safe distance.
    /// </summary>
    internal class AIModelMeander : AIStateMachine
    {
        public AIModelMeander(EngineCore engine, SpriteShipBase owner, SpriteBase observedObject)
            : base(engine, owner, observedObject)
        {
        }

        /*
        private const string _assetPath = @"Data\AI\Logistics\FlyBy.txt";

        private readonly EngineCore _engine;
        private readonly SpriteShipBase _owner;
        private readonly SpriteBase _observedObject;

        #region Instance parameters.

        public float DistanceToKeep { get; set; } = 500;
        public DateTime? LastDecisionTime { get; set; } = DateTime.Now.AddHours(-1);
        public int MillisecondsBetweenDecisions { get; set; } = 50;
        public SiRelativeDirection FavorateDirection = SiRelativeDirection.None;

        #endregion

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
        */
    }
}
