using AI2D.Engine;
using AI2D.GraphicObjects;
using System;

namespace AI2D.Weapons
{
    public class WeaponBase
    {
        protected Core _core;
        protected BaseGraphicObject _owner;

        protected DateTime _lastFired = DateTime.Now.AddMinutes(-5);
        protected AudioClip _bulletSound;
        protected string _imagePath;
        public string Name { get; private set; }
        public int Speed { get; set; } = 25;
        public int RoundQuantity { get; set; } = int.MaxValue;
        public int FireDelayMilliseconds { get; set; } = 100;
        public int Damage { get; set; } = 1;

        public bool CanLockOn { get; set; } = false;
        public double MaxLockOnAngle { get; set; } = 20;
        public double MaxLocks { get; set; } = 2;
        public double MinLockDistance { get; set; } = 50;
        public double MaxLockDistance { get; set; } = 400;

        public WeaponBase(Core core, string name, string imagePath, string soundPath, float soundVolume)
        {
            _core = core;
            _imagePath = imagePath;
            _bulletSound = _core.Actors.GetSoundCached(soundPath, soundVolume);
            Name = name;
        }

        public virtual bool Fire()
        {
            if (CanFire)
            {
                RoundQuantity--;
                _bulletSound.Play();
                _core.Actors.CreateBullet(_imagePath, this, _owner);
                return true;
            }
            return false;
        }

        public void SetOwner(BaseGraphicObject owner)
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
