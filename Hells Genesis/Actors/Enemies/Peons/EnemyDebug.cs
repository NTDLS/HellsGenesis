using HG.Actors.Weapons;
using HG.AI.Logistics;
using HG.Engine;
using HG.Types;
using System;
using System.Drawing;
using System.IO;

namespace HG.Actors.Enemies.Peons
{
    /// <summary>
    /// Debugging enemy uint.
    /// </summary>
    internal class EnemyDebug : EnemyPeonBase
    {
        public const int ScoreMultiplier = 15;
        private const string _assetPath = @"..\..\..\Assets\Graphics\Enemy\Debug\";
        private readonly int imageCount = 6;
        private readonly int selectedImageIndex = 0;

        public EnemyDebug(Core core)
            : base(core, GetGenericHP(core), ScoreMultiplier)
        {
            selectedImageIndex = HgRandom.Random.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"), new Size(32, 32));

            SetHitPoints(HgRandom.Random.Next(_core.Settings.MinEnemyHealth, _core.Settings.MaxEnemyHealth));

            Velocity.MaxBoost = 1.5;
            Velocity.MaxSpeed = HgRandom.Random.Next(_core.Settings.MaxSpeed - 4, _core.Settings.MaxSpeed - 3);

            AddSecondaryWeapon(new WeaponVulcanCannon(_core)
            {
                RoundQuantity = 1000,
                FireDelayMilliseconds = 250
            });

            AddSecondaryWeapon(new WeaponDualVulcanCannon(_core)
            {
                RoundQuantity = 500,
                FireDelayMilliseconds = 500
            });

            SelectSecondaryWeapon(typeof(WeaponVulcanCannon));

            AddAIController(new HostileEngagement(_core, this, _core.Player.Actor));
            AddAIController(new Taunt(_core, this, _core.Player.Actor));
            AddAIController(new Meander(_core, this, _core.Player.Actor));

            if (HgRandom.FlipCoin())
            {
                SetDefaultAIController(AIControllers[typeof(Taunt)]);
            }
            else
            {
                SetDefaultAIController(AIControllers[typeof(Meander)]);
            }

            behaviorChangeThresholdMiliseconds = HgRandom.RandomNumber(2000, 10000);

            SetDefaultAIController(AIControllers[typeof(Taunt)]);
        }

        #region Artificial Intelligence.

        DateTime lastBehaviorChangeTime = DateTime.Now;
        double behaviorChangeThresholdMiliseconds = 0;

        public override void ApplyIntelligence(HgPoint<double> displacementVector)
        {
            base.ApplyIntelligence(displacementVector);

            if ((DateTime.Now - lastBehaviorChangeTime).TotalMilliseconds > behaviorChangeThresholdMiliseconds)
            {
                behaviorChangeThresholdMiliseconds = HgRandom.RandomNumber(2000, 10000);

                if (HgRandom.ChanceIn(2))
                {
                    SetDefaultAIController(AIControllers[typeof(HostileEngagement)]);
                }
                if (HgRandom.ChanceIn(2))
                {
                    SetDefaultAIController(AIControllers[typeof(Taunt)]);
                }
                else if (HgRandom.ChanceIn(2))
                {
                    SetDefaultAIController(AIControllers[typeof(Meander)]);
                }
            }

            double distanceToPlayer = DistanceTo(_core.Player.Actor);


            if (distanceToPlayer < 700)
            {
                if (distanceToPlayer > 200 && HasSecondaryWeaponAndAmmo(typeof(WeaponVulcanCannon)))
                {
                    bool isPointingAtPlayer = IsPointingAt(_core.Player.Actor, 8.0);
                    if (isPointingAtPlayer)
                    {
                        SelectSecondaryWeapon(typeof(WeaponVulcanCannon));
                        SelectedSecondaryWeapon?.Fire();
                    }
                }
                else if (distanceToPlayer > 0 && HasSecondaryWeaponAndAmmo(typeof(WeaponDualVulcanCannon)))
                {
                    bool isPointingAtPlayer = IsPointingAt(_core.Player.Actor, 8.0);
                    if (isPointingAtPlayer)
                    {
                        SelectSecondaryWeapon(typeof(WeaponDualVulcanCannon));
                        SelectedSecondaryWeapon?.Fire();
                    }
                }
            }




            DefaultAIController?.ApplyIntelligence(displacementVector);
        }

        #endregion
    }
}
