using Si.Audio;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Engine.Sprite.Player._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
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
        protected SpriteBase Owner { get; private set; }

        protected DateTime _lastFired = DateTime.Now.AddMinutes(-5);
        protected SiAudioClip _fireSound;

        /// <summary>
        /// The variance in degrees that the loaded munition will use for an initial heading angle.
        /// </summary>
        public float AngleVarianceDegrees { get; set; } = 0;
        /// <summary>
        /// The variance expressed in decimal percentage that determines the loaded munitions initial velovity.
        /// </summary>
        public float SpeedVariancePercent { get; set; } = 0;
        /// <summary>
        /// The distance from the total canvas that the munition will be allowed to travel before it is deleted.
        /// </summary>
        public float MunitionSceneDistanceLimit { get; set; }
        public string Name { get; private set; }
        public float Speed { get; set; } = 25;
        public int RoundsFired { get; set; }
        public int RoundQuantity { get; set; }
        public int FireDelayMilliseconds { get; set; } = 100;
        public int Damage { get; set; } = 1;
        public bool CanLockOn { get; set; } = false;
        public List<WeaponsLock> LockedTargets { get; set; } = new();
        public float MaxLockOnAngle { get; set; } = 10;
        public float MaxLocks { get; set; } = 1;
        public float MinLockDistance { get; set; } = 50;
        public float MaxLockDistance { get; set; } = 100;
        public bool ExplodesOnImpact { get; set; } = false;

        public WeaponBase(EngineCore engine, string name, string soundPath, float soundVolume)
        {
            _engine = engine;
            _fireSound = _engine.Assets.GetAudio(soundPath, soundVolume);
            Name = name;
            MunitionSceneDistanceLimit = _engine.Settings.MunitionSceneDistanceLimit;
        }

        public WeaponBase(EngineCore engine, SpriteBase owner, string name, string soundPath, float soundVolume)
        {
            Owner = owner;
            _engine = engine;
            _fireSound = _engine.Assets.GetAudio(soundPath, soundVolume);
            Name = name;
            MunitionSceneDistanceLimit = _engine.Settings.MunitionSceneDistanceLimit;
        }


        public virtual MunitionBase CreateMunition(SiPoint location = null, float? angle = null, SpriteBase lockedTarget = null)
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
            public SpriteBase Sprite { get; set; }

            public SiWeaponsLockType LockType { get; set; }
        }

        public virtual void ApplyIntelligence(float epoch)
        {
            LockedTargets.Clear();

            if (Owner is SpritePlayerBase owner)
            {
                var potentialTargets = _engine.Sprites.Enemies.Visible();

                foreach (var potentialTarget in potentialTargets)
                {
                    if (CanLockOn && Owner.IsPointingAt(potentialTarget, MaxLockOnAngle))
                    {
                        var distance = Owner.DistanceTo(potentialTarget);
                        if (distance.IsBetween(MinLockDistance, MaxLockDistance))
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

                foreach (var hardLock in LockedTargets.Take((int)MaxLocks))
                {
                    hardLock.LockType = SiWeaponsLockType.Hard;
                    hardLock.Sprite.IsLockedOnHard = true;
                    hardLock.Sprite.IsLockedOnSoft = false;
                }

                foreach (var softLock in LockedTargets.Skip((int)MaxLocks))
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

                if (CanLockOn && Owner.IsPointingAt(_engine.Player.Sprite, MaxLockOnAngle))
                {
                    var distance = Owner.DistanceTo(_engine.Player.Sprite);
                    if (distance.IsBetween(MinLockDistance, MaxLockDistance))
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

        public virtual bool Fire(SiPoint location, float? angle = null)
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
                _engine.Sprites.Munitions.Create(this, location, angle);

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
                _engine.Sprites.Munitions.Create(this);

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
                    result = (DateTime.Now - _lastFired).TotalMilliseconds > FireDelayMilliseconds;
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
