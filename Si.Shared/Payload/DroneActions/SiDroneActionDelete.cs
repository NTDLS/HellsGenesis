namespace Si.Shared.Payload.DroneActions
{
    /// <summary>
    /// Drone needs to be deleted, not exploded.
    /// </summary>
    public class SiDroneActionDelete : SiDroneAction
    {
        public SiDroneActionDelete(Guid playerMultiplayUID)
            : base(playerMultiplayUID)
        {
        }
    }
}
