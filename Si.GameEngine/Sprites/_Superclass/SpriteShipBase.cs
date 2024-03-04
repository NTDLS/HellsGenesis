using Si.GameEngine.Sprites.Weapons._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using Si.Library.Mathematics.Geometry;
using Si.Rendering;
using System;
using System.Collections.Generic;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Sprites._Superclass
{
    /// <summary>
    /// The ship base is a ship object that moves, can be hit, explodes and can be the subject of locking weapons.
    /// </summary>
    public class SpriteShipBase : SpriteBase
    {
        private readonly Dictionary<string, WeaponBase> _droneWeaponsCache = new();

        public SpriteRadarPositionIndicator RadarPositionIndicator { get; protected set; }
        public SpriteRadarPositionTextBlock RadarPositionText { get; protected set; }
        public SiTimeRenewableResources RenewableResources { get; set; } = new();

        private readonly string _assetPathlockedOnImage = @"Graphics\Weapon\Locked On.png";
        private readonly string _assetPathlockedOnSoftImage = @"Graphics\Weapon\Locked Soft.png";

        public SpriteShipBase(GameEngineCore gameEngine, string name = "")
            : base(gameEngine, name)
        {
            if (IsDrone)
            {
                BuildDroneWeaponsCache();
            }

            _gameEngine = gameEngine;
        }

        public override void Initialize(string imagePath = null)
        {

            _lockedOnImage = _gameEngine.Assets.GetBitmap(_assetPathlockedOnImage);
            _lockedOnSoftImage = _gameEngine.Assets.GetBitmap(_assetPathlockedOnSoftImage);

            base.Initialize(imagePath);
        }

        /// <summary>
        /// Fires a drone weapon (a weapon without ammo limits).
        /// </summary>
        /// <param name="weaponTypeName"></param>
        /// <returns></returns>
        public bool FireDroneWeapon(string weaponTypeName)
        {
            return GetDroneWeaponByTypeName(weaponTypeName)?.Fire() == true;
        }

        /// <summary>
        /// Builds the cache of all weapons so the drone can fire quickly.
        /// </summary>
        private void BuildDroneWeaponsCache()
        {
            var allWeapons = SiReflection.GetSubClassesOf<WeaponBase>();

            foreach (var weapon in allWeapons)
            {
                _ = GetDroneWeaponByTypeName(weapon.Name);
            }
        }

        /// <summary>
        /// Gets a cached drone weapon (a weapon without ammo limits).
        /// </summary>
        /// <param name="weaponTypeName"></param>
        /// <returns></returns>
        private WeaponBase GetDroneWeaponByTypeName(string weaponTypeName)
        {
            if (_droneWeaponsCache.TryGetValue(weaponTypeName, out var weapon))
            {
                return weapon;
            }

            var weaponType = SiReflection.GetTypeByName(weaponTypeName);
            weapon = SiReflection.CreateInstanceFromType<WeaponBase>(weaponType, new object[] { _gameEngine, this });

            _droneWeaponsCache.Add(weaponTypeName, weapon);

            return weapon;
        }

        public override void Explode()
        {
            _gameEngine.Sprites.Animations.AddRandomExplosionAt(this);
            _gameEngine.Sprites.Particles.ParticleBlastAt(SiRandom.Between(200, 800), this);
            CreateExplosionFragments();
            _gameEngine.Rendering.AddScreenShake(4, 800);
            _gameEngine.Audio.PlayRandomExplosion();
            base.Explode();
        }

        public void CreateExplosionFragments()
        {
            var image = GetImage();
            if (image == null)
            {
                return;
            }

            int fragmentCount = SiRandom.Between(2, 10);

            var fragmentImages = _gameEngine.Rendering.GenerateIrregularFragments(image, fragmentCount);

            for (int index = 0; index < fragmentCount; index++)
            {
                var fragment = _gameEngine.Sprites.GenericSprites.CreateAt(this, fragmentImages[index]);
                //TODO: Can we implement this.
                fragment.CleanupMode = ParticleCleanupMode.DistanceOffScreen;
                fragment.FadeToBlackReductionAmount = SiRandom.Between(0.001f, 0.01f);

                fragment.Velocity.Angle.Degrees = SiRandom.Between(0.0f, 359.0f);
                fragment.Velocity.Speed = SiRandom.Between(1, 3.5f);
                fragment.Velocity.ThrottlePercentage = 1;
                fragment.VectorType = ParticleVectorType.Independent;
            }
        }

        public void CreateParticlesExplosion()
        {
            _gameEngine.Sprites.Particles.CreateAt(this, SiRenderingUtility.GetRandomHotColor(), SiRandom.Between(30, 50));
            _gameEngine.Audio.PlayRandomExplosion();
        }

        public void FixRadarPositionIndicator()
        {
            if (RadarPositionIndicator != null)
            {
                if (_gameEngine.Display.GetCurrentScaledScreenBounds().IntersectsWith(RenderBounds, -50) == false)
                {
                    RadarPositionText.DistanceValue = Math.Abs(DistanceTo(_gameEngine.Player.Sprite));

                    RadarPositionText.Visable = true;
                    RadarPositionText.IsFixedPosition = true;
                    RadarPositionIndicator.Visable = true;
                    RadarPositionIndicator.IsFixedPosition = true;

                    float requiredAngleRadians = _gameEngine.Player.Sprite.AngleToRadians(this);

                    RadarPositionIndicator.Location = _gameEngine.Display.CenterScreen
                        + SiPoint.PointFromAngleAtDistance360(new SiAngle(requiredAngleRadians), new SiPoint(200, 200));
                    RadarPositionIndicator.Velocity.Angle.Radians = requiredAngleRadians;

                    RadarPositionText.Location = _gameEngine.Display.CenterScreen
                        + SiPoint.PointFromAngleAtDistance360(new SiAngle(requiredAngleRadians), new SiPoint(120, 120));
                    RadarPositionIndicator.Velocity.Angle.Radians = requiredAngleRadians;
                }
                else
                {
                    RadarPositionText.Visable = false;
                    RadarPositionIndicator.Visable = false;
                }
            }
        }

        public override void Cleanup()
        {
            if (RadarPositionIndicator != null)
            {
                RadarPositionIndicator.QueueForDelete();
                RadarPositionText.QueueForDelete();
            }

            base.Cleanup();
        }
    }
}
