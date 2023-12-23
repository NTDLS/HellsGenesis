using Si.Game.Engine;
using Si.Game.Engine.Types.Geometry;
using Si.Game.Sprites.Player.BasesAndInterfaces;
using Si.Game.TickControllers.BasesAndInterfaces;
using Si.Game.Utility.ExtensionMethods;
using static Si.Shared.SiConstants;

namespace Si.Game.Controller
{
    /// <summary>
    /// This is the controller for the single local player.
    /// </summary>
    internal class PlayerSpriteTickController : PlayerTickControllerBase<SpritePlayerBase>
    {
        private readonly EngineCore _gameCore;
        private bool _allowLockPlayerAngleToNearbyEnemy = true;

        public SpritePlayerBase Sprite { get; set; }

        public PlayerSpriteTickController(EngineCore gameCore)
            : base(gameCore)
        {
            _gameCore = gameCore;
        }

        /// <summary>
        /// Moves the player taking into account any inputs and returns a X,Y describing the amount and direction of movement.
        /// </summary>
        /// <returns></returns>
        public override SiPoint ExecuteWorldClockTick()
        {
            var displacementVector = new SiPoint();

            if (Sprite.Visable)
            {
                if (GameCore.Input.IsKeyPressed(SiPlayerKey.PrimaryFire))
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

                if (GameCore.Input.IsKeyPressed(SiPlayerKey.SecondaryFire))
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

                if (GameCore.Settings.LockPlayerAngleToNearbyEnemy)
                {
                    #region //This needs some work. It works, but its odd - the movement is rigid.
                    if (GameCore.Input.IsKeyPressed(SiPlayerKey.RotateClockwise) == false && GameCore.Input.IsKeyPressed(SiPlayerKey.RotateCounterClockwise) == false)
                    {
                        if (_allowLockPlayerAngleToNearbyEnemy)
                        {
                            var enemies = GameCore.Sprites.Enemies.Visible();
                            foreach (var enemy in enemies)
                            {
                                var distanceTo = Sprite.DistanceTo(enemy);
                                if (distanceTo < 500)
                                {
                                    if (Sprite.IsPointingAt(enemy, 50))
                                    {
                                        if (enemy.IsPointingAway(Sprite, 50))
                                        {
                                            var angleTo = Sprite.AngleTo360(enemy);
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
                if (GameCore.Input.IsKeyPressed(SiPlayerKey.SpeedBoost) && GameCore.Input.IsKeyPressed(SiPlayerKey.Forward)
                    && Sprite.Velocity.AvailableBoost > 0 && Sprite.Velocity.BoostRebuilding == false)
                {
                    if (Sprite.Velocity.BoostPercentage < 1.0)
                    {
                        double boostToAdd = Sprite.Velocity.BoostPercentage > 0
                            ? GameCore.Settings.PlayerThrustRampUp * (1 - Sprite.Velocity.BoostPercentage) : GameCore.Settings.PlayerThrustRampUp;

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
                        Sprite.Velocity.BoostPercentage -= GameCore.Settings.PlayerThrustRampDown;
                        if (Sprite.Velocity.BoostPercentage < 0.01)
                        {
                            Sprite.Velocity.BoostPercentage = 0;
                        }
                    }

                    if (GameCore.Input.IsKeyPressed(SiPlayerKey.SpeedBoost) == false && Sprite.Velocity.AvailableBoost < GameCore.Settings.MaxPlayerBoostAmount)
                    {
                        Sprite.Velocity.AvailableBoost = (Sprite.Velocity.AvailableBoost + 5).Box(0, GameCore.Settings.MaxPlayerBoostAmount);

                        if (Sprite.Velocity.BoostRebuilding && Sprite.Velocity.AvailableBoost >= GameCore.Settings.PlayerBoostRebuildFloor)
                        {
                            Sprite.Velocity.BoostRebuilding = false;
                        }
                    }
                }

                //Make player thrust "build up" and fade-in.
                if (GameCore.Input.IsKeyPressed(SiPlayerKey.Forward))
                {
                    if (Sprite.Velocity.ThrottlePercentage < 1.0)
                    {
                        //We add the first thrust amount with the amount defined in "PlayerThrustRampUp", after that we add the
                        // calculated inversee-percentage-from-100-percent of "PlayerThrustRampUp" to the ThrottlePercentage until we reach 100%.
                        // This causes our thrust to start qucikly and acceleration to fade fast as we approach full throttle. The last few %-points
                        //  of throttle will take a while. We do the reverse of this to stop. Stopping fast at first and slowly-slowly slowing to a stop.

                        double thrustToAdd = Sprite.Velocity.ThrottlePercentage > 0
                            ? GameCore.Settings.PlayerThrustRampUp * (1 - Sprite.Velocity.ThrottlePercentage) : GameCore.Settings.PlayerThrustRampUp;

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
                            ? GameCore.Settings.PlayerThrustRampDown * Sprite.Velocity.ThrottlePercentage : GameCore.Settings.PlayerThrustRampDown;

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
                            ? GameCore.Settings.PlayerThrustRampDown * (1 - Sprite.Velocity.ThrottlePercentage * -1) : GameCore.Settings.PlayerThrustRampDown;

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
                        GameCore.Input.IsKeyPressed(SiPlayerKey.SpeedBoost)
                        && GameCore.Input.IsKeyPressed(SiPlayerKey.Forward)
                        && Sprite.Velocity.AvailableBoost > 0 && Sprite.Velocity.BoostRebuilding == false;
                }

                if (Sprite.Velocity.AvailableBoost <= 0)
                {
                    Sprite.Velocity.AvailableBoost = 0;
                    Sprite.Velocity.BoostRebuilding = true;
                }

                if (Sprite.ThrustAnimation != null)
                {
                    Sprite.ThrustAnimation.Visable = GameCore.Input.IsKeyPressed(SiPlayerKey.Forward);
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
                var rotationSpeed = GameCore.Settings.MaxPlayerRotationSpeedDegrees * Sprite.Velocity.ThrottlePercentage;

                if (GameCore.Input.IsKeyPressed(SiPlayerKey.RotateCounterClockwise))
                {
                    Sprite.Rotate(-(rotationSpeed > 1.0 ? rotationSpeed : 1.0));
                }
                else if (GameCore.Input.IsKeyPressed(SiPlayerKey.RotateClockwise))
                {
                    Sprite.Rotate(rotationSpeed > 1.0 ? rotationSpeed : 1.0);
                }
            }

            //Scroll the background.
            GameCore.Display.BackgroundOffset.X += displacementVector.X;
            GameCore.Display.BackgroundOffset.Y += displacementVector.Y;

            if (Sprite.Velocity.RecoilPercentage > 0)
            {
                Sprite.Velocity.RecoilPercentage -= Sprite.Velocity.RecoilPercentage * 0.10;
                if (Sprite.Velocity.RecoilPercentage < 0.001)
                {
                    Sprite.Velocity.RecoilPercentage = 0;
                }
            }

            Sprite.RenewableResources.RenewAllResources();

            var multiplayVector = Sprite.GetMultiplayVector();
            if (multiplayVector != null)
            {
                _gameCore.Multiplay.RecordSpriteVector(multiplayVector);
            }

            return displacementVector;
        }

        public void ResetAndShow()
        {
            Sprite.Reset();

            GameCore.Sprites.RenderRadar = true;
            Sprite.Visable = true;
            Sprite.ShipEngineIdleSound.Play();
            Sprite.AllSystemsGoSound.Play();
        }

        public void Hide()
        {
            Sprite.Visable = false;
            GameCore.Sprites.RenderRadar = false;
            Sprite.ShipEngineIdleSound.Stop();
            Sprite.ShipEngineRoarSound.Stop();
        }
    }
}
