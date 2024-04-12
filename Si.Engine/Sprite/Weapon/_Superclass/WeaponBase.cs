using Newtonsoft.Json;
using Si.Audio;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Engine.Sprite.Player._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.GameEngine.Sprite.SupportingClasses.Metadata;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.Weapon._Superclass
{
    /// <summary>
    /// A weapon is a "device" that fires a "munition" (_MunitionBase). It must be owned by another sprite.
    /// </summary>
    public class WeaponBase
    {
        public Guid UID { get; private set; } = Guid.NewGuid();
        protected EngineCore _engine;
        protected SpriteInteractiveBase Owner { get; private set; }
        protected DateTime _lastFired = DateTime.Now.AddMinutes(-5);
        protected SiAudioClip _fireSound;

        public WeaponMetadata Metadata { get; set; }
        public List<WeaponsLock> LockedTargets { get; set; } = new();
        public int RoundsFired { get; set; }
        public int RoundQuantity { get; set; }

        public WeaponBase(EngineCore engine, SpriteInteractiveBase owner, string name)
        {
            Owner = owner;
            _engine = engine;

            LoadMetadata(name);
        }

        /// <summary>
        /// Sets the sprites image, sets speed, shields, adds attachements and weapons
        /// from a .json file in the same path with the same name as the sprite image.
        /// </summary>
        /// <param name="spriteImagePath"></param>
        public void LoadMetadata(string weaponName)
        {
            var metadataJson = _engine.Assets.GetText($@"Sprites\Weapon\{weaponName}.json");
            Metadata = JsonConvert.DeserializeObject<WeaponMetadata>(metadataJson);
            Metadata.MunitionSceneDistanceLimit = _engine.Settings.MunitionSceneDistanceLimit;

            if (string.IsNullOrEmpty(Metadata.SoundPath) == false)
            {
                _fireSound = _engine.Assets.GetAudio(Metadata.SoundPath, Metadata.SoundVolume);
            }
        }

        public MunitionBase CreateMunition(SiVector location = null, SpriteInteractiveBase lockedTarget = null)
        {
            if (Owner == null)
            {
                throw new Exception("Weapon is not owned.");
            }

            string spritePath = null;

            int? spriteCount = Metadata.SpritePaths?.Count();
            if (spriteCount > 0)
            {
                spritePath = Metadata.SpritePaths[SiRandom.Between(0, ((int)spriteCount) - 1)];
            }

            switch (Metadata.MunitionType)
            {
                case MunitionType.Projectile:
                    {
                        var munition = new ProjectileMunitionBase(_engine, this, Owner, spritePath, location ?? Owner.Location);
                        return munition;
                    }
                case MunitionType.Energy:
                    {
                        var munition = new EnergyMunitionBase(_engine, this, Owner, spritePath, location ?? Owner.Location);
                        return munition;
                    }
                case MunitionType.Seeking:
                    {
                        var munition = new SeekingMunitionBase(_engine, this, Owner, spritePath, location ?? Owner.Location)
                        {
                            SeekingRotationRateRadians = Metadata.GuidanceRotation.ToRadians(),
                            MaxSeekingObservationAngleDegrees = Metadata.GuidanceAngle,
                            MaxSeekingObservationDistance = Metadata.GuidanceDistance
                        };
                        return munition;
                    }
                case MunitionType.Locking:
                    {
                        var munition = new LockingMunitionBase(_engine, this, Owner, spritePath, lockedTarget, location ?? Owner.Location)
                        {
                            GuidedRotationRateInRadians = Metadata.GuidanceRotation.ToRadians(),
                            MaxGuidedObservationAngleDegrees = Metadata.GuidanceAngle
                        };
                        return munition;
                    }
                default:
                    throw new Exception($"The weapon type {Metadata?.MunitionType} is not implemented.");
            }
        }

        public class WeaponsLock
        {
            public float Distance { get; set; }
            public SpriteInteractiveBase Sprite { get; set; }

            public SiWeaponsLockType LockType { get; set; }
        }

        public virtual void ApplyIntelligence(float epoch)
        {
            //We're just doing "locked on" magic here.

            LockedTargets.Clear();

            if (Owner is SpritePlayerBase owner)
            {
                var potentialTargets = _engine.Sprites.Enemies.Visible();

                foreach (var potentialTarget in potentialTargets)
                {
                    if (Metadata.MunitionType == MunitionType.Locking && Owner.IsPointingAt(potentialTarget, Metadata.MaxLockOnAngle))
                    {
                        var distance = Owner.DistanceTo(potentialTarget);
                        if (distance.IsBetween(Metadata.MinLockDistance, Metadata.MaxLockDistance))
                        {
                            LockedTargets.Add(new WeaponsLock()
                            {
                                Sprite = potentialTarget,
                                Distance = Owner.DistanceTo(potentialTarget)
                            });
                        }
                    }
                }

                LockedTargets = LockedTargets.OrderBy(o => o.Distance).ToList();

                foreach (var hardLock in LockedTargets.Take(Metadata.MaxLocks))
                {
                    hardLock.LockType = SiWeaponsLockType.Hard;
                    hardLock.Sprite.IsLockedOnHard = true;
                    hardLock.Sprite.IsLockedOnSoft = false;
                }

                foreach (var softLock in LockedTargets.Skip(Metadata.MaxLocks))
                {
                    softLock.LockType = SiWeaponsLockType.Soft;
                    softLock.Sprite.IsLockedOnHard = false;
                    softLock.Sprite.IsLockedOnSoft = true;
                }

                var lockedTargets = LockedTargets.Select(o => o.Sprite);

                foreach (var potentialTarget in potentialTargets.Where(o => !lockedTargets.Contains(o)))
                {
                    potentialTarget.IsLockedOnHard = false;
                    potentialTarget.IsLockedOnSoft = false;
                }
            }
            else if (Owner is SpriteEnemyBase enemy)
            {
                _engine.Player.Sprite.IsLockedOnSoft = false;
                _engine.Player.Sprite.IsLockedOnHard = false;

                if (Metadata.MunitionType == MunitionType.Locking && Owner.IsPointingAt(_engine.Player.Sprite, Metadata.MaxLockOnAngle))
                {
                    var distance = Owner.DistanceTo(_engine.Player.Sprite);
                    if (distance.IsBetween(Metadata.MinLockDistance, Metadata.MaxLockDistance))
                    {
                        _engine.Player.Sprite.IsLockedOnHard = true;
                        _engine.Player.Sprite.IsLockedOnSoft = false;

                        LockedTargets.Add(new WeaponsLock()
                        {
                            Sprite = _engine.Player.Sprite,
                            Distance = Owner.DistanceTo(_engine.Player.Sprite),
                            LockType = SiWeaponsLockType.Hard
                        });
                    }
                }
            }
        }

        public virtual bool Fire(SiVector location)
        {
            if (Owner == null)
            {
                throw new ArgumentNullException("Weapon is not owned.");
            }

            if (CanFire)
            {
                RoundsFired++;
                RoundQuantity--;
                _fireSound.Play();
                _engine.Sprites.Munitions.Add(this, location);

                return true;
            }

            return false;
        }

        public virtual bool Fire()
        {
            if (Owner == null)
            {
                throw new ArgumentNullException("Weapon is not owned.");
            }

            if (CanFire)
            {
                RoundsFired++;
                RoundQuantity--;
                _fireSound.Play();
                _engine.Sprites.Munitions.Add(this);

                return true;
            }

            return false;
        }

        public virtual void Hit()
        {
        }

        public bool CanFire
        {
            get
            {
                bool result = false;
                if (RoundQuantity > 0)
                {
                    result = (DateTime.Now - _lastFired).TotalMilliseconds > Metadata.FireDelayMilliseconds;
                    if (result)
                    {
                        _lastFired = DateTime.Now;
                    }
                }
                return result;
            }
        }
    }
}
