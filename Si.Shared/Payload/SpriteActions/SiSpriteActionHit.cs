namespace Si.Shared.Payload.SpriteActions
{
    public class SiSpriteActionHit : SiSpriteAction
    {
        public int Damage { get; set; }

        public SiSpriteActionHit(Guid multiplayUID)
            : base(multiplayUID)
        {
        }
    }
}
