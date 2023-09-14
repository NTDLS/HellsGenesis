﻿using HG.Actors.BaseClasses;
using HG.Actors.Weapons.Bullets;
using HG.Actors.Weapons.Bullets.BaseClasses;
using HG.Engine;
using HG.Types;
using HG.Types.Geometry;
using System;
using System.Collections.Generic;

namespace HG.Actors.Weapons.BaseClasses
{
    internal class WeaponBase
    {
        public Guid UID { get; private set; } = Guid.NewGuid();
        protected Core _core;
        protected ActorBase _owner;

        protected DateTime _lastFired = DateTime.Now.AddMinutes(-5);
        protected HgAudioClip _fireSound;

        /// <summary>
        /// RecoilAmount is expressed in decimal percentage of thrust.
        /// </summary>
        public double RecoilAmount { get; set; } = 0;
        public double AngleVariancePercent { get; set; } = 0;
        public double SpeedVariancePercent { get; set; } = 0;
        public string Name { get; private set; }
        public int Speed { get; set; } = 25;
        public int RoundQuantity { get; set; } = int.MaxValue;
        public int RoundsFired { get; set; }
        public int FireDelayMilliseconds { get; set; } = 100;
        public int Damage { get; set; } = 1;
        public bool CanLockOn { get; set; } = false;
        public List<ActorBase> LockedOnObjects { get; set; } = new();
        public double MaxLockOnAngle { get; set; } = 10;
        public double MaxLocks { get; set; } = 1;
        public double MinLockDistance { get; set; } = 50;
        public double MaxLockDistance { get; set; } = 100;
        public bool ExplodesOnImpact { get; set; } = false;

        public WeaponBase(Core core, string name, string soundPath, float soundVolume)
        {
            _core = core;
            _fireSound = _core.Audio.Get(soundPath, soundVolume);
            Name = name;
        }

        public WeaponBase(Core core, ActorShipBase owner, string name, string soundPath, float soundVolume)
        {
            _owner = owner;
            _core = core;
            _fireSound = _core.Audio.Get(soundPath, soundVolume);
            Name = name;
        }

        public virtual BulletBase CreateBullet(ActorBase lockedTarget, HgPoint xyOffset = null)
        {
            if (_owner == null)
            {
                throw new ArgumentNullException("Weapon is not owned.");
            }
            return new BulletGeneric(_core, this, _owner, @"Graphics\Weapon\BulletGeneric.png", lockedTarget, xyOffset);
        }

        public virtual void ApplyIntelligence(HgPoint displacementVector, ActorBase wouldFireAt)
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
                _core.Actors.Bullets.Create(this, _owner);

                ApplyRecoil();

                return true;
            }

            return false;
        }

        public void ApplyRecoil()
        {
            _owner.Velocity.RecoilPercentage += RecoilAmount;
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
