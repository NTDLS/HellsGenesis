using HG.Controller.Interfaces;
using HG.Engine;
using HG.Engine.Types.Geometry;
using HG.Sprites;
using HG.Utility.ExtensionMethods;

namespace HG.Controller
{
    internal class PlayerTickController : IVectorGeneratorTickController<SpritePlayer>
    {
        public SpritePlayer Sprite { get; set; }
        private readonly EngineCore _core;

        public PlayerTickController(EngineCore core)
        {
            _core = core;
        }

        private bool _allowLockPlayerAngleToNearbyEnemy = true;

        /// <summary>
        /// Moves the player taking into account any inputs and returns a X,Y describing the amount and direction of movement.
        /// </summary>
        /// <returns></returns>
        public HgPoint ExecuteWorldClockTick()
        {
            var displacementVector = new HgPoint();

            if (Sprite.Visable)
            {
                if (_core.Input.IsKeyPressed(HgPlayerKey.PrimaryFire))
                {
                    if (Sprite.PrimaryWeapon != null && Sprite.PrimaryWeapon.Fire())
                    {
                        if (Sprite.PrimaryWeapon?.RoundQuantity == 25)
                        {
                            Sprite.AmmoLowSound.Play();
                        }
                        if (Sprite.PrimaryWeapon?.RoundQuantity == 0)
                        {
                            Sprite.AmmoEmptySound.Play();
                        }
                    }
                }

                if (_core.Input.IsKeyPressed(HgPlayerKey.SecondaryFire))
                {
                    if (Sprite.SelectedSecondaryWeapon != null && Sprite.SelectedSecondaryWeapon.Fire())
                    {
                        if (Sprite.SelectedSecondaryWeapon?.RoundQuantity == 25)
                        {
                            Sprite.AmmoLowSound.Play();
                        }
                        if (Sprite.SelectedSecondaryWeapon?.RoundQuantity == 0)
                        {
                            Sprite.AmmoEmptySound.Play();
                            Sprite.SelectFirstAvailableUsableSecondaryWeapon();
                        }
                    }
                }

                if (_core.Settings.LockPlayerAngleToNearbyEnemy)
                {
                    #region //This needs some work. It works, but its odd - the movement is rigid.
                    if (_core.Input.IsKeyPressed(HgPlayerKey.RotateClockwise) == false && _core.Input.IsKeyPressed(HgPlayerKey.RotateCounterClockwise) == false)
                    {
                        if (_allowLockPlayerAngleToNearbyEnemy)
                        {
                            var enemies = _core.Sprites.Enemies.Visible();
                            foreach (var enemy in enemies)
                            {
                                var distanceTo = Sprite.DistanceTo(enemy);
                                if (distanceTo < 500)
                                {
                                    if (Sprite.IsPointingAt(enemy, 50))
                                    {
                                        if (enemy.IsPointingAway(Sprite, 50))
                                        {
                                            var angleTo = Sprite.AngleTo(enemy);
                                            Sprite.Velocity.Angle.Degrees = angleTo;
                                        }
                                    }
                                }
                            }
                            _allowLockPlayerAngleToNearbyEnemy = false;
                        }
                    }
                    else
                    {
                        _allowLockPlayerAngleToNearbyEnemy = true;
                    }
                    #endregion
                }

                //Make player boost "build up" and fade-in.
                if (_core.Input.IsKeyPressed(HgPlayerKey.SpeedBoost) && _core.Input.IsKeyPressed(HgPlayerKey.Forward)
                    && Sprite.Velocity.AvailableBoost > 0 && Sprite.Velocity.BoostRebuilding == false)
                {
                    if (Sprite.Velocity.BoostPercentage < 1.0)
                    {
                        double boostToAdd = Sprite.Velocity.BoostPercentage > 0
                            ? _core.Settings.PlayerThrustRampUp * (1 - Sprite.Velocity.BoostPercentage) : _core.Settings.PlayerThrustRampUp;

                        Sprite.Velocity.BoostPercentage += boostToAdd;
                    }

                    Sprite.Velocity.AvailableBoost -= Sprite.Velocity.MaxBoost * Sprite.Velocity.BoostPercentage;
                    if (Sprite.Velocity.AvailableBoost < 0)
                    {
                        Sprite.Velocity.AvailableBoost = 0;
                    }
                }
                else
                {
                    //If no "forward" or "reverse" user input is received... then fade the boost and rebuild available boost.
                    if (Sprite.Velocity.BoostPercentage > 0)
                    {
                        Sprite.Velocity.BoostPercentage -= _core.Settings.PlayerThrustRampDown;
                        if (Sprite.Velocity.BoostPercentage < 0.01)
                        {
                            Sprite.Velocity.BoostPercentage = 0;
                        }
                    }

                    if (_core.Input.IsKeyPressed(HgPlayerKey.SpeedBoost) == false && Sprite.Velocity.AvailableBoost < _core.Settings.MaxPlayerBoost)
                    {
                        Sprite.Velocity.AvailableBoost = (Sprite.Velocity.AvailableBoost + 5).Box(0, _core.Settings.MaxPlayerBoost);

                        if (Sprite.Velocity.BoostRebuilding && Sprite.Velocity.AvailableBoost >= _core.Settings.PlayerBoostRebuildMin)
                        {
                            Sprite.Velocity.BoostRebuilding = false;
                        }
                    }
                }

                //Make player thrust "build up" and fade-in.
                if (_core.Input.IsKeyPressed(HgPlayerKey.Forward))
                {
                    if (Sprite.Velocity.ThrottlePercentage < 1.0)
                    {
                        //We add the first thrust amount with the amount defined in "PlayerThrustRampUp", after that we add the
                        // calculated inversee-percentage-from-100-percent of "PlayerThrustRampUp" to the ThrottlePercentage until we reach 100%.
                        // This causes our thrust to start qucikly and acceleration to fade fast as we approach full throttle. The last few %-points
                        //  of throttle will take a while. We do the reverse of this to stop. Stopping fast at first and slowly-slowly slowing to a stop.

                        double thrustToAdd = Sprite.Velocity.ThrottlePercentage > 0
                            ? _core.Settings.PlayerThrustRampUp * (1 - Sprite.Velocity.ThrottlePercentage) : _core.Settings.PlayerThrustRampUp;

                        Sprite.Velocity.ThrottlePercentage += thrustToAdd;
                    }
                }
                else
                {
                    //If no "forward" or "reverse" user input is received... then fade the thrust.
                    if (Sprite.Velocity.ThrottlePercentage > 0)
                    {
                        //Ramp down to a stop:
                        double thrustToRemove = Sprite.Velocity.ThrottlePercentage < 1
                            ? _core.Settings.PlayerThrustRampDown * Sprite.Velocity.ThrottlePercentage : _core.Settings.PlayerThrustRampDown;

                        Sprite.Velocity.ThrottlePercentage -= thrustToRemove;

                        if (Sprite.Velocity.ThrottlePercentage < 0.01)
                        {
                            //Dont overshoot the stop.
                            Sprite.Velocity.ThrottlePercentage = 0;
                        }
                    }
                    else if (Sprite.Velocity.ThrottlePercentage < 0)
                    {
                        //Ramp up to a stop:
                        double thrustToRemove = Sprite.Velocity.ThrottlePercentage * -1 < 1
                            ? _core.Settings.PlayerThrustRampDown * (1 - Sprite.Velocity.ThrottlePercentage * -1) : _core.Settings.PlayerThrustRampDown;

                        Sprite.Velocity.ThrottlePercentage += thrustToRemove;
                        if (Sprite.Velocity.ThrottlePercentage > 0)
                        {
                            //Dont overshoot the stop.
                            Sprite.Velocity.ThrottlePercentage = 0;
                        }
                    }
                }

                if (Sprite.BoostAnimation != null)
                {
                    Sprite.BoostAnimation.Visable =
                        _core.Input.IsKeyPressed(HgPlayerKey.SpeedBoost)
                        && _core.Input.IsKeyPressed(HgPlayerKey.Forward)
                        && Sprite.Velocity.AvailableBoost > 0 && Sprite.Velocity.BoostRebuilding == false;
                }

                if (Sprite.Velocity.AvailableBoost <= 0)
                {
                    Sprite.Velocity.AvailableBoost = 0;
                    Sprite.Velocity.BoostRebuilding = true;
                }

                if (Sprite.ThrustAnimation != null)
                {
                    Sprite.ThrustAnimation.Visable = _core.Input.IsKeyPressed(HgPlayerKey.Forward);
                }

                var thrustVector = Sprite.Velocity.MaxSpeed * (Sprite.Velocity.ThrottlePercentage + -Sprite.Velocity.RecoilPercentage);

                if (Sprite.Velocity.BoostPercentage > 0)
                {
                    thrustVector += Sprite.Velocity.MaxBoost * Sprite.Velocity.BoostPercentage;
                }

                displacementVector.X += Sprite.Velocity.Angle.X * thrustVector;
                displacementVector.Y += Sprite.Velocity.Angle.Y * thrustVector;

                if (Sprite.Velocity.BoostPercentage > 0.1)
                {
                    Sprite.ShipEngineBoostSound.Play();
                }
                else
                {
                    Sprite.ShipEngineBoostSound.Fade();
                }

                if (Sprite.Velocity.ThrottlePercentage > 0.1)
                {
                    Sprite.ShipEngineRoarSound.Play();
                }
                else
                {
                    Sprite.ShipEngineRoarSound.Fade();
                }

                //We are going to restrict the rotation speed to a percentage of thrust.
                var rotationSpeed = _core.Settings.MaxPlayerRotationSpeedDegrees * Sprite.Velocity.ThrottlePercentage;

                if (_core.Input.IsKeyPressed(HgPlayerKey.RotateCounterClockwise))
                {
                    Sprite.Rotate(-(rotationSpeed > 1.0 ? rotationSpeed : 1.0));
                }
                else if (_core.Input.IsKeyPressed(HgPlayerKey.RotateClockwise))
                {
                    Sprite.Rotate(rotationSpeed > 1.0 ? rotationSpeed : 1.0);
                }
            }

            //Scroll the background.
            _core.Display.BackgroundOffset.X += displacementVector.X;
            _core.Display.BackgroundOffset.Y += displacementVector.Y;

            _core.Display.CurrentQuadrant = _core.Display.GetQuadrant(
                Sprite.X + _core.Display.BackgroundOffset.X,
                Sprite.Y + _core.Display.BackgroundOffset.Y);

            if (Sprite.Velocity.RecoilPercentage > 0)
            {
                Sprite.Velocity.RecoilPercentage -= Sprite.Velocity.RecoilPercentage * 0.01;
                if (Sprite.Velocity.RecoilPercentage < 0.01)
                {
                    Sprite.Velocity.RecoilPercentage = 0;
                }
            }

            Sprite.RenewableResources.RenewAllResources();

            return displacementVector;
        }

        public void ResetAndShow()
        {
            Sprite.Reset();

            _core.Sprites.RenderRadar = true;
            Sprite.Visable = true;
            Sprite.ShipEngineIdleSound.Play();
            Sprite.AllSystemsGoSound.Play();
        }

        public void Hide()
        {
            Sprite.Visable = false;
            _core.Sprites.RenderRadar = false;
            Sprite.ShipEngineIdleSound.Stop();
            Sprite.ShipEngineRoarSound.Stop();
        }
    }
}
