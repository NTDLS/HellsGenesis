using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Sprites.Player.BaseClasses;
using System.Drawing;

namespace StrikeforceInfinity.Game.Sprites.Player
{
    internal class SpriteDreadnaughtPlayerDrone : SpritePlayerDroneBase
    {
        public SpriteDreadnaughtPlayerDrone(EngineCore gameCore)
            : base(gameCore)
        {
            ShipClass = HgPlayerClass.Dreadnaught;

            string imagePath = @$"Graphics\Player\Ships\{ShipClass}.png";
            Initialize(imagePath, new Size(32, 32));
        }
    }
}
