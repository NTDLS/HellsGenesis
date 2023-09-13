using HG.Actors.Enemies.BaseClasses;
using HG.Actors.Weapons;
using HG.AI.Logistics;
using HG.Engine;
using HG.Types.Geometry;
using HG.Utility;
using System;
using System.Drawing;
using System.IO;

namespace HG.Actors.Enemies.Peons
{
    internal class EnemyPhoenix : EnemyPeonBase
    {
        public const int bountyMultiplier = 15;
        private const string _assetPath = @"Graphics\Enemy\Phoenix\";
        private readonly int imageCount = 6;
        private readonly int selectedImageIndex = 0;

        public EnemyPhoenix(Core core)
            : base(core, GetGenericHP(core), bountyMultiplier)
        {
            selectedImageIndex = HgRandom.Random.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"), new Size(32, 32));

            SetHullHealth(HgRandom.Random.Next(Settings.MinEnemyHealth, Settings.MaxEnemyHealth));

            Velocity.MaxBoost = 1.5;
            Velocity.MaxSpeed = HgRandom.Random.Next(Settings.MaxSpeed - 4, Settings.MaxSpeed - 3);

            SetPrimaryWeapon<WeaponScattershot>(1000);
            AddSecondaryWeapon<WeaponDualVulcanCannon>(500);

            AddAIController(new HostileEngagement(_core, this, _core.Player.Actor));
            AddAIController(new Taunt(_core, this, _core.Player.Actor));
            AddAIController(new Meander(_core, this, _core.Player.Actor));

            //if (HgRandom.FlipCoin())
            //{
            SetDefaultAIController(AIControllers[typeof(Taunt)]);
            //}
            //else
            //{
            //    SetDefaultAIController(AIControllers[typeof(Meander)]);
            //}

            behaviorChangeThresholdMiliseconds = HgRandom.RandomNumber(2000, 10000);

            SetDefaultAIController(AIControllers[typeof(Taunt)]);
        }

        #region Artificial Intelligence.

        DateTime lastBehaviorChangeTime = DateTime.Now;
        double behaviorChangeThresholdMiliseconds = 0;

        public override void ApplyIntelligence(HgPoint displacementVector)
        {
            base.ApplyIntelligence(displacementVector);

            if ((DateTime.Now - lastBehaviorChangeTime).TotalMilliseconds > behaviorChangeThresholdMiliseconds)
            {
                behaviorChangeThresholdMiliseconds = HgRandom.RandomNumber(2000, 10000);

                /*
                if (HgRandom.ChanceIn(2))
                {
                    SetDefaultAIController(AIControllers[typeof(HostileEngagement)]);
                }
                if (HgRandom.ChanceIn(2))
                {
                */
                SetDefaultAIController(AIControllers[typeof(Taunt)]);
                /*
                }
                else if (HgRandom.ChanceIn(2))
                {
                    SetDefaultAIController(AIControllers[typeof(Meander)]);
                }
                */
            }

            if (IsHostile)
            {
                double distanceToPlayer = DistanceTo(_core.Player.Actor);

                if (distanceToPlayer < 800)
                {
                    if (distanceToPlayer > 400 && HasSelectedSecondaryWeaponAndAmmo())
                    {
                        bool isPointingAtPlayer = IsPointingAt(_core.Player.Actor, 8.0);
                        if (isPointingAtPlayer)
                        {
                            SelectedSecondaryWeapon?.Fire();
                        }
                    }
                    else if (distanceToPlayer > 0 && HasSelectedPrimaryWeaponAndAmmo())
                    {
                        bool isPointingAtPlayer = IsPointingAt(_core.Player.Actor, 15.0);
                        if (isPointingAtPlayer)
                        {
                            PrimaryWeapon?.Fire();
                        }
                    }
                }
            }

            DefaultAIController?.ApplyIntelligence(displacementVector);
        }

        #endregion
    }
}
