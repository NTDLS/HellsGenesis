namespace Si.Shared.Payload.DroneActions
{
    public class SiSpriteAction
    {
        public Guid MultiplayUID { get; set; }
        public DateTime Timestamp { get; set; }

        public SiSpriteAction(Guid multiplayUID)
        {
            MultiplayUID = multiplayUID;
        }

        public SiSpriteAction()
        {
        }
    }
}
