using HG.Actors.Ordinary;
using HG.Engine;
using HG.TickHandlers.Interfaces;
using HG.Types;
using HG.Utility.ExtensionMethods;

namespace HG.TickHandlers
{
    internal class PlayerTickHandler : IVectorGeneratorTickManager
    {
        public ActorPlayer Actor { get; set; }
        private readonly Core _core;

        public PlayerTickHandler(Core core)
        {
            _core = core;
        }

        private bool _allowLockPlayerAngleToNearbyEnemy = true;

        /// <summary>
        /// Moves the player taking into account any inputs and returns a X,Y describing the amount and direction of movement.
        /// </summary>
        /// <returns></returns>
        public HgPoint<double> ExecuteWorldClockTick()
        {
            var displacementVector = new HgPoint<double>();

            if (Actor.Visable)
            {
                if (_core.Input.IsKeyPressed(HgPlayerKey.PrimaryFire))
                {
                    if (Actor.PrimaryWeapon != null && Actor.PrimaryWeapon.Fire())
                    {
                        if (Actor.PrimaryWeapon?.RoundQuantity == 25)
                        {
                            Actor.AmmoLowSound.Play();
                        }
                        if (Actor.PrimaryWeapon?.RoundQuantity == 0)
                        {
                            Actor.AmmoEmptySound.Play();
                        }
                    }
                }

                if (_core.Input.IsKeyPressed(HgPlayerKey.SecondaryFire))
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

                if (_core.Settings.LockPlayerAngleToNearbyEnemy)
                {
                    #region //This needs some work. It works, but its sloppy - the movement is rigid.
                    if (_core.Input.IsKeyPressed(HgPlayerKey.RotateClockwise) == false && _core.Input.IsKeyPressed(HgPlayerKey.RotateCounterClockwise) == false)
                    {
                        if (_allowLockPlayerAngleToNearbyEnemy)
                        {
                            var enemies = _core.Actors.Enemies.Visible();
                            foreach (var enemy in enemies)
                            {
                                var distanceTo = Actor.DistanceTo(enemy);
                                if (distanceTo < 500)
                                {
                                    if (Actor.IsPointingAt(enemy, 50))
                                    {
                                        if (enemy.IsPointingAway(Actor, 50))
                                        {
                                            var angleTo = Actor.AngleTo(enemy);
                                            Actor.Velocity.Angle.Degrees = angleTo;
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
                        Actor.Velocity.AvailableBoost = (Actor.Velocity.AvailableBoost + (1 - Actor.Velocity.BoostPercentage)).Box(0, _core.Settings.MaxPlayerBoost);
                    }
                    else
                    {
                        Actor.IsBoostFading = false;
                    }
                }

                if (Actor.BoostAnimation != null)
                {
                    Actor.BoostAnimation.Visable = _core.Input.IsKeyPressed(HgPlayerKey.SpeedBoost)
                        && _core.Input.IsKeyPressed(HgPlayerKey.Forward)
                        && Actor.Velocity.AvailableBoost > 0
                        && Actor.IsBoostFading == false;
                }

                if (Actor.ThrustAnimation != null)
                {
                    Actor.ThrustAnimation.Visable = Actor.Velocity.ThrottlePercentage > 0;
                }

                //Make player thrust "build up" and fade-in.
                if (_core.Input.IsKeyPressed(HgPlayerKey.Forward))
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
                    var forwardThrust = Actor.Velocity.MaxSpeed * Actor.Velocity.ThrottlePercentage;

                    if (Actor.Velocity.BoostPercentage > 0)
                    {
                        forwardThrust += Actor.Velocity.MaxBoost * Actor.Velocity.BoostPercentage;
                    }

                    //Close to the right wall and travelling in that direction.
                    if (Actor.X > _core.Display.NatrualScreenBounds.X + _core.Display.NatrualScreenBounds.Width - _core.Settings.InfiniteScrollWallX
                        && Actor.Velocity.Angle.X > 0)
                    {
                        displacementVector.X = Actor.Velocity.Angle.X * forwardThrust;
                    }

                    //Close to the bottom wall and travelling in that direction.
                    if (Actor.Y > _core.Display.NatrualScreenBounds.Y + _core.Display.NatrualScreenBounds.Height - _core.Settings.InfiniteScrollWallY
                        && Actor.Velocity.Angle.Y > 0)
                    {
                        displacementVector.Y = Actor.Velocity.Angle.Y * forwardThrust;
                    }

                    //Close to the left wall and travelling in that direction.
                    if (Actor.X < _core.Display.NatrualScreenBounds.X + _core.Settings.InfiniteScrollWallX
                        && Actor.Velocity.Angle.X < 0)
                    {
                        displacementVector.X = Actor.Velocity.Angle.X * forwardThrust;
                    }

                    //Close to the top wall and travelling in that direction.
                    if (Actor.Y < _core.Display.NatrualScreenBounds.Y + _core.Settings.InfiniteScrollWallY
                        && Actor.Velocity.Angle.Y < 0)
                    {
                        displacementVector.Y = Actor.Velocity.Angle.Y * forwardThrust;
                    }

                    Actor.X += Actor.Velocity.Angle.X * forwardThrust - displacementVector.X;
                    Actor.Y += Actor.Velocity.Angle.Y * forwardThrust - displacementVector.Y;
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

                //We are going to restrict the rotation speed to a percentage of thrust.
                var rotationSpeed = Actor.Velocity.MaxRotationSpeed * Actor.Velocity.ThrottlePercentage;

                if (_core.Input.IsKeyPressed(HgPlayerKey.RotateCounterClockwise))
                {
                    Actor.Rotate(-(rotationSpeed > 1.0 ? rotationSpeed : 1.0));
                }
                else if (_core.Input.IsKeyPressed(HgPlayerKey.RotateClockwise))
                {
                    Actor.Rotate(rotationSpeed > 1.0 ? rotationSpeed : 1.0);
                }
            }

            //Scroll the background.
            _core.Display.BackgroundOffset.X += displacementVector.X;
            _core.Display.BackgroundOffset.Y += displacementVector.Y;

            _core.Display.CurrentQuadrant = _core.Display.GetQuadrant(
                Actor.X + _core.Display.BackgroundOffset.X,
                Actor.Y + _core.Display.BackgroundOffset.Y);

            Actor.RenewableResources.RenewAllResources();

            return displacementVector;
        }

        public void ResetAndShow()
        {
            Actor.Reset();

            _core.Actors.RenderRadar = true;
            Actor.Visable = true;
            Actor.ShipEngineIdleSound.Play();
            Actor.AllSystemsGoSound.Play();
        }

        public void Hide()
        {
            Actor.Visable = false;
            _core.Actors.RenderRadar = false;
            Actor.ShipEngineIdleSound.Stop();
            Actor.ShipEngineRoarSound.Stop();
        }
    }
}
