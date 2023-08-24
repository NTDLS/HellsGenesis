using HG.Actors.Objects;
using HG.Types;

namespace HG.Engine.Managers
{
    internal class EnginePlayerManager
    {
        public ActorPlayer Actor { get; set; }
        private readonly Core _core;

        public EnginePlayerManager(Core core)
        {
            _core = core;
        }

        public HGPoint<double> ExecuteWorldClockTick()
        {
            var appliedOffset = new HGPoint<double>();

            if (Actor.Visable)
            {
                if (_core.Input.IsKeyPressed(HGPlayerKey.PrimaryFire))
                {
                    if (Actor.SelectedPrimaryWeapon != null && Actor.SelectedPrimaryWeapon.Fire())
                    {
                        if (Actor.SelectedPrimaryWeapon?.RoundQuantity == 25)
                        {
                            Actor.AmmoLowSound.Play();
                        }
                        if (Actor.SelectedPrimaryWeapon?.RoundQuantity == 0)
                        {
                            Actor.AmmoEmptySound.Play();
                            Actor.SelectFirstAvailableUsablePrimaryWeapon();
                        }
                    }
                }

                if (_core.Input.IsKeyPressed(HGPlayerKey.SecondaryFire))
                {
                    if (Actor.SelectedSecondaryWeapon != null && Actor.SelectedSecondaryWeapon.Fire())
                    {
                        if (Actor.SelectedSecondaryWeapon?.RoundQuantity == 25)
                        {
                            Actor.AmmoLowSound.Play();
                        }
                        if (Actor.SelectedSecondaryWeapon?.RoundQuantity == 0)
                        {
                            Actor.AmmoEmptySound.Play();
                            Actor.SelectFirstAvailableUsableSecondaryWeapon();
                        }
                    }
                }

                //Make player boost "build up" and fade-in.
                if (_core.Input.IsKeyPressed(HGPlayerKey.SpeedBoost) && _core.Input.IsKeyPressed(HGPlayerKey.Forward)
                    && Actor.Velocity.AvailableBoost > 0 && Actor.IsBoostFading == false)
                {
                    if (Actor.Velocity.BoostPercentage < 1.0)
                    {
                        Actor.Velocity.BoostPercentage += _core.Settings.PlayerThrustRampUp;
                    }

                    Actor.Velocity.AvailableBoost -= Actor.Velocity.MaxBoost * Actor.Velocity.BoostPercentage;
                    if (Actor.Velocity.AvailableBoost < 0)
                    {
                        Actor.Velocity.AvailableBoost = 0;
                    }
                }
                else
                {
                    if (Actor.Velocity.AvailableBoost == 0)
                    {
                        //The boost was all used up, now we have to wait on it to cool down.
                        Actor.IsBoostFading = true;
                    }

                    //If no "forward" or "reverse" user input is received... then fade the boost and rebuild available boost.
                    if (Actor.Velocity.BoostPercentage > _core.Settings.MinPlayerThrust)
                    {
                        Actor.Velocity.BoostPercentage -= _core.Settings.PlayerThrustRampDown;
                        if (Actor.Velocity.BoostPercentage < 0)
                        {
                            Actor.Velocity.BoostPercentage = 0;
                        }
                    }

                    if (Actor.Velocity.AvailableBoost < _core.Settings.MaxPlayerBoost)
                    {
                        Actor.Velocity.AvailableBoost += 1 - Actor.Velocity.BoostPercentage;
                    }
                    else
                    {
                        Actor.IsBoostFading = false;
                    }
                }

                if (Actor.BoostAnimation != null)
                {
                    Actor.BoostAnimation.Visable = (_core.Input.IsKeyPressed(HGPlayerKey.SpeedBoost)
                        && _core.Input.IsKeyPressed(HGPlayerKey.Forward)
                        && Actor.Velocity.AvailableBoost > 0
                        && Actor.IsBoostFading == false);
                }

                if (Actor.ThrustAnimation != null)
                {
                    Actor.ThrustAnimation.Visable = Actor.Velocity.ThrottlePercentage > 0;
                }

                //Make player thrust "build up" and fade-in.
                if (_core.Input.IsKeyPressed(HGPlayerKey.Forward))
                {
                    if (Actor.Velocity.ThrottlePercentage < 1.0)
                    {
                        Actor.Velocity.ThrottlePercentage += _core.Settings.PlayerThrustRampUp;
                    }
                }
                else
                {
                    //If no "forward" or "reverse" user input is received... then fade the thrust.
                    if (Actor.Velocity.ThrottlePercentage > _core.Settings.MinPlayerThrust)
                    {
                        Actor.Velocity.ThrottlePercentage -= _core.Settings.PlayerThrustRampDown;
                        if (Actor.Velocity.ThrottlePercentage < 0)
                        {
                            Actor.Velocity.ThrottlePercentage = 0;
                        }
                    }
                }

                if (Actor.Velocity.ThrottlePercentage > 0)
                {
                    double forwardThrust = (Actor.Velocity.MaxSpeed * Actor.Velocity.ThrottlePercentage);

                    if (Actor.Velocity.BoostPercentage > 0)
                    {
                        forwardThrust += Actor.Velocity.MaxBoost * Actor.Velocity.BoostPercentage;
                    }

                    //Close to the right wall and travelling in that direction.
                    if (Actor.X > _core.Display.NatrualScreenBounds.X + _core.Display.NatrualScreenBounds.Width - _core.Settings.InfiniteScrollWallX
                        && Actor.Velocity.Angle.X > 0)
                    {
                        appliedOffset.X = (Actor.Velocity.Angle.X * forwardThrust);
                    }

                    //Close to the bottom wall and travelling in that direction.
                    if (Actor.Y > _core.Display.NatrualScreenBounds.Y + _core.Display.NatrualScreenBounds.Height - _core.Settings.InfiniteScrollWallY
                        && Actor.Velocity.Angle.Y > 0)
                    {
                        appliedOffset.Y = (Actor.Velocity.Angle.Y * forwardThrust);
                    }

                    //Close to the left wall and travelling in that direction.
                    if (Actor.X < _core.Display.NatrualScreenBounds.X + _core.Settings.InfiniteScrollWallX
                        && Actor.Velocity.Angle.X < 0)
                    {
                        appliedOffset.X = (Actor.Velocity.Angle.X * forwardThrust);
                    }

                    //Close to the top wall and travelling in that direction.
                    if (Actor.Y < _core.Display.NatrualScreenBounds.Y + _core.Settings.InfiniteScrollWallY
                        && Actor.Velocity.Angle.Y < 0)
                    {
                        appliedOffset.Y = (Actor.Velocity.Angle.Y * forwardThrust);
                    }

                    Actor.X += (Actor.Velocity.Angle.X * forwardThrust) - appliedOffset.X;
                    Actor.Y += (Actor.Velocity.Angle.Y * forwardThrust) - appliedOffset.Y;
                }

                if (Actor.Velocity.BoostPercentage > 0)
                {
                    Actor.ShipEngineBoostSound.Play();
                }
                else
                {
                    Actor.ShipEngineBoostSound.Fade();
                }

                if (Actor.Velocity.ThrottlePercentage > _core.Settings.MinPlayerThrust)
                {
                    Actor.ShipEngineRoarSound.Play();
                }
                else
                {
                    Actor.ShipEngineRoarSound.Fade();
                }

                //Scroll the background.
                _core.Display.BackgroundOffset.X += appliedOffset.X;
                _core.Display.BackgroundOffset.Y += appliedOffset.Y;

                //We are going to restrict the rotation speed to a percentage of thrust.
                var rotationSpeed = (Actor.Velocity.MaxRotationSpeed * Actor.Velocity.ThrottlePercentage);

                if (_core.Input.IsKeyPressed(HGPlayerKey.RotateCounterClockwise))
                {
                    Actor.Rotate(-(rotationSpeed > 1.0 ? rotationSpeed : 1.0));
                }
                else if (_core.Input.IsKeyPressed(HGPlayerKey.RotateClockwise))
                {
                    Actor.Rotate(rotationSpeed > 1.0 ? rotationSpeed : 1.0);
                }
            }

            _core.Display.CurrentQuadrant = _core.Display.GetQuadrant(
                Actor.X + _core.Display.BackgroundOffset.X,
                Actor.Y + _core.Display.BackgroundOffset.Y);

            return appliedOffset;
        }
    }
}
