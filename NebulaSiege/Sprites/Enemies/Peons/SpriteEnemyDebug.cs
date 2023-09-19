using NebulaSiege.AI.Logistics;
using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Utility;
using NebulaSiege.Weapons;
using System;
using System.Drawing;
using System.IO;

namespace NebulaSiege.Sprites.Enemies.Peons
{
    /// <summary>
    /// Debugging enemy uint.
    /// </summary>
    internal class SpriteEnemyDebug : _SpriteEnemyPeonBase
    {
        public const int hullHealth = 10;
        public const int bountyMultiplier = 15;

        private const string _assetPath = @"Graphics\Enemy\Debug\";
        private readonly int imageCount = 1;
        private readonly int selectedImageIndex = 0;

        public SpriteEnemyDebug(EngineCore core)
            : base(core, hullHealth, bountyMultiplier)
        {
            selectedImageIndex = HgRandom.Generator.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"), new Size(32, 32));

            Velocity.MaxBoost = 1.5;
            Velocity.MaxSpeed = HgRandom.Generator.Next(_core.Settings.MaxEnemySpeed - 4, _core.Settings.MaxEnemySpeed - 3);

            SetPrimaryWeapon<WeaponVulcanCannon>(1000);
            AddSecondaryWeapon<WeaponDualVulcanCannon>(500);

            AddAIController(new HostileEngagement(_core, this, _core.Player.Sprite));
            AddAIController(new Taunt(_core, this, _core.Player.Sprite));
            AddAIController(new Meander(_core, this, _core.Player.Sprite));

            //if (HgRandom.FlipCoin())
            //{
            SetDefaultAIController(AIControllers[typeof(Taunt)]);
            //}
            //else
            //{
            //    SetDefaultAIController(AIControllers[typeof(Meander)]);
            //}

            behaviorChangeThresholdMiliseconds = HgRandom.Between(2000, 10000);

            SetDefaultAIController(AIControllers[typeof(Taunt)]);
        }

        #region Artificial Intelligence.

        DateTime lastBehaviorChangeTime = DateTime.Now;
        double behaviorChangeThresholdMiliseconds = 0;

        public override void ApplyIntelligence(NsPoint displacementVector)
        {
            base.ApplyIntelligence(displacementVector);

            if ((DateTime.Now - lastBehaviorChangeTime).TotalMilliseconds > behaviorChangeThresholdMiliseconds)
            {
                behaviorChangeThresholdMiliseconds = HgRandom.Between(2000, 10000);

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
                double distanceToPlayer = DistanceTo(_core.Player.Sprite);

                if (distanceToPlayer < 800)
                {
                    if (distanceToPlayer > 400 && HasSelectedSecondaryWeaponAndAmmo())
                    {
                        bool isPointingAtPlayer = IsPointingAt(_core.Player.Sprite, 8.0);
                        if (isPointingAtPlayer)
                        {
                            SelectedSecondaryWeapon?.Fire();
                        }
                    }
                    else if (distanceToPlayer > 0 && HasSelectedPrimaryWeaponAndAmmo())
                    {
                        bool isPointingAtPlayer = IsPointingAt(_core.Player.Sprite, 15.0);
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
