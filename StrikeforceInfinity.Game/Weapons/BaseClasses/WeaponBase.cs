﻿using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Sprites;
using StrikeforceInfinity.Game.Utility.ExtensionMethods;
using StrikeforceInfinity.Game.Weapons.Munitions;
using System;
using System.Collections.Generic;

namespace StrikeforceInfinity.Game.Weapons.BaseClasses
{
    /// <summary>
    /// A weapon is a "device" that fires a "munition" (_MunitionBase). It must be owned by another sprite.
    /// </summary>
    internal class WeaponBase
    {
        public Guid UID { get; private set; } = Guid.NewGuid();
        protected EngineCore _gameCore;
        protected SpriteBase _owner;

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

        public WeaponBase(EngineCore gameCore, string name, string soundPath, float soundVolume)
        {
            _gameCore = gameCore;
            _fireSound = _gameCore.Assets.GetAudio(soundPath, soundVolume);
            Name = name;
        }

        public WeaponBase(EngineCore gameCore, _SpriteShipBase owner, string name, string soundPath, float soundVolume)
        {
            _owner = owner;
            _gameCore = gameCore;
            _fireSound = _gameCore.Assets.GetAudio(soundPath, soundVolume);
            Name = name;
        }

        public virtual MunitionBase CreateMunition(SiPoint xyOffset, SpriteBase lockedTarget = null)
        {
            if (_owner == null)
            {
                throw new ArgumentNullException("Weapon is not owned.");
            }
            throw new Exception("Create munition should always be overridden by the owning weapon.");

        }

        public virtual bool ApplyWeaponsLock(SiPoint displacementVector, SpriteBase wouldFireAt)
        {
            if (_owner == null)
            {
                throw new ArgumentNullException("Weapon is not owned.");
            }

            bool lockOn = false;
            bool softLockOn = false;

            if (CanLockOn)
            {
                if (_owner.IsPointingAt(wouldFireAt, MaxLockOnAngle))
                {
                    var distance = _owner.DistanceTo(wouldFireAt);
                    if (distance >= MinLockDistance && distance <= MaxLockDistance)
                    {
                        if (LockedOnObjects.Count < MaxLocks)
                        {
                            lockOn = true;
                            LockedOnObjects.Add(wouldFireAt);
                        }
                        else
                        {
                            softLockOn = true;
                            wouldFireAt.IsLockedOnSoft = true;
                        }
                    }
                }
            }

            wouldFireAt.IsLockedOn = lockOn;
            wouldFireAt.IsLockedOnSoft = softLockOn;

            return lockOn || softLockOn;
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
                _gameCore.Sprites.Munitions.Create(this);

                ApplyRecoil();

                return true;
            }

            return false;
        }

        public void ApplyRecoil()
        {
            _owner.Velocity.RecoilPercentage += RecoilAmount;
            _owner.Velocity.RecoilPercentage.Box(0, _gameCore.Settings.MaxRecoilPercentage);
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
