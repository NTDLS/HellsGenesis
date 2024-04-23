using Si.Engine.Sprite.Weapon.Munition._Superclass;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite._Superclass._SpriteBase
{
    public partial class SpriteBase
    {
        public void ReviveDeadOrExploded()
        {
            IsDeadOrExploded = false;
        }

        /// <summary>
        /// Subtract from the objects hullHealth.
        /// </summary>
        /// <returns></returns>
        public virtual void Hit(int damage)
        {
            if (ShieldHealth > 0)
            {
                _engine.Audio.PlayRandomShieldHit();
                damage /= 2; //Weapons do less damage to Shields. They are designed to take hits.
                damage = damage < 1 ? 1 : damage;
                damage = damage > ShieldHealth ? ShieldHealth : damage; //No need to go negative with the damage.
                ShieldHealth -= damage;

                OnHit?.Invoke(this, SiDamageType.Shield, damage);
            }
            else
            {
                _engine.Audio.PlayRandomHullHit();
                damage = damage > HullHealth ? HullHealth : damage; //No need to go negative with the damage.
                HullHealth -= damage;

                OnHit?.Invoke(this, SiDamageType.Hull, damage);
            }
        }

        /// <summary>
        /// Hits this object with a given munition.
        /// </summary>
        /// <returns></returns>
        public virtual void Hit(MunitionBase munition)
        {
            Hit(munition.Weapon.Metadata.Damage);
        }

        public virtual void Explode()
        {
            foreach (var attachment in Attachments)
            {
                attachment.Explode();
            }

            IsDeadOrExploded = true;
            _isVisible = false;

            if (this is not SpriteAttachment) //Attachments are deleted when the owning object is deleted.
            {
                QueueForDelete();
            }

            OnExplode?.Invoke(this);
        }

        public virtual void HitExplosion()
        {
            _engine.Sprites.Animations.AddRandomHitExplosionAt(this);
        }
    }
}

