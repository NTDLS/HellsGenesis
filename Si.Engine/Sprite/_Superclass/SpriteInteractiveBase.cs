using Newtonsoft.Json;
using SharpDX.Direct2D1;
using Si.Engine.Sprite.Player._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.GameEngine.Sprite.SupportingClasses;
using Si.GameEngine.Sprite.SupportingClasses.Metadata;
using Si.Library;
using Si.Library.Mathematics;
using Si.Library.Mathematics.Geometry;
using Si.Library.Sprite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static Si.Engine.Manager.CollisionManager;

namespace Si.Engine.Sprite._Superclass
{
    /// <summary>
    /// A sprite that the player can see, probably shoot and destroy and might even shoot back.
    /// </summary>
    public class SpriteInteractiveBase : SpriteBase
    {
        #region Locking Indicator.

        public bool IsLockedOnSoft { get; set; } //This is just graphics candy, the object would be subject of a foreign weapons lock, but the other foreign weapon owner has too many locks.
        protected Bitmap _lockedOnImage;
        protected Bitmap _lockedOnSoftImage;
        private bool _isLockedOn = false;

        public bool IsLockedOnHard //The object is the subject of a foreign weapons lock.
        {
            get => _isLockedOn;
            set
            {
                if (_isLockedOn == false && value == true)
                {
                    //TODO: This should not play every loop.
                    _engine.Audio.LockedOnBlip.Play();
                }
                _isLockedOn = value;
            }
        }

        #endregion

        public SiTimeRenewableResources RenewableResources { get; set; } = new();
        public InteractiveSpriteMetadata Metadata { get; set; }
        public List<WeaponBase> Weapons { get; private set; } = new();

        public SpriteInteractiveBase(EngineCore engine, string name = "")
            : base(engine, name)
        {
            _engine = engine;

            _lockedOnImage = _engine.Assets.GetBitmap(@"Sprites\Weapon\Locked On.png");
            _lockedOnSoftImage = _engine.Assets.GetBitmap(@"Sprites\Weapon\Locked Soft.png");
        }

        /// <summary>
        /// Sets the sprites image, sets speed, shields, adds attachements and weapons
        /// from a .json file in the same path with the same name as the sprite image.
        /// </summary>
        /// <param name="spriteImagePath"></param>
        public void SetImageAndLoadMetadata(string spriteImagePath)
        {
            SetImage(spriteImagePath);

            string metadataFile = $"{Path.GetDirectoryName(spriteImagePath)}\\{Path.GetFileNameWithoutExtension(spriteImagePath)}.json";
            var metadataJson = _engine.Assets.GetText(metadataFile);

            Metadata = JsonConvert.DeserializeObject<InteractiveSpriteMetadata>(metadataJson);

            // Set standard variables here:
            Speed = Metadata.Speed;
            Throttle = Metadata.Throttle;
            MaxThrottle = Metadata.MaxThrottle;

            SetHullHealth(Metadata.Hull);
            SetShieldHealth(Metadata.Shields);

            foreach (var weapon in Metadata.Weapons)
            {
                AddWeapon(weapon.Type, weapon.MunitionCount);
            }

            foreach (var attachment in Metadata.Attachments)
            {
                AttachOfType(attachment.Type, attachment.LocationRelativeToOwner);
            }

            if (this is SpritePlayerBase player)
            {
                player.SetPrimaryWeapon(Metadata.PrimaryWeapon.Type, Metadata.PrimaryWeapon.MunitionCount);
                player.SelectFirstAvailableUsableSecondaryWeapon();
            }
        }

        /// <summary>
        /// The total velocity multiplied by the given mass.
        /// </summary>
        /// <param name="mass"></param>
        /// <returns></returns>
        public float TotalMomentum()
            => TotalVelocity * Metadata.Mass;

        /// <summary>
        /// Number that defines how much motion a sprite is in.
        /// </summary>
        public float TotalVelocity
            => Math.Abs(MovementVector.Sum()) + Math.Abs(RotationSpeed);

        /// <summary>
        /// The total velocity multiplied by the given mass, excpet for the mass is returned when the velocity is 0;
        /// </summary>
        /// <param name="mass"></param>
        /// <returns></returns>
        public float TotalMomentumWithRestingMass()
        {
            var totalRelativeVelocity = TotalVelocity;
            if (totalRelativeVelocity == 0)
            {
                return Metadata.Mass;
            }
            return TotalVelocity * Metadata?.Mass ?? 1;
        }

        #region Weapons selection and evaluation.

        public void ClearWeapons() => Weapons.Clear();

        public void AddWeapon(string weaponTypeName, int munitionCount)
        {
            var weaponType = SiReflection.GetTypeByName(weaponTypeName);

            var weapon = Weapons.Where(o => o.GetType() == weaponType).SingleOrDefault();

            if (weapon == null)
            {
                weapon = SiReflection.CreateInstanceFromType<WeaponBase>(weaponType, new object[] { _engine, this });
                weapon.RoundQuantity += munitionCount;
                Weapons.Add(weapon);
            }
            else
            {
                weapon.RoundQuantity += munitionCount;
            }
        }

        public void AddWeapon<T>(int munitionCount) where T : WeaponBase
        {
            var weapon = GetWeaponOfType<T>();
            if (weapon == null)
            {
                weapon = SiReflection.CreateInstanceOf<T>(new object[] { _engine, this });
                weapon.RoundQuantity += munitionCount;
                Weapons.Add(weapon);
            }
            else
            {
                weapon.RoundQuantity += munitionCount;
            }
        }

        public int TotalAvailableWeaponRounds() => (from o in Weapons select o.RoundQuantity).Sum();
        public int TotalWeaponFiredRounds() => (from o in Weapons select o.RoundsFired).Sum();

        public bool HasWeapon<T>() where T : WeaponBase
        {
            var existingWeapon = (from o in Weapons where o.GetType() == typeof(T) select o).FirstOrDefault();
            return existingWeapon != null;
        }

        public bool HasWeaponAndAmmo<T>() where T : WeaponBase
        {
            var existingWeapon = (from o in Weapons where o.GetType() == typeof(T) select o).FirstOrDefault();
            return existingWeapon != null && existingWeapon.RoundQuantity > 0;
        }

        public bool FireWeapon<T>() where T : WeaponBase
        {
            var weapon = GetWeaponOfType<T>();
            return weapon?.Fire() == true;
        }

        public bool FireWeapon<T>(SiVector location) where T : WeaponBase
        {
            var weapon = GetWeaponOfType<T>();
            return weapon?.Fire(location) == true;
        }

        public WeaponBase GetWeaponOfType<T>() where T : WeaponBase
        {
            return (from o in Weapons where o.GetType() == typeof(T) select o).FirstOrDefault();
        }

        #endregion

        public override void Render(RenderTarget renderTarget)
        {
            base.Render(renderTarget);

            if (Visable)
            {
                if (_lockedOnImage != null && IsLockedOnHard)
                {
                    DrawImage(renderTarget, _lockedOnImage, 0);
                }
                else if (_lockedOnImage != null && IsLockedOnSoft)
                {
                    DrawImage(renderTarget, _lockedOnSoftImage, 0);
                }
            }
        }

        public override bool TryMunitionHit(MunitionBase munition, SiVector hitTestPosition)
        {
            if (IntersectsAABB(hitTestPosition))
            {
                Hit(munition);
                if (HullHealth <= 0)
                {
                    Explode();
                }
                return true;
            }
            return false;
        }

        public override void Explode()
        {
            _engine.Events.Add(() =>
            {
                _engine.Sprites.Animations.AddRandomExplosionAt(this);
                _engine.Sprites.Particles.ParticleBlastAt(SiRandom.Between(200, 800), this);
                _engine.Sprites.CreateFragmentsOf(this);
                _engine.Rendering.AddScreenShake(4, 800);
                _engine.Audio.PlayRandomExplosion();
            });
            base.Explode();
        }

        /// <summary>
        /// Provides a way to make decisions about the sprite that do not necessirily have anyhting to do with movement.
        /// </summary>
        /// <param name="epoch"></param>
        /// <param name="displacementVector"></param>
        public virtual void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
        }

        /// <summary>
        /// Performs collision detection for this one sprite using the passed in collection of collidable objects.
        /// 
        /// This is called before ApplyMotion().
        /// </summary>
        /// <param name="collidables">Contains all objects that have CollisionDetection enabled with their predicted new locations.</param>
        public virtual void PerformCollisionDetection(float epoch)
        {
            //HEY PAT!
            // - This function (PerformCollisionDetection) is called before ApplyMotion().
            // - collidables[] contains all objects that have CollisionDetection enabled.
            // - Each element in collidables[] has a Position property which is the location where
            //      the sprite will be AFTER the next call to ApplyMotion() (e.g. the sprite has not
            //      yet moved but this will tell you where it will be when it next moves).
            //      We should? be able to use this to detect a collision and back each of the sprites
            //      velocities off... right?
            // - Note that thisCollidable also contains the predicted location after the move.
            // - How the hell do we handle collateral collisions? Please tell me we dont have to iterate.... 
            // - Turns out a big problem is going to be that each colliding sprite will have two seperate handlers.
            //      this might make it difficult.... not sure yet.
            // - I think we need to determine the angle of the "collider" and do the bounce math on that.
            // - I added sprite mass, velocity and momemtium. This should help us determine whos gonna get moved and by what amount.

            //IsHighlighted = true;

            var thisCollidable = new PredictedSpriteRegion(this, _engine.Display.RenderWindowPosition, epoch);

            foreach (var other in _engine.Collisions.Colliadbles)
            {
                if (thisCollidable.Sprite == other.Sprite || _engine.Collisions.IsAlreadyHandled(thisCollidable.Sprite, other.Sprite))
                {
                    continue;
                }

                if (thisCollidable.IntersectsSAT(other))
                {
                    //The items added to this collection are rendered to the screen via
                    //  EngineCore.RenderEverything() when Engine.Settings.HighlightCollisions is true.
                    RespondToCollisions(
                        _engine.Collisions.Add(thisCollidable, other));
                }
            }
        }

        /// <summary>
        /// Chanegs the movement vector of two sprites involved in a collision.
        /// </summary>
        /// <param name="collision"></param>
        public void RespondToCollisions(Collision collision)
        {
            //We have to save the movement vectors because the calle to sssssss is going to change then.
            var originalSprite1Velocity = collision.Object1.Sprite.MovementVector;
            var originalSprite2Velocity = collision.Object2.Sprite.MovementVector;

            RespondToCollision(collision.Object1.Sprite, originalSprite1Velocity, collision.Object2.Sprite, originalSprite2Velocity);
            RespondToCollision(collision.Object2.Sprite, originalSprite2Velocity, collision.Object1.Sprite, originalSprite1Velocity);

        }

        /// <summary>
        /// Chanegs the movement vector of the action sprite in response to a collision with collideWithSprite.
        /// </summary>
        /// <param name="actionSprite">The sprite which will have its movement vector modified in response to the collision.</param>
        /// <param name="actionSpriteVelocity">The movement vector of the actionSprite</param>
        /// <param name="collideWithSprite">The sprite that the actionSprite is colliding with. This sprite will not be altered.</param>
        /// <param name="collideWithSpriteVelocity">The movement vector of the collideWithSprite</param>
        public void RespondToCollision(SpriteInteractiveBase actionSprite, SiVector actionSpriteVelocity,
                                        SpriteInteractiveBase collideWithSprite, SiVector collideWithSpriteVelocity)
        {
           float massA = actionSprite.Metadata.Mass;
            float massB = collideWithSprite.Metadata.Mass;

            var r = (collideWithSprite.Location - actionSprite.Location).Normalize(); // Collision normal

            var vA_prime = actionSpriteVelocity - (2 * massB / (massA + massB))
                * SiVector.Dot(actionSpriteVelocity - collideWithSpriteVelocity, r) / r.Length() * r;

            actionSprite.MovementVector = vA_prime * actionSprite.Throttle;
        }
    }
}
