using Si.Engine.Loudout;
using Si.Engine.Sprite.Enemy.Starbase._Superclass;
using Si.Engine.Sprite.Weapon;
using Si.Library.Mathematics.Geometry;
using System.Collections.Generic;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.Enemy.Starbase
{
    internal class SpriteEnemyStarbaseGarrison : SpriteEnemyStarbase
    {
        /* Other Names:
            Nexus
            Forge
            Bastion
            Citadel
            Spire
            Stronghold
            Enclave
            Garrison
            Fortress
        */

        SiPoint[] _turrentLocations =
        [
            new(583, 147),
            new(148, 419),
            new(1018, 415),
            new(146, 913),
            new(581, 1173),
            new(1016, 909),
        ];

        class Turret
        {
            public SiPoint BaseLocation { get; set; }
            public SpriteAttachment Sprite { get; set; }

            public Turret(SpriteAttachment sprite, SiPoint baseLocation)
            {
                BaseLocation = baseLocation;
                Sprite = sprite;
            }
        }

        private readonly List<Turret> _turrets = new();

        string _assetPath;

        public SpriteEnemyStarbaseGarrison(EngineCore engine)
            : base(engine)
        {
            ShipClass = SiEnemyClass.Garrison;
            _assetPath = @$"Graphics\Enemy\Starbase\{ShipClass}";
            SetImage(@$"{_assetPath}\Hull.png");

            //Load the loadout from file or create a new one if it does not exist.
            EnemyShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new EnemyShipLoadout(ShipClass)
                {
                    Description = "→ Garrison ←\n"
                       + "TODO: Add a description\n",
                    Speed = 0.0f,
                    Boost = 0.0f,
                    HullHealth = 200,
                    ShieldHealth = 100,
                    Bounty = 50
                };

                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), int.MaxValue));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), int.MaxValue));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponThunderstrikeMissile), int.MaxValue));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);

            foreach (var turrentLocation in _turrentLocations)
            {
                var attachment = Attach($@"{_assetPath}\Turret.png", true, 3);
                _turrets.Add(new Turret(attachment, turrentLocation));
            }

            Velocity.Angle.Degrees = 0;
        }


        public override void ApplyMotion(float epoch, SiPoint displacementVector)
        {
            Velocity.Angle.Degrees += 0.05f;

            foreach (var turret in _turrets)
            {
                // Since the turret.BaseLocation is relative to the top-left corner of the base sprite, we need
                // to get the position relative to the center of the base sprite image so that we can rotate around that.
                var turretOffset = turret.BaseLocation - (Size / 2.0f);

                // Apply the rotated offsets to get the new turret location relative to the base sprite center.
                turret.Sprite.Location = Location + turretOffset.Rotate(Velocity.Angle.Radians);

                //Point the turret at the player.
                turret.Sprite.Velocity.Angle.Degrees = turret.Sprite.AngleTo360(_engine.Player.Sprite);
            }

            base.ApplyMotion(epoch, displacementVector);
        }
    }
}
