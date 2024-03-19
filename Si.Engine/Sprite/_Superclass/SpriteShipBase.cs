using Si.Engine.Sprite.Weapon._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;
using System;
using System.Collections.Generic;

namespace Si.Engine.Sprite._Superclass
{
    /// <summary>
    /// The ship base is a ship object that moves, can be hit, explodes and can be the subject of locking weapons.
    /// </summary>
    public class SpriteShipBase : SpriteInteractiveBase
    {
        private readonly Dictionary<string, WeaponBase> _droneWeaponsCache = new();
        public SpriteRadarPositionIndicator RadarPositionIndicator { get; protected set; }
        public SpriteRadarPositionTextBlock RadarPositionText { get; protected set; }

        private readonly string _assetPathlockedOnImage = @"Sprites\Weapon\Locked On.png";
        private readonly string _assetPathlockedOnSoftImage = @"Sprites\Weapon\Locked Soft.png";

        public SpriteShipBase(EngineCore engine, string name = "")
            : base(engine, name)
        {
            _engine = engine;

            _lockedOnImage = _engine.Assets.GetBitmap(_assetPathlockedOnImage);
            _lockedOnSoftImage = _engine.Assets.GetBitmap(_assetPathlockedOnSoftImage);
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
            weapon = SiReflection.CreateInstanceFromType<WeaponBase>(weaponType, new object[] { _engine, this });

            _droneWeaponsCache.Add(weaponTypeName, weapon);

            return weapon;
        }

        public void FixRadarPositionIndicator()
        {
            if (RadarPositionIndicator != null)
            {
                if (_engine.Display.GetCurrentScaledScreenBounds().IntersectsWith(RenderBounds, -50) == false)
                {
                    RadarPositionText.DistanceValue = Math.Abs(DistanceTo(_engine.Player.Sprite));

                    RadarPositionText.Visable = true;
                    RadarPositionText.IsFixedPosition = true;
                    RadarPositionIndicator.Visable = true;
                    RadarPositionIndicator.IsFixedPosition = true;

                    float requiredAngleRadians = _engine.Player.Sprite.AngleToRadians(this);

                    RadarPositionIndicator.Location = _engine.Display.CenterCanvas
                        + SiPoint.PointFromAngleAtDistance360(new SiAngle(requiredAngleRadians), new SiPoint(200, 200));
                    RadarPositionIndicator.Velocity.ForwardAngle.Radians = requiredAngleRadians;

                    RadarPositionText.Location = _engine.Display.CenterCanvas
                        + SiPoint.PointFromAngleAtDistance360(new SiAngle(requiredAngleRadians), new SiPoint(120, 120));
                    RadarPositionIndicator.Velocity.ForwardAngle.Radians = requiredAngleRadians;
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
