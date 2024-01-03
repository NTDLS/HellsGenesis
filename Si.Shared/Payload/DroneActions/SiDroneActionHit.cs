namespace Si.Shared.Payload.DroneActions
{
    public class SiDroneActionHit : SiDroneAction
    {
        public int Damage { get; set; }

        public SiDroneActionHit(Guid multiplayUID)
            : base(multiplayUID)
        {
        }
    }
}
