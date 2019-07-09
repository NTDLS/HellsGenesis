using AI2D.Engine;
using System.Drawing;
using System.Linq;

namespace AI2D.GraphicObjects.Enemies
{
    public class EnemyScinzad : BaseEnemy
    {
        private const string _assetPath = @"..\..\Assets\Graphics\Enemy\";
        private readonly string[] _imagePaths = {
            #region images.
            "Scinzad (1).png",
            "Scinzad (2).png",
            "Scinzad (3).png",
            "Scinzad (4).png",
            "Scinzad (5).png",
            "Scinzad (6).png",
            "Scinzad (7).png",
            "Scinzad (8).png"
            #endregion
        };

        public EnemyScinzad(Core core)
            : base(core)
        {
            int imageIndex = Utility.Random.Next(0, 1000) % _imagePaths.Count();

            HitPoints = Utility.Random.Next(Consants.Limits.MinEnemyHealth, Consants.Limits.MaxEnemyHealth);

            SetImage(_assetPath + _imagePaths[imageIndex], new Size(32, 32));
        }

        public override void ApplyIntelligence()
        {
            //If we are close to the player.
            double distanceToPlayer = Utility.DistanceTo(this, _core.Actors.Player);
            if (distanceToPlayer < 400)
            {
                //If we are pointing at the player.
                bool isPointingAtPlayer = IsPointingAt(_core.Actors.Player, 8.0);
                if (isPointingAtPlayer)
                {
                    if (CurrentWeapon?.RoundQuantity == 0)
                    {
                        SelectFirstAvailableUsableWeapon();
                    }

                    CurrentWeapon?.Fire();
                }
            }

            //If the enemy is off the screen, point at the player and come back into view.
            if (X < (0 - (Size.Width + 40)) || Y < (0 - (Size.Height + 40))
                || X >= (_core.Display.VisibleSize.Width + Size.Width) + 40
                || Y >= (_core.Display.VisibleSize.Height + Size.Height) + 40)
            {
                MoveInDirectionOf(_core.Actors.Player);
            }
        }
    }
}
