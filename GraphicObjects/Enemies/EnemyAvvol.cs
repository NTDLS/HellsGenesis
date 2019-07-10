using AI2D.Engine;
using AI2D.Types;
using AI2D.Weapons;
using System.Drawing;
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

            HitPoints = Utility.Random.Next(Constants.Limits.MinEnemyHealth, Constants.Limits.MaxEnemyHealth);

            SetImage(_assetPath + _imagePaths[imageIndex], new Size(32,32));
        }

        public override void ApplyIntelligence(PointD frameAppliedOffset)
        {
            base.ApplyIntelligence(frameAppliedOffset);

            MoveInDirectionOf(_core.Actors.Player);

            double distanceToPlayer = Utility.DistanceTo(this, _core.Actors.Player);
            if (distanceToPlayer > 500 && HasWeaponAndAmmo(typeof(WeaponGuidedFragMissile)))
            {
                bool isPointingAtPlayer = IsPointingAt(_core.Actors.Player, 8.0);
                if (isPointingAtPlayer)
                {
                    SelectWeapon(typeof(WeaponGuidedFragMissile));
                    CurrentWeapon?.Fire();
                }
            }
            else if (distanceToPlayer > 300 && HasWeaponAndAmmo(typeof(WeaponPhotonTorpedo)))
            {
                bool isPointingAtPlayer = IsPointingAt(_core.Actors.Player, 8.0);
                if (isPointingAtPlayer)
                {
                    SelectWeapon(typeof(WeaponPhotonTorpedo));
                    CurrentWeapon?.Fire();
                }
            }
            else if (distanceToPlayer > 200 && HasWeaponAndAmmo(typeof(WeaponVulcanCannon)))
            {
                bool isPointingAtPlayer = IsPointingAt(_core.Actors.Player, 8.0);
                if (isPointingAtPlayer)
                {
                    SelectWeapon(typeof(WeaponVulcanCannon));
                    CurrentWeapon?.Fire();
                }
            }
            else if (distanceToPlayer > 100 && HasWeaponAndAmmo(typeof(WeaponDualVulcanCannon)))
            {
                bool isPointingAtPlayer = IsPointingAt(_core.Actors.Player, 8.0);
                if (isPointingAtPlayer)
                {
                    SelectWeapon(typeof(WeaponDualVulcanCannon));
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
