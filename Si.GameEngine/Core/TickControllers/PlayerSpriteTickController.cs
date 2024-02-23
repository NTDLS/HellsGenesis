using Si.GameEngine.Core.TickControllers._Superclass;
using Si.GameEngine.Sprites.Player._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Payload.SpriteActions;
using Si.Library.Types.Geometry;
using System;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Core.TickControllers
{
    /// <summary>
    /// This is the controller for the single local player.
    /// </summary>
    public class PlayerSpriteTickController : PlayerTickControllerBase<SpritePlayerBase>
    {
        private readonly GameEngineCore _gameEngine;
        private bool _allowLockPlayerAngleToNearbyEnemy = true;

        public SpritePlayerBase Sprite { get; set; }

        public PlayerSpriteTickController(GameEngineCore gameEngine)
            : base(gameEngine)
        {
            _gameEngine = gameEngine;
        }

        public void InstantiatePlayerClass(Type playerClassType)
        {
            Sprite = SiReflection.CreateInstanceFromType<SpritePlayerBase>(playerClassType, new object[] { _gameEngine });
            Sprite.Visable = false;
        }

        /// <summary>
        /// Moves the player taking into account any inputs and returns a X,Y describing the amount and direction of movement.
        /// </summary>
        /// <returns></returns>
        public override SiPoint ExecuteWorldClockTick(double epoch)
        {
            var displacementVector = new SiPoint();

            Sprite.IsLockedOnSoft = false;
            Sprite.IsLockedOnHard = false;

            if (Sprite.Visable)
            {
                //Sprite.PrimaryWeapon.ApplyIntelligence();
                Sprite.SelectedSecondaryWeapon?.ApplyIntelligence(epoch);

                if (GameEngine.Input.IsKeyPressed(SiPlayerKey.PrimaryFire))
                {
                    if (Sprite.PrimaryWeapon != null && Sprite.PrimaryWeapon.Fire())
                    {
                        if (_gameEngine.Multiplay.State.PlayMode != SiPlayMode.SinglePlayer && Sprite.IsDrone == false)
                        {
                            _gameEngine.Multiplay.RecordDroneActionFireWeapon(new SiSpriteActionFireWeapon(Sprite.MultiplayUID)
                            {
                                WeaponTypeName = Sprite.PrimaryWeapon.GetType().Name,
                            });
                        }

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

                if (GameEngine.Input.IsKeyPressed(SiPlayerKey.SecondaryFire))
                {
                    if (Sprite.SelectedSecondaryWeapon != null && Sprite.SelectedSecondaryWeapon.Fire())
                    {
                        if (_gameEngine.Multiplay.State.PlayMode != SiPlayMode.SinglePlayer && Sprite.IsDrone == false)
                        {
                            _gameEngine.Multiplay.RecordDroneActionFireWeapon(new SiSpriteActionFireWeapon(Sprite.MultiplayUID)
                            {
                                WeaponTypeName = Sprite.SelectedSecondaryWeapon.GetType().Name,
                            });
                        }

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

                #region LockPlayerAngleToNearbyEnemy.
                //This needs some work. It works, but its odd - the movement is rigid.

                if (GameEngine.Settings.LockPlayerAngleToNearbyEnemy)
                {
                    if (GameEngine.Input.IsKeyPressed(SiPlayerKey.RotateClockwise) == false && GameEngine.Input.IsKeyPressed(SiPlayerKey.RotateCounterClockwise) == false)
                    {
                        if (_allowLockPlayerAngleToNearbyEnemy)
                        {
                            var enemies = GameEngine.Sprites.Enemies.Visible();
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
                }
                #endregion

                //Make player boost "build up" and fade-in.
                if (GameEngine.Input.IsKeyPressed(SiPlayerKey.SpeedBoost) && GameEngine.Input.IsKeyPressed(SiPlayerKey.Forward)
                    && Sprite.Velocity.AvailableBoost > 0 && Sprite.Velocity.BoostRebuilding == false)
                {
                    if (Sprite.Velocity.BoostPercentage < 1.0)
                    {
                        double boostToAdd = Sprite.Velocity.BoostPercentage > 0
                            ? GameEngine.Settings.PlayerThrustRampUp * (1 - Sprite.Velocity.BoostPercentage) : GameEngine.Settings.PlayerThrustRampUp;

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
                        Sprite.Velocity.BoostPercentage -= GameEngine.Settings.PlayerThrustRampDown;
                        if (Sprite.Velocity.BoostPercentage < 0.01)
                        {
                            Sprite.Velocity.BoostPercentage = 0;
                        }
                    }

                    if (GameEngine.Input.IsKeyPressed(SiPlayerKey.SpeedBoost) == false && Sprite.Velocity.AvailableBoost < GameEngine.Settings.MaxPlayerBoostAmount)
                    {
                        Sprite.Velocity.AvailableBoost = (Sprite.Velocity.AvailableBoost + 5).Box(0, GameEngine.Settings.MaxPlayerBoostAmount);

                        if (Sprite.Velocity.BoostRebuilding && Sprite.Velocity.AvailableBoost >= GameEngine.Settings.PlayerBoostRebuildFloor)
                        {
                            Sprite.Velocity.BoostRebuilding = false;
                        }
                    }
                }

                //Make player thrust "build up" and fade-in.
                if (GameEngine.Input.IsKeyPressed(SiPlayerKey.Forward))
                {
                    if (Sprite.Velocity.ThrottlePercentage < 1.0)
                    {
                        //We add the first thrust amount with the amount defined in "PlayerThrustRampUp", after that we add the
                        // calculated inversee-percentage-from-100-percent of "PlayerThrustRampUp" to the ThrottlePercentage until we reach 100%.
                        // This causes our thrust to start qucikly and acceleration to fade fast as we approach full throttle. The last few %-points
                        //  of throttle will take a while. We do the reverse of this to stop. Stopping fast at first and slowly-slowly slowing to a stop.

                        double thrustToAdd = Sprite.Velocity.ThrottlePercentage > 0
                            ? GameEngine.Settings.PlayerThrustRampUp * (1 - Sprite.Velocity.ThrottlePercentage) : GameEngine.Settings.PlayerThrustRampUp;

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
                            ? GameEngine.Settings.PlayerThrustRampDown * Sprite.Velocity.ThrottlePercentage : GameEngine.Settings.PlayerThrustRampDown;

                        Sprite.Velocity.ThrottlePercentage -= thrustToRemove;

                        if (Sprite.Velocity.ThrottlePercentage < 0.01)
                        {
                            //Don't overshoot the stop.
                            Sprite.Velocity.ThrottlePercentage = 0;
                        }
                    }
                    else if (Sprite.Velocity.ThrottlePercentage < 0)
                    {
                        //Ramp up to a stop:
                        double thrustToRemove = Sprite.Velocity.ThrottlePercentage * -1 < 1
                            ? GameEngine.Settings.PlayerThrustRampDown * (1 - Sprite.Velocity.ThrottlePercentage * -1) : GameEngine.Settings.PlayerThrustRampDown;

                        Sprite.Velocity.ThrottlePercentage += thrustToRemove;
                        if (Sprite.Velocity.ThrottlePercentage > 0)
                        {
                            //Don't overshoot the stop.
                            Sprite.Velocity.ThrottlePercentage = 0;
                        }
                    }
                }

                if (Sprite.BoostAnimation != null)
                {
                    Sprite.BoostAnimation.Visable =
                        GameEngine.Input.IsKeyPressed(SiPlayerKey.SpeedBoost)
                        && GameEngine.Input.IsKeyPressed(SiPlayerKey.Forward)
                        && Sprite.Velocity.AvailableBoost > 0 && Sprite.Velocity.BoostRebuilding == false;
                }

                if (Sprite.Velocity.AvailableBoost <= 0)
                {
                    Sprite.Velocity.AvailableBoost = 0;
                    Sprite.Velocity.BoostRebuilding = true;
                }

                if (Sprite.ThrustAnimation != null)
                {
                    Sprite.ThrustAnimation.Visable = GameEngine.Input.IsKeyPressed(SiPlayerKey.Forward);
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
                var rotationSpeed = GameEngine.Settings.MaxPlayerRotationSpeedDegrees * Sprite.Velocity.ThrottlePercentage;

                if (GameEngine.Input.IsKeyPressed(SiPlayerKey.RotateCounterClockwise))
                {
                    Sprite.Rotate(-(rotationSpeed > 1.0 ? rotationSpeed : 1.0));
                }
                else if (GameEngine.Input.IsKeyPressed(SiPlayerKey.RotateClockwise))
                {
                    Sprite.Rotate(rotationSpeed > 1.0 ? rotationSpeed : 1.0);
                }

                //insted of a fixed location, lets set the player to the center of the background offset.
                //this is so bullets work right
            }

            //Scroll the background.
            GameEngine.Display.RenderWindowPosition.X += displacementVector.X;
            GameEngine.Display.RenderWindowPosition.Y += displacementVector.Y;

            //Move the player in the direction of the background. This keeps the player visually in place, which is in the center screen.
            Sprite.X += displacementVector.X;
            Sprite.Y += displacementVector.Y;

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
            if (multiplayVector != null && Sprite.Visable)
            {
                _gameEngine.Multiplay.RecordDroneActionVector(multiplayVector);
            }

            return displacementVector;
        }

        public void ResetAndShow()
        {
            Sprite.Reset();

            GameEngine.Sprites.PlayerStatsText.Visable = true;
            GameEngine.Sprites.RenderRadar = true;
            Sprite.Visable = true;
            Sprite.ShipEngineIdleSound.Play();
            Sprite.AllSystemsGoSound.Play();
        }

        public void Show()
        {
            GameEngine.Sprites.PlayerStatsText.Visable = true;
            GameEngine.Sprites.RenderRadar = true;
            Sprite.Visable = true;
            Sprite.ShipEngineIdleSound.Play();
            Sprite.AllSystemsGoSound.Play();
        }

        public void Hide()
        {
            GameEngine.Sprites.PlayerStatsText.Visable = false;
            GameEngine.Sprites.RenderRadar = false;
            Sprite.Visable = false;
            Sprite.ShipEngineIdleSound.Stop();
            Sprite.ShipEngineRoarSound.Stop();
        }
    }
}
