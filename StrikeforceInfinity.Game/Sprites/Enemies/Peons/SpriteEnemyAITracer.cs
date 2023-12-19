using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Loudouts;
using StrikeforceInfinity.Game.Sprites.Enemies.Peons.BaseClasses;
using StrikeforceInfinity.Game.Utility;
using StrikeforceInfinity.Game.Utility.ExtensionMethods;
using StrikeforceInfinity.Game.Weapons;
using System;
using System.Drawing;
using System.IO;

namespace StrikeforceInfinity.Game.Sprites.Enemies.Peons
{
    internal class SpriteEnemyAITracer : SpriteEnemyPeonBase
    {
        public const int hullHealth = 10;
        public const int bountyMultiplier = 15;

        private const string _assetPath = @"Graphics\Enemy\AITracer\";
        private readonly int imageCount = 1;
        private readonly int selectedImageIndex = 0;

        private readonly StreamWriter _trainingDataDumpFile;

        ~SpriteEnemyAITracer()
        {
            _trainingDataDumpFile.Close();
            _trainingDataDumpFile.Dispose();
        }

        public SpriteEnemyAITracer(EngineCore gameCore)
            : base(gameCore, hullHealth, bountyMultiplier)
        {
            selectedImageIndex = HgRandom.Generator.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"), new Size(32, 32));

            string trainingDataDumpFilePath = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt";

            _trainingDataDumpFile = new StreamWriter(trainingDataDumpFilePath);

            _trainingDataDumpFile.WriteLine(
                      "Angle_To_Player,"
                    + "Distance_To_Player,"
                    + "Player_Throttle_Percentage,"
                    + "Player_Boost_Percentage,"
                    + "Player_Angle,"
                    + "Enemy_Angle,"
                    + "Enemy_Throttle_Percentage,"
                    + "Enemy_Boost_Percentage"
                );

            ShipClass = HgEnemyClass.AITracer;

            Velocity.BoostPercentage = 0;
            Velocity.ThrottlePercentage = 0;

            //Load the loadout from file or create a new one if it does not exist.
            EnemyShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new EnemyShipLoadout(ShipClass)
                {
                    Description = "→ AI Tracer ←\n"
                       + "TODO: Enemy that is controlled by a second player to train AI.\n",
                    MaxSpeed = 4.5,
                    MaxBoost = 2.5,
                    HullHealth = 20,
                    ShieldHealth = 10,
                };

                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 100000));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 100000));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponThunderstrikeMissile), 100000));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);
        }

        public override void ApplyMotion(SiPoint displacementVector)
        {
            var thrustVector = Velocity.MaxSpeed * (Velocity.ThrottlePercentage + -Velocity.RecoilPercentage);

            if (Velocity.BoostPercentage > 0)
            {
                thrustVector += Velocity.MaxBoost * Velocity.BoostPercentage;
            }

            X += Velocity.Angle.X * thrustVector - displacementVector.X;
            Y += Velocity.Angle.Y * thrustVector - displacementVector.Y;
        }

        public override void ApplyIntelligence(SiPoint displacementVector)
        {
            _trainingDataDumpFile.WriteLine(
                      $"{AngleTo(_gameCore.Player.Sprite):n4},"
                    + $"{DistanceTo(_gameCore.Player.Sprite):n4},"
                    + $"{_gameCore.Player.Sprite.Velocity.ThrottlePercentage:n4},"
                    + $"{_gameCore.Player.Sprite.Velocity.BoostPercentage:n4},"
                    + $"{_gameCore.Player.Sprite.Velocity.Angle.Degrees:42},"
                    + $"{Velocity.Angle.Degrees:n4},"
                    + $"{Velocity.ThrottlePercentage:n4},"
                    + $"{Velocity.BoostPercentage:n4}"
                );

            if (Visable)
            {

                if (_gameCore.Input.IsKeyPressed(HgPlayerKey.AltPrimaryFire))
                {
                    if (HasWeaponAndAmmo<WeaponVulcanCannon>())
                    {
                        FireWeapon<WeaponVulcanCannon>();
                    }
                }

                /*
                if (_gameCore.Input.IsKeyPressed(HgPlayerKey.AltSecondaryFire))
                {
                    if (SelectedSecondaryWeapon != null && SelectedSecondaryWeapon.Fire())
                    {
                        if (SelectedSecondaryWeapon?.RoundQuantity == 25)
                        {
                            AmmoLowSound.Play();
                        }
                        if (SelectedSecondaryWeapon?.RoundQuantity == 0)
                        {
                            AmmoEmptySound.Play();
                            SelectFirstAvailableUsableSecondaryWeapon();
                        }
                    }
                }
                */

                //Make player boost "build up" and fade-in.
                if (_gameCore.Input.IsKeyPressed(HgPlayerKey.AltSpeedBoost) && _gameCore.Input.IsKeyPressed(HgPlayerKey.AltForward)
                    && Velocity.AvailableBoost > 0 && Velocity.BoostRebuilding == false)
                {
                    if (Velocity.BoostPercentage < 1.0)
                    {
                        double boostToAdd = Velocity.BoostPercentage > 0
                            ? _gameCore.Settings.PlayerThrustRampUp * (1 - Velocity.BoostPercentage) : _gameCore.Settings.PlayerThrustRampUp;
                        Velocity.BoostPercentage += boostToAdd;
                    }

                    Velocity.AvailableBoost -= Velocity.MaxBoost * Velocity.BoostPercentage;
                    if (Velocity.AvailableBoost < 0)
                    {
                        Velocity.AvailableBoost = 0;
                    }
                }
                else
                {
                    //If no "forward" or "reverse" user input is received... then fade the boost and rebuild available boost.
                    if (Velocity.BoostPercentage > 0)
                    {
                        Velocity.BoostPercentage -= _gameCore.Settings.PlayerThrustRampDown;
                        if (Velocity.BoostPercentage < 0.01)
                        {
                            Velocity.BoostPercentage = 0;
                        }
                    }
                    if (_gameCore.Input.IsKeyPressed(HgPlayerKey.AltSpeedBoost) == false && Velocity.AvailableBoost < _gameCore.Settings.MaxPlayerBoostAmount)
                    {
                        Velocity.AvailableBoost = (Velocity.AvailableBoost + 5).Box(0, _gameCore.Settings.MaxPlayerBoostAmount);

                        if (Velocity.BoostRebuilding && Velocity.AvailableBoost >= _gameCore.Settings.PlayerBoostRebuildFloor)
                        {
                            Velocity.BoostRebuilding = false;
                        }
                    }
                }

                //Make player thrust "build up" and fade-in.
                if (_gameCore.Input.IsKeyPressed(HgPlayerKey.AltForward))
                {
                    if (Velocity.ThrottlePercentage < 1.0)
                    {
                        //We add the first thrust amount with the amount defined in "PlayerThrustRampUp", after that we add the
                        // calculated inversee-percentage-from-100-percent of "PlayerThrustRampUp" to the ThrottlePercentage until we reach 100%.
                        // This causes our thrust to start qucikly and acceleration to fade fast as we approach full throttle. The last few %-points
                        //  of throttle will take a while. We do the reverse of this to stop. Stopping fast at first and slowly-slowly slowing to a stop.

                        double thrustToAdd = Velocity.ThrottlePercentage > 0
                        ? _gameCore.Settings.PlayerThrustRampUp * (1 - Velocity.ThrottlePercentage) : _gameCore.Settings.PlayerThrustRampUp;

                        Velocity.ThrottlePercentage += thrustToAdd;
                    }
                }
                else
                {
                    //If no "forward" or "reverse" user input is received... then fade the thrust.
                    if (Velocity.ThrottlePercentage > 0)
                    {
                        //Ramp down to a stop:
                        double thrustToRemove = Velocity.ThrottlePercentage < 1
                        ? _gameCore.Settings.PlayerThrustRampDown * Velocity.ThrottlePercentage : _gameCore.Settings.PlayerThrustRampDown;
                        Velocity.ThrottlePercentage -= thrustToRemove;

                        if (Velocity.ThrottlePercentage < 0.01)
                        {
                            //Dont overshoot the stop.
                            Velocity.ThrottlePercentage = 0;
                        }
                    }
                    else if (Velocity.ThrottlePercentage < 0)
                    {
                        //Ramp up to a stop:
                        double thrustToRemove = Velocity.ThrottlePercentage * -1 < 1
                        ? _gameCore.Settings.PlayerThrustRampDown * (1 - Velocity.ThrottlePercentage * -1) : _gameCore.Settings.PlayerThrustRampDown;

                        Velocity.ThrottlePercentage += thrustToRemove;
                        if (Velocity.ThrottlePercentage > 0)
                        {
                            //Dont overshoot the stop.
                            Velocity.ThrottlePercentage = 0;
                        }
                    }
                }

                if (BoostAnimation != null)
                {
                    BoostAnimation.Visable =
                        _gameCore.Input.IsKeyPressed(HgPlayerKey.AltSpeedBoost)
                        && _gameCore.Input.IsKeyPressed(HgPlayerKey.AltForward)
                    && Velocity.AvailableBoost > 0 && Velocity.BoostRebuilding == false;
                }

                if (Velocity.AvailableBoost <= 0)
                {
                    Velocity.AvailableBoost = 0;
                    Velocity.BoostRebuilding = true;
                }

                if (ThrustAnimation != null)
                {
                    ThrustAnimation.Visable = _gameCore.Input.IsKeyPressed(HgPlayerKey.AltForward);
                }

                //We are going to restrict the rotation speed to a percentage of thrust.
                var rotationSpeed = _gameCore.Settings.MaxPlayerRotationSpeedDegrees * Velocity.ThrottlePercentage;

                if (_gameCore.Input.IsKeyPressed(HgPlayerKey.AltRotateCounterClockwise))
                {
                    Rotate(-(rotationSpeed > 1.0 ? rotationSpeed : 1.0));
                }
                else if (_gameCore.Input.IsKeyPressed(HgPlayerKey.AltRotateClockwise))
                {
                    Rotate(rotationSpeed > 1.0 ? rotationSpeed : 1.0);
                }
            }

            if (Velocity.RecoilPercentage > 0)
            {
                Velocity.RecoilPercentage -= Velocity.RecoilPercentage * 0.10;
                if (Velocity.RecoilPercentage < 0.001)
                {
                    Velocity.RecoilPercentage = 0;
                }
            }

            RenewableResources.RenewAllResources();
        }
    }
}
