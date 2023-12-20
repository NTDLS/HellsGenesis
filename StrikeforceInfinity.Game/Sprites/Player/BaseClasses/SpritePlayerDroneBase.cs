using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;

namespace StrikeforceInfinity.Game.Sprites.Player.BaseClasses
{
    /// <summary>
    /// The player base is a sub-class of the ship base. This is used as the enemy drone of the player in multiplay.
    /// </summary>
    internal class SpritePlayerDroneBase : _SpriteShipBase
    {
        public HgPlayerClass ShipClass { get; set; }

        public SpritePlayerDroneBase(EngineCore gameCore)
            : base(gameCore)
        {
        }

        public virtual void ApplyIntelligence(SiPoint displacementVector)
        {
            //For the player class, this is only used for multiplay.
        }
    }
}
