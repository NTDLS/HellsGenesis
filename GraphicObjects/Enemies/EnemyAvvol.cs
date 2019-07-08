using AI2D.Engine;
using System.Linq;

namespace AI2D.GraphicObjects.Enemies
{
    public class EnemyAvvol : BaseEnemy
    {
        private const string _assetPath = @"..\..\Assets\Graphics\Enemy\";
        private readonly string[] _imagePaths = {
            #region images.
            "Avvol (1).png",
            "Avvol (2).png",
            "Avvol (3).png",
            "Avvol (4).png",
            "Avvol (5).png",
            "Avvol (6).png",
            "Avvol (7).png",
            "Avvol (8).png"
            #endregion
        };

        public EnemyAvvol(Core core)
            : base(core)
        {
            int imageIndex = Utility.Random.Next(0, 1000) % _imagePaths.Count();

            HitPoints = Utility.Random.Next(Consants.Limits.MinEnemyHealth, Consants.Limits.MaxEnemyHealth);

            LoadResources(_assetPath +_imagePaths[imageIndex], new System.Drawing.Size(32, 32));
        }

        public override void ApplyIntelligence()
        {
            MoveInDirectionOf(_core.Actors.Player);

            //If we are close to the player.
            double distanceToPlayer = Utility.DistanceTo(this, _core.Actors.Player);
            if (distanceToPlayer > 400)
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
