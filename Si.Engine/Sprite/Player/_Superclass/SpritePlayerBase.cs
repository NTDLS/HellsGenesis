using Si.Audio;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library;
using Si.Library.Mathematics;
using System.Linq;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.Player._Superclass
{
    /// <summary>
    /// The player base is a sub-class of the ship base. It is only used by the Player and as a model for menu selections.
    /// </summary>
    public class SpritePlayerBase : SpriteInteractiveBase
    {
        public readonly string BoostResourceName = "SpritePlayerBase:Boost";

        public SiAudioClip AmmoLowSound { get; private set; }
        public SiAudioClip AmmoEmptySound { get; private set; }
        public SiAudioClip ShipEngineRoarSound { get; private set; }
        public SiAudioClip ShipEngineIdleSound { get; private set; }
        public SiAudioClip AllSystemsGoSound { get; private set; }
        public SiAudioClip ShieldFailSound { get; private set; }
        public SiAudioClip ShieldDownSound { get; private set; }
        public SiAudioClip ShieldMaxSound { get; private set; }
        public SiAudioClip ShieldNominalSound { get; private set; }
        public SiAudioClip SystemsFailingSound { get; private set; }
        public SiAudioClip HullBreachedSound { get; private set; }
        public SiAudioClip IntegrityLowSound { get; private set; }
        public SiAudioClip ShipEngineBoostSound { get; private set; }
        public int MaxHullHealth { get; set; }
        public int MaxShieldPoints { get; set; }
        public SpriteAnimation ThrusterAnimation { get; private set; }
        public SpriteAnimation BoosterAnimation { get; private set; }
        public WeaponBase PrimaryWeapon { get; private set; }
        public WeaponBase SelectedSecondaryWeapon { get; private set; }

        public SpritePlayerBase(EngineCore engine, string imagePath)
            : base(engine, imagePath)
        {
            OnHit += SpritePlayer_OnHit;

            AmmoLowSound = _engine.Assets.GetAudio(@"Sounds\Ship\Ammo Low.wav", 0.75f);
            SystemsFailingSound = _engine.Assets.GetAudio(@"Sounds\Ship\Systems Failing.wav", 0.75f);
            HullBreachedSound = _engine.Assets.GetAudio(@"Sounds\Ship\Hull Breached.wav", 0.75f);
            IntegrityLowSound = _engine.Assets.GetAudio(@"Sounds\Ship\Integrity Low.wav", 0.75f);
            ShieldFailSound = _engine.Assets.GetAudio(@"Sounds\Ship\Shield Fail.wav", 0.75f);
            ShieldDownSound = _engine.Assets.GetAudio(@"Sounds\Ship\Shield Down.wav", 0.75f);
            ShieldMaxSound = _engine.Assets.GetAudio(@"Sounds\Ship\Shield Max.wav", 0.75f);
            ShieldNominalSound = _engine.Assets.GetAudio(@"Sounds\Ship\Shield Nominal.wav", 0.75f);
            AllSystemsGoSound = _engine.Assets.GetAudio(@"Sounds\Ship\All Systems Go.wav", 0.75f);
            AmmoLowSound = _engine.Assets.GetAudio(@"Sounds\Ship\Ammo Low.wav", 0.75f);
            AmmoEmptySound = _engine.Assets.GetAudio(@"Sounds\Ship\Ammo Empty.wav", 0.75f);
            ShipEngineRoarSound = _engine.Assets.GetAudio(@"Sounds\Ship\Engine Roar.wav", 0.5f, true);
            ShipEngineIdleSound = _engine.Assets.GetAudio(@"Sounds\Ship\Engine Idle.wav", 0.5f, true);
            ShipEngineBoostSound = _engine.Assets.GetAudio(@"Sounds\Ship\Engine Boost.wav", 0.5f, true);

            Orientation = new SiVector(0);
            RecalculateMovementVector();
            Throttle = 0;

            RenewableResources.Create(BoostResourceName, _engine.Settings.MaxPlayerBoostAmount,
                _engine.Settings.MaxPlayerBoostAmount, 250f, _engine.Settings.MaxPlayerBoostAmount / 10);

            if (ThrusterAnimation == null || ThrusterAnimation.IsQueuedForDeletion == true)
            {
                ThrusterAnimation = new SpriteAnimation(_engine, @"Sprites\Animation\ThrustStandard32x32.png")
                {
                    SpriteTag = "PlayerForwardThrust",
                    Visable = false,
                    OwnerUID = UID
                };
                //ThrustAnimation.Reset();
                _engine.Sprites.Animations.Insert(ThrusterAnimation, this);
                ThrusterAnimation.OnVisibilityChanged += (sender) => UpdateThrustAnimationPositions();
            }

            if (BoosterAnimation == null || BoosterAnimation.IsQueuedForDeletion == true)
            {
                BoosterAnimation = new SpriteAnimation(_engine, @"Sprites\Animation\ThrustBoost32x32.png")
                {
                    SpriteTag = "PlayerForwardThrust",
                    Visable = false,
                    OwnerUID = UID
                };
                //BoostAnimation.Reset();
                _engine.Sprites.Animations.Insert(BoosterAnimation, this);
                BoosterAnimation.OnVisibilityChanged += (sender) => UpdateThrustAnimationPositions();
            }

            CenterInUniverse();
        }

        public override void Cleanup()
        {
            ThrusterAnimation?.QueueForDelete();
            BoosterAnimation?.QueueForDelete();
            base.Cleanup();
        }

        public override void VisibilityChanged()
        {
            UpdateThrustAnimationPositions();
            if (Visable == false)
            {
                if (ThrusterAnimation != null) ThrusterAnimation.Visable = false;
                if (BoosterAnimation != null) BoosterAnimation.Visable = false;
                ShipEngineIdleSound?.Stop();
                ShipEngineRoarSound?.Stop();
            }
        }

        public override void RotationChanged() => UpdateThrustAnimationPositions();

        //The player position does not change, only the background offset changes... hmmmm. :/
        public override void LocationChanged() => UpdateThrustAnimationPositions();

        public string GetLoadoutHelpText()
        {
            string weaponName = SiReflection.GetStaticPropertyValue(Metadata.PrimaryWeapon.Type, "Name");
            string primaryWeapon = $"{weaponName} x{Metadata.PrimaryWeapon.MunitionCount}";

            string secondaryWeapons = string.Empty;
            foreach (var weapon in Metadata.Weapons)
            {
                weaponName = SiReflection.GetStaticPropertyValue(weapon.Type, "Name");
                secondaryWeapons += $"{weaponName} x{weapon.MunitionCount}\n{new string(' ', 20)}";
            }

            string result = $"             Name : {Metadata.Name}\n";
            result += $"   Primary weapon : {primaryWeapon.Trim()}\n";
            result += $"Secondary Weapons : {secondaryWeapons.Trim()}\n";
            result += $"          Shields : {Metadata.Shields:n0}\n";
            result += $"             Hull : {Metadata.Hull:n0}\n";
            result += $"            Speed : {Metadata.Speed:n1}\n";
            result += $"         Throttle : {Metadata.MaxThrottle:n1}\n";
            result += $"\n{Metadata.Description}";

            return result;
        }

        /// <summary>
        /// Resets ship state, health etc while keeping the existing class.
        /// </summary>
        public void Reset()
        {
            ReviveDeadOrExploded();

            //TODO: We should reload metadata and reapply it.
        }

        public override void AddShieldHealth(int pointsToAdd)
        {
            if (ShieldHealth < _engine.Settings.MaxShieldHealth && ShieldHealth + pointsToAdd >= _engine.Settings.MaxShieldHealth)
            {
                ShieldMaxSound.Play(); //If we didnt have full shields but now we do, tell the player.
            }

            base.AddShieldHealth(pointsToAdd);
        }

        private void UpdateThrustAnimationPositions()
        {
            var pointBehind = (Orientation * -1) * new SiVector(40, 40);

            if (ThrusterAnimation != null)
            {
                if (Visable)
                {
                    ThrusterAnimation.Orientation = Orientation;
                    ThrusterAnimation.Location = Location + pointBehind;
                }
            }

            if (BoosterAnimation != null)
            {
                if (Visable)
                {
                    BoosterAnimation.Orientation = Orientation;
                    BoosterAnimation.Location = Location + pointBehind;
                }
            }
        }

        public override void MunitionHit(MunitionBase munition)
        {
            Hit(munition);
            if (HullHealth <= 0)
            {
                //Explode(); //We don't auto delete the player because the engine always assumes its valid. 
            }
        }

        public override bool TryMunitionHit(MunitionBase munition, SiVector hitTestPosition)
        {
            if (munition.FiredFromType == SiFiredFromType.Enemy)
            {
                return IntersectsAABB(hitTestPosition);
            }
            return false;
        }

        private void SpritePlayer_OnHit(SpriteBase sender, SiDamageType damageType, int damageAmount)
        {
            if (damageType == SiDamageType.Shield)
            {
                if (ShieldHealth == 0)
                {
                    ShieldDownSound.Play();
                }
            }

            //This is the hit that took us under the treshold.
            if (HullHealth < 100 && HullHealth + damageAmount > 100)
            {
                IntegrityLowSound.Play();
            }
            else if (HullHealth < 50 && HullHealth + damageAmount > 50)
            {
                SystemsFailingSound.Play();
            }
            else if (HullHealth < 20 && HullHealth + damageAmount > 20)
            {
                HullBreachedSound.Play();
            }
        }

        #region Weapons selection and evaluation.

        public void SetPrimaryWeapon(string weaponTypeName, int munitionCount)
        {
            var weaponType = SiReflection.GetTypeByName(weaponTypeName);

            PrimaryWeapon = SiReflection.CreateInstanceFromType<WeaponBase>(weaponType, [_engine, this]);
            PrimaryWeapon.RoundQuantity = munitionCount;
        }

        public WeaponBase SelectPreviousAvailableUsableSecondaryWeapon()
        {
            WeaponBase previousWeapon = null;

            foreach (var weapon in Weapons)
            {
                if (weapon == SelectedSecondaryWeapon)
                {
                    if (previousWeapon == null)
                    {
                        return SelectLastAvailableUsableSecondaryWeapon(); //No sutible weapon found after the current one. Go back to the end.
                    }
                    SelectedSecondaryWeapon = previousWeapon;
                    return previousWeapon;
                }

                previousWeapon = weapon;
            }

            return SelectFirstAvailableUsableSecondaryWeapon(); //No sutible weapon found after the current one. Go back to the beginning.
        }

        public WeaponBase SelectNextAvailableUsableSecondaryWeapon()
        {
            bool selectNextWeapon = false;

            foreach (var weapon in Weapons)
            {
                if (selectNextWeapon)
                {
                    SelectedSecondaryWeapon = weapon;
                    return weapon;
                }

                if (weapon == SelectedSecondaryWeapon) //Find the current weapon in the collection;
                {
                    selectNextWeapon = true;
                }
            }

            return SelectFirstAvailableUsableSecondaryWeapon(); //No sutible weapon found after the current one. Go back to the beginning.
        }

        public WeaponBase SelectFirstAvailableUsableSecondaryWeapon()
        {
            var existingWeapon = (from o in Weapons where o.RoundQuantity > 0 select o).FirstOrDefault();
            if (existingWeapon != null)
            {
                SelectedSecondaryWeapon = existingWeapon;
            }
            else
            {
                SelectedSecondaryWeapon = null;
            }
            return SelectedSecondaryWeapon;
        }

        public WeaponBase SelectLastAvailableUsableSecondaryWeapon()
        {
            var existingWeapon = (from o in Weapons where o.RoundQuantity > 0 select o).LastOrDefault();
            if (existingWeapon != null)
            {
                SelectedSecondaryWeapon = existingWeapon;
            }
            else
            {
                SelectedSecondaryWeapon = null;
            }
            return SelectedSecondaryWeapon;
        }

        #endregion
    }
}
