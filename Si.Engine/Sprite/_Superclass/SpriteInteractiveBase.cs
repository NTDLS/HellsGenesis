using Newtonsoft.Json;
using Si.Engine.Loudout;
using Si.Engine.Sprite.Player._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Library;
using Si.Library.Mathematics;
using Si.Library.Mathematics.Geometry;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Si.Engine.Sprite._Superclass
{
    /// <summary>
    /// A sprite that the player can see, probably shoot and destroy and might even shoot back.
    /// </summary>
    public class SpriteInteractiveBase : SpriteBase
    {
        public SiTimeRenewableResources RenewableResources { get; set; } = new();

        public bool TakesDamage { get; set; }

        public List<WeaponBase> Weapons { get; private set; } = new();

        public int Bounty { get; set; } = 0;

        public SpriteInteractiveBase(EngineCore engine, string name = "")
            : base(engine, name)
        {
            _engine = engine;
        }

        /// <summary>
        /// Sets the sprites image, sets speed, shields, adds attachements and weapons
        /// from a .json file in the same path with the same name as the sprite image.
        /// </summary>
        /// <param name="spriteImagePath"></param>
        public void SetImageAndLoadMetadata(string spriteImagePath)
        {
            SetImage(spriteImagePath);

            string metadataFile = $"{Path.GetDirectoryName(spriteImagePath)}\\{Path.GetFileNameWithoutExtension(spriteImagePath)}.json";
            var metadataJson = _engine.Assets.GetText(metadataFile);

            var metadata = JsonConvert.DeserializeObject<SpriteMetadata>(metadataJson);

            Velocity.MaximumSpeed = metadata.Speed;
            Velocity.MaximumSpeedBoost = metadata.Boost;
            Bounty = metadata.Bounty;
            TakesDamage = metadata.TakesDamage;

            SetHullHealth(metadata.HullHealth);
            SetShieldHealth(metadata.ShieldHealth);

            foreach (var weapon in metadata.Weapons)
            {
                AddWeapon(weapon.Type, weapon.MunitionCount);
            }

            foreach (var attachment in metadata.Attachments)
            {
                AttachOfType(attachment.Type, attachment.LocationRelativeToOwner);
            }

            if (this is SpritePlayerBase player)
            {
                player.Meta = metadata;

                player.SetPrimaryWeapon(metadata.PrimaryWeapon.Type, metadata.PrimaryWeapon.MunitionCount);

                player.SelectFirstAvailableUsableSecondaryWeapon();
            }

        }

        #region Weapons selection and evaluation.

        public void ClearWeapons() => Weapons.Clear();

        public void AddWeapon(string weaponTypeName, int munitionCount)
        {
            var weaponType = SiReflection.GetTypeByName(weaponTypeName);

            var weapon = Weapons.Where(o => o.GetType() == weaponType).SingleOrDefault();

            if (weapon == null)
            {
                weapon = SiReflection.CreateInstanceFromType<WeaponBase>(weaponType, new object[] { _engine, this });
                weapon.RoundQuantity += munitionCount;
                Weapons.Add(weapon);
            }
            else
            {
                weapon.RoundQuantity += munitionCount;
            }
        }

        public void AddWeapon<T>(int munitionCount) where T : WeaponBase
        {
            var weapon = GetWeaponOfType<T>();
            if (weapon == null)
            {
                weapon = SiReflection.CreateInstanceOf<T>(new object[] { _engine, this });
                weapon.RoundQuantity += munitionCount;
                Weapons.Add(weapon);
            }
            else
            {
                weapon.RoundQuantity += munitionCount;
            }
        }

        public int TotalAvailableWeaponRounds() => (from o in Weapons select o.RoundQuantity).Sum();
        public int TotalWeaponFiredRounds() => (from o in Weapons select o.RoundsFired).Sum();

        public bool HasWeapon<T>() where T : WeaponBase
        {
            var existingWeapon = (from o in Weapons where o.GetType() == typeof(T) select o).FirstOrDefault();
            return existingWeapon != null;
        }

        public bool HasWeaponAndAmmo<T>() where T : WeaponBase
        {
            var existingWeapon = (from o in Weapons where o.GetType() == typeof(T) select o).FirstOrDefault();
            return existingWeapon != null && existingWeapon.RoundQuantity > 0;
        }

        public bool FireWeapon<T>() where T : WeaponBase
        {
            var weapon = GetWeaponOfType<T>();
            return weapon?.Fire() == true;
        }

        public bool FireWeapon<T>(SiPoint location, float? angle = null) where T : WeaponBase
        {
            var weapon = GetWeaponOfType<T>();
            return weapon?.Fire(location, angle) == true;
        }

        public WeaponBase GetWeaponOfType<T>() where T : WeaponBase
        {
            return (from o in Weapons where o.GetType() == typeof(T) select o).FirstOrDefault();
        }

        #endregion

        public override bool TryMunitionHit(MunitionBase munition, SiPoint hitTestPosition)
        {
            if (Intersects(hitTestPosition))
            {
                Hit(munition);
                if (HullHealth <= 0)
                {
                    Explode();
                }
                return true;
            }
            return false;
        }

        public override void Explode()
        {
            _engine.Events.Add(() =>
            {
                _engine.Sprites.Animations.AddRandomExplosionAt(this);
                _engine.Sprites.Particles.ParticleBlastAt(SiRandom.Between(200, 800), this);
                _engine.Sprites.CreateFragmentsOf(this);
                _engine.Rendering.AddScreenShake(4, 800);
                _engine.Audio.PlayRandomExplosion();
            });
            base.Explode();
        }

        /// <summary>
        /// Provides a way to make decisions about the sprite that do not necessirily have anyhting to do with movement.
        /// </summary>
        /// <param name="epoch"></param>
        /// <param name="displacementVector"></param>
        public virtual void ApplyIntelligence(float epoch, SiPoint displacementVector)
        {
        }
    }
}
