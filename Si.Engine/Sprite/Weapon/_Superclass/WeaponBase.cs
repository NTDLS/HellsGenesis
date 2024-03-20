using Newtonsoft.Json;
using Si.Audio;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Engine.Sprite.Player._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.GameEngine.Sprite.Metadata;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;
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

        public WeaponMetadata Meta { get; set; }
        public List<WeaponsLock> LockedTargets { get; set; } = new();
        public int RoundsFired { get; set; }
        public int RoundQuantity { get; set; }

        public WeaponBase(EngineCore engine, string name, string soundPath, float soundVolume)
        {
            _engine = engine;
            _fireSound = _engine.Assets.GetAudio(soundPath, soundVolume);
            LoadMetadata(name);
        }

        public WeaponBase(EngineCore engine, SpriteInteractiveBase owner, string name, string soundPath, float soundVolume)
        {
            Owner = owner;
            _engine = engine;
            _fireSound = _engine.Assets.GetAudio(soundPath, soundVolume);
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
            Meta = JsonConvert.DeserializeObject<WeaponMetadata>(metadataJson);
            Meta.MunitionSceneDistanceLimit = _engine.Settings.MunitionSceneDistanceLimit;
        }

        public virtual MunitionBase CreateMunition(SiPoint location = null, SpriteInteractiveBase lockedTarget = null)
        {
            if (Owner == null)
            {
                throw new ArgumentNullException("Weapon is not owned.");
            }
            throw new Exception("Create munition should always be overridden by the owning weapon.");

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
                    if (Meta.CanLockOn && Owner.IsPointingAt(potentialTarget, Meta.MaxLockOnAngle))
                    {
                        var distance = Owner.DistanceTo(potentialTarget);
                        if (distance.IsBetween(Meta.MinLockDistance, Meta.MaxLockDistance))
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

                foreach (var hardLock in LockedTargets.Take(Meta.MaxLocks))
                {
                    hardLock.LockType = SiWeaponsLockType.Hard;
                    hardLock.Sprite.IsLockedOnHard = true;
                    hardLock.Sprite.IsLockedOnSoft = false;
                }

                foreach (var softLock in LockedTargets.Skip(Meta.MaxLocks))
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

                if (Meta.CanLockOn && Owner.IsPointingAt(_engine.Player.Sprite, Meta.MaxLockOnAngle))
                {
                    var distance = Owner.DistanceTo(_engine.Player.Sprite);
                    if (distance.IsBetween(Meta.MinLockDistance, Meta.MaxLockDistance))
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

        public virtual bool Fire(SiPoint location)
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
                    result = (DateTime.Now - _lastFired).TotalMilliseconds > Meta.FireDelayMilliseconds;
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
