using Si.GameEngine.Core;
using Si.GameEngine.Core.Types;
using Si.GameEngine.Sprites._Superclass;
using Si.GameEngine.Sprites.Player._Superclass;
using Si.GameEngine.Sprites.Weapons.Munitions._Superclass;
using Si.Library.ExtensionMethods;
using Si.Library.Types.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Si.GameEngine.Sprites.Weapons._Superclass
{
    /// <summary>
    /// A weapon is a "device" that fires a "munition" (_MunitionBase). It must be owned by another sprite.
    /// </summary>
    public class WeaponBase
    {
        public Guid UID { get; private set; } = Guid.NewGuid();
        protected GameEngineCore _gameEngine;
        protected SpriteShipBase _owner;

        protected DateTime _lastFired = DateTime.Now.AddMinutes(-5);
        protected SiAudioClip _fireSound;

        /// <summary>
        /// RecoilAmount is expressed in decimal percentage of thrust.
        /// </summary>
        public double RecoilAmount { get; set; } = 0;
        /// <summary>
        /// The variance in degrees that the loaded munition will use for an initial heading angle.
        /// </summary>
        public double AngleVarianceDegrees { get; set; } = 0;
        /// <summary>
        /// The variance expressed in decimal percentage that determines the loaded munitions initial velovity.
        /// </summary>
        public double SpeedVariancePercent { get; set; } = 0;
        /// <summary>
        /// The distance from the total canvas that the munition will be allowed to travel before it is deleted.
        /// </summary>
        public double MunitionSceneDistanceLimit { get; set; }
        public string Name { get; private set; }
        public int Speed { get; set; } = 25;
        public int RoundQuantity { get; set; }
        public int RoundsFired { get; set; }
        public int FireDelayMilliseconds { get; set; } = 100;
        public int Damage { get; set; } = 1;
        public bool CanLockOn { get; set; } = false;
        public List<SpriteBase> LockedOnObjects { get; set; } = new();
        public double MaxLockOnAngle { get; set; } = 10;
        public double MaxLocks { get; set; } = 1;
        public double MinLockDistance { get; set; } = 50;
        public double MaxLockDistance { get; set; } = 100;
        public bool ExplodesOnImpact { get; set; } = false;

        public WeaponBase(GameEngineCore gameEngine, string name, string soundPath, float soundVolume)
        {
            _gameEngine = gameEngine;
            _fireSound = _gameEngine.Assets.GetAudio(soundPath, soundVolume);
            Name = name;
            MunitionSceneDistanceLimit = _gameEngine.Settings.MunitionSceneDistanceLimit;
        }

        public WeaponBase(GameEngineCore gameEngine, SpriteShipBase owner, string name, string soundPath, float soundVolume)
        {
            _owner = owner;
            _gameEngine = gameEngine;
            _fireSound = _gameEngine.Assets.GetAudio(soundPath, soundVolume);
            Name = name;
            MunitionSceneDistanceLimit = _gameEngine.Settings.MunitionSceneDistanceLimit;
        }

        public virtual MunitionBase CreateMunition(SiPoint xyOffset, SpriteBase lockedTarget = null)
        {
            if (_owner == null)
            {
                throw new ArgumentNullException("Weapon is not owned.");
            }
            throw new Exception("Create munition should always be overridden by the owning weapon.");

        }

        /// <summary>
        /// Removes all locks that this weapon has acquired.
        /// </summary>
        public virtual void ClearWeaponsLocks()
        {
            LockedOnObjects.ForEach(o =>
            {
                o.IsLockedOnHard = false;
                o.IsLockedOnSoft = false;
            });

            LockedOnObjects.Clear();
        }

        /// <summary>
        /// Gets soct locks on targets, these will be translated into hard locks later by HardenWeaponsLocks().
        /// </summary>
        /// <param name="wouldFireAt"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual void AcquireSoftWeaponsLocks(SpriteBase wouldFireAt)
        {
            if (_owner is SpritePlayerBase)
            {
            }

            if (_owner == null)
            {
                throw new ArgumentNullException("Weapon is not owned.");
            }

            if (CanLockOn)
            {
                if (_owner.IsPointingAt(wouldFireAt, MaxLockOnAngle))
                {
                    var distance = _owner.DistanceTo(wouldFireAt);
                    if (distance.IsBetween(MinLockDistance, MaxLockDistance))
                    {
                        LockedOnObjects.Add(wouldFireAt);
                        wouldFireAt.IsLockedOnSoft = true;
                    }
                }
            }
        }

        class DistanceLock
        {
            public double Distance { get; set; }
            public SpriteBase Sprite { get; set; }
        }

        public virtual void HardenWeaponsLocks()
        {
            if (_owner == null)
            {
                throw new ArgumentNullException("Weapon is not owned.");
            }

            var distanceLocks = new List<DistanceLock>();

            LockedOnObjects.ForEach(o =>
            {
                distanceLocks.Add(new DistanceLock()
                {
                    Sprite = o,
                    Distance = _owner.DistanceTo(o)
                });
            });

            distanceLocks = distanceLocks.OrderBy(o => o.Distance).ToList();

            foreach (var hardLock in distanceLocks.Take((int)MaxLocks))
            {
                hardLock.Sprite.IsLockedOnHard = true;
                hardLock.Sprite.IsLockedOnSoft = false;
            }

            foreach (var softLock in distanceLocks.Skip((int)MaxLocks))
            {
                softLock.Sprite.IsLockedOnHard = false;
                softLock.Sprite.IsLockedOnSoft = true;
                LockedOnObjects.Remove(softLock.Sprite);
            }
        }

        public virtual bool Fire()
        {
            if (_owner == null)
            {
                throw new ArgumentNullException("Weapon is not owned.");
            }

            if (CanFire)
            {
                RoundsFired++;
                RoundQuantity--;
                _fireSound.Play();
                _gameEngine.Sprites.Munitions.Create(this);

                ApplyRecoil();

                return true;
            }

            return false;
        }

        public void ApplyRecoil()
        {
            //TODO: uncomment this later, testing multiplayer.
            //_owner.Velocity.RecoilPercentage += RecoilAmount;
            //_owner.Velocity.RecoilPercentage.Box(0, _gameEngine.Settings.MaxRecoilPercentage);
        }

        public virtual void Hit()
        {
        }

        public bool CanFire
        {
            get
            {
                bool result = false;
                if (RoundQuantity > 0 || _owner.IsDrone)
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
