using static Si.Library.SiConstants;

namespace Si.Engine.Sprite._Superclass._Root
{
    public partial class SpriteBase
    {
        public delegate void HitEvent(SpriteBase sender, SiDamageType damageType, int damageAmount);
        public event HitEvent? OnHit;

        public delegate void QueuedForDeleteEvent(SpriteBase sender);
        public event QueuedForDeleteEvent? OnQueuedForDelete;

        public delegate void VisibilityChangedEvent(SpriteBase sender);
        public event VisibilityChangedEvent? OnVisibilityChanged;

        public delegate void ExplodeEvent(SpriteBase sender);
        public event ExplodeEvent? OnExplode;

        public virtual void VisibilityChanged() { }
        public virtual void LocationChanged() { }
        public virtual void RotationChanged() { }
    }
}
