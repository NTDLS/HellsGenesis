using AI2D.Engine;
using AI2D.Actors;
using AI2D.Actors.Bullets;
using AI2D.Types;
using System;
using System.Collections.Generic;

namespace AI2D.Weapons
{
    public class WeaponBase
    {
        public Guid UID { get; private set; } = Guid.NewGuid();
        protected Core _core;
        protected ActorBase _owner;

        protected DateTime _lastFired = DateTime.Now.AddMinutes(-5);
        protected AudioClip _fireSound;

        public string Name { get; private set; }
        public int Speed { get; set; } = 25;
        public int RoundQuantity { get; set; } = int.MaxValue;
        public int RoundsFired { get; set; }
        public int FireDelayMilliseconds { get; set; } = 100;
        public int Damage { get; set; } = 1;
        public bool CanLockOn { get; set; } = false;
        public List<ActorBase> LockedOnObjects { get; set; } = new List<ActorBase>();
        public double MaxLockOnAngle { get; set; } = 10;
        public double MaxLocks { get; set; } = 1;
        public double MinLockDistance { get; set; } = 50;
        public double MaxLockDistance { get; set; } = 100;
        public bool ExplodesOnImpact { get; set; } = false;

        public WeaponBase(Core core, string name, string soundPath, float soundVolume)
        {
            _core = core;
            _fireSound = _core.Actors.GetSoundCached(soundPath, soundVolume);
            Name = name;
        }

        public virtual BulletBase CreateBullet(ActorBase lockedTarget, PointD xyOffset = null)
        {
            return new BulletGeneric(_core, this, _owner, @"..\..\..\Assets\Graphics\Weapon\BulletGeneric.png", lockedTarget, xyOffset);
        }

        public virtual void ApplyIntelligence(PointD frameAppliedOffset, ActorBase wouldFireAt)
        {
            bool lockOn = false;
            bool softLockOn = false;

            if (this.CanLockOn)
            {
                if (this._owner.IsPointingAt(wouldFireAt, this.MaxLockOnAngle))
                {
                    var distance = this._owner.DistanceTo(wouldFireAt);
                    if (distance >= this.MinLockDistance && distance <= this.MaxLockDistance)
                    {
                        if (this.LockedOnObjects.Count < this.MaxLocks)
                        {
                            lockOn = true;
                            this.LockedOnObjects.Add(wouldFireAt);
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
            if (CanFire)
            {
                RoundsFired++;
                RoundQuantity--;
                _fireSound.Play();
                _core.Actors.AddNewBullet(this, _owner);
                return true;
            }
            return false;
        }

        public virtual void Hit()
        {
        }

        public void SetOwner(ActorBase owner)
        {
            if (_owner == null)
            {
                _owner = owner;
            }
            else
            {
                throw new Exception("The weapon is already owned by an object");
            }
        }

        public bool CanFire
        {
            get
            {
                bool result = false;
                if (RoundQuantity > 0)
                {
                    result = ((DateTime.Now - _lastFired).TotalMilliseconds > FireDelayMilliseconds);
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
