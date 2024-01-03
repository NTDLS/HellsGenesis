namespace Si.Shared.Payload.DroneActions
{
    public class SiDroneAction
    {
        public Guid MultiplayUID { get; set; }

        public SiDroneAction(Guid multiplayUID)
        {
            MultiplayUID = multiplayUID;
        }
    }
}
