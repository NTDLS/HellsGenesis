using HG.Actors.Weapons;
using HG.AI.Logistics;
using HG.Engine;
using HG.Types;
using System.Drawing;
using System.IO;

namespace HG.Actors.Enemies
{
    /// <summary>
    /// Debugging enemy uint.
    /// </summary>
    internal class EnemyDebug : EnemyBase
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
            Velocity.MaxSpeed = HgRandom.Random.Next(_core.Settings.MaxSpeed - 2, _core.Settings.MaxSpeed); //Upper end of the speed spectrum

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

            //AddAIController(new HostileEngagement(_core, this, _core.Player.Actor));
            AddAIController(new Taunt(_core, this, _core.Player.Actor));
            //AddAIController(new Meander(_core, this, _core.Player.Actor));

            SetCurrentAIController(AIControllers[typeof(Taunt)]);

        }

        #region Artificial Intelligence.

        public override void ApplyIntelligence(HgPoint<double> displacementVector)
        {
            base.ApplyIntelligence(displacementVector);

            if (CurrentAIController != null)
            {
                CurrentAIController.ApplyIntelligence(displacementVector);
            }
        }

        #endregion
    }
}
