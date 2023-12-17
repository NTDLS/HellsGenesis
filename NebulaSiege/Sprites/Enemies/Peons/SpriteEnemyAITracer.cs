﻿using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Loudouts;
using NebulaSiege.Sprites.Enemies.Peons.BaseClasses;
using NebulaSiege.Utility;
using NebulaSiege.Utility.ExtensionMethods;
using NebulaSiege.Weapons;
using System.Drawing;
using System.IO;

namespace NebulaSiege.Sprites.Enemies.Peons
{
    internal class SpriteEnemyAITracer : SpriteEnemyPeonBase
    {
        public const int hullHealth = 10;
        public const int bountyMultiplier = 15;

        private const string _assetPath = @"Graphics\Enemy\AITracer\";
        private readonly int imageCount = 1;
        private readonly int selectedImageIndex = 0;

        public SpriteEnemyAITracer(EngineCore core)
            : base(core, hullHealth, bountyMultiplier)
        {
            selectedImageIndex = HgRandom.Generator.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"), new Size(32, 32));

            ShipClass = HgEnemyClass.Phoenix;

            this.Velocity.BoostPercentage = 0;
            this.Velocity.ThrottlePercentage = 0;

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

                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 5000));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 42));
                loadout.Weapons.Add(new ShipLoadoutWeapon(typeof(WeaponThunderstrikeMissile), 16));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);
        }

        #region Artificial Intelligence.

        public override void ApplyMotion(NsPoint displacementVector)
        {
            X += Velocity.Angle.X * (Velocity.MaxSpeed * Velocity.ThrottlePercentage) - displacementVector.X;
            Y += Velocity.Angle.Y * (Velocity.MaxSpeed * Velocity.ThrottlePercentage) - displacementVector.Y;

            //base.UpdateThrustAnimationPositions();
        }

        public override void ApplyIntelligence(NsPoint displacementVector)
        {
            if (Visable)
            {
                /*
                  if (_core.Input.IsKeyPressed(HgPlayerKey.AltPrimaryFire))
                  {
                      if (PrimaryWeapon != null && PrimaryWeapon.Fire())
                      {
                          if (PrimaryWeapon?.RoundQuantity == 25)
                          {
                              AmmoLowSound.Play();
                          }
                          if (PrimaryWeapon?.RoundQuantity == 0)
                          {
                              AmmoEmptySound.Play();
                          }
                      }
                  }

                  if (_core.Input.IsKeyPressed(HgPlayerKey.AltSecondaryFire))
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
                if (_core.Input.IsKeyPressed(HgPlayerKey.AltSpeedBoost) && _core.Input.IsKeyPressed(HgPlayerKey.AltForward)
                    && Velocity.AvailableBoost > 0 && Velocity.BoostRebuilding == false)
                {
                    if (Velocity.BoostPercentage < 1.0)
                    {
                        double boostToAdd = Velocity.BoostPercentage > 0
                            ? _core.Settings.PlayerThrustRampUp * (1 - Velocity.BoostPercentage) : _core.Settings.PlayerThrustRampUp;
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
                        Velocity.BoostPercentage -= _core.Settings.PlayerThrustRampDown;
                        if (Velocity.BoostPercentage < 0.01)
                        {
                            Velocity.BoostPercentage = 0;
                        }
                    }
                    if (_core.Input.IsKeyPressed(HgPlayerKey.AltSpeedBoost) == false && Velocity.AvailableBoost < _core.Settings.MaxPlayerBoostAmount)
                    {
                        Velocity.AvailableBoost = (Velocity.AvailableBoost + 5).Box(0, _core.Settings.MaxPlayerBoostAmount);

                        if (Velocity.BoostRebuilding && Velocity.AvailableBoost >= _core.Settings.PlayerBoostRebuildFloor)
                        {
                            Velocity.BoostRebuilding = false;
                        }
                    }
                }

                //Make player thrust "build up" and fade-in.
                if (_core.Input.IsKeyPressed(HgPlayerKey.AltForward))
                {
                    if (Velocity.ThrottlePercentage < 1.0)
                    {
                        //We add the first thrust amount with the amount defined in "PlayerThrustRampUp", after that we add the
                        // calculated inversee-percentage-from-100-percent of "PlayerThrustRampUp" to the ThrottlePercentage until we reach 100%.
                        // This causes our thrust to start qucikly and acceleration to fade fast as we approach full throttle. The last few %-points
                        //  of throttle will take a while. We do the reverse of this to stop. Stopping fast at first and slowly-slowly slowing to a stop.

                        double thrustToAdd = Velocity.ThrottlePercentage > 0
                        ? _core.Settings.PlayerThrustRampUp * (1 - Velocity.ThrottlePercentage) : _core.Settings.PlayerThrustRampUp;

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
                        ? _core.Settings.PlayerThrustRampDown * Velocity.ThrottlePercentage : _core.Settings.PlayerThrustRampDown;
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
                        ? _core.Settings.PlayerThrustRampDown * (1 - Velocity.ThrottlePercentage * -1) : _core.Settings.PlayerThrustRampDown;

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
                        _core.Input.IsKeyPressed(HgPlayerKey.AltSpeedBoost)
                        && _core.Input.IsKeyPressed(HgPlayerKey.AltForward)
                    && Velocity.AvailableBoost > 0 && Velocity.BoostRebuilding == false;
                }

                if (Velocity.AvailableBoost <= 0)
                {
                    Velocity.AvailableBoost = 0;
                    Velocity.BoostRebuilding = true;
                }

                if (ThrustAnimation != null)
                {
                    ThrustAnimation.Visable = _core.Input.IsKeyPressed(HgPlayerKey.AltForward);
                }

                //We are going to restrict the rotation speed to a percentage of thrust.
                var rotationSpeed = _core.Settings.MaxPlayerRotationSpeedDegrees * Velocity.ThrottlePercentage;

                if (_core.Input.IsKeyPressed(HgPlayerKey.AltRotateCounterClockwise))
                {
                    Rotate(-(rotationSpeed > 1.0 ? rotationSpeed : 1.0));
                }
                else if (_core.Input.IsKeyPressed(HgPlayerKey.AltRotateClockwise))
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
        #endregion
    }
}
