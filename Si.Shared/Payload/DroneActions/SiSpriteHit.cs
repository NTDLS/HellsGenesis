namespace Si.Shared.Payload.DroneActions
{
    public class SiSpriteHit : SiSpriteAction
    {
        public int Damage { get; set; }

        public SiSpriteHit(Guid multiplayUID)
            : base(multiplayUID)
        {
        }

    }
}
