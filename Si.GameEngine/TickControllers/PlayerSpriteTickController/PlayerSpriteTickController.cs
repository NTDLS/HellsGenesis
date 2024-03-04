using Si.GameEngine.Sprites.Player._Superclass;
using Si.GameEngine.TickControllers._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;
using Si.Library.Payload.SpriteActions;
using System;
using static Si.Library.SiConstants;

namespace Si.GameEngine.TickControllers.PlayerSpriteTickController
{
    /// <summary>
    /// This is the controller for the single local player.
    /// </summary>
    public class PlayerSpriteTickController : PlayerSpriteTickControllerBase<SpritePlayerBase>
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
        public override SiPoint ExecuteWorldClockTick(float epoch)
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
                    if (GameEngine.Input.IsKeyPressed(SiPlayerKey.RotateClockwise) == false
                        && GameEngine.Input.IsKeyPressed(SiPlayerKey.RotateCounterClockwise) == false)
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
                    && Sprite.Velocity.AvailableBoost > 0 && Sprite.Velocity.IsBoostRecharging == false)
                {
                    if (Sprite.Velocity.ForwardBoostMomentium < 1.0)
                    {
                        float boostToAdd = Sprite.Velocity.ForwardBoostMomentium > 0
                            ? GameEngine.Settings.PlayerThrustRampUp * (1 - Sprite.Velocity.ForwardBoostMomentium) : GameEngine.Settings.PlayerThrustRampUp;

                        Sprite.Velocity.ForwardBoostMomentium += boostToAdd;
                    }

                    Sprite.Velocity.AvailableBoost -= Sprite.Velocity.MaximumBoostSpeed * Sprite.Velocity.ForwardBoostMomentium;
                    if (Sprite.Velocity.AvailableBoost < 0)
                    {
                        Sprite.Velocity.AvailableBoost = 0;
                    }
                }
                else
                {
                    //If no "forward" or "reverse" user input is received... then fade the boost and rebuild available boost.
                    if (Sprite.Velocity.ForwardBoostMomentium > 0)
                    {
                        Sprite.Velocity.ForwardBoostMomentium -= GameEngine.Settings.PlayerThrustRampDown;
                        if (Sprite.Velocity.ForwardBoostMomentium < 0.01)
                        {
                            Sprite.Velocity.ForwardBoostMomentium = 0;
                        }
                    }

                    if (GameEngine.Input.IsKeyPressed(SiPlayerKey.SpeedBoost) == false && Sprite.Velocity.AvailableBoost < GameEngine.Settings.MaxPlayerBoostAmount)
                    {
                        Sprite.Velocity.AvailableBoost = (Sprite.Velocity.AvailableBoost + 5).Clamp(0, GameEngine.Settings.MaxPlayerBoostAmount);

                        if (Sprite.Velocity.IsBoostRecharging && Sprite.Velocity.AvailableBoost >= GameEngine.Settings.PlayerBoostRebuildFloor)
                        {
                            Sprite.Velocity.IsBoostRecharging = false;
                        }
                    }
                }

                #region Forward and Reverse.

                float forwardThrustToAdd = Sprite.Velocity.ForwardMomentium == 0 ? GameEngine.Settings.PlayerThrustRampUp
                    : GameEngine.Settings.PlayerThrustRampUp * (1 - Sprite.Velocity.ForwardMomentium);

                //Make player forward momentium "build up" and fade-out.
                if (GameEngine.Input.IsKeyPressed(SiPlayerKey.Forward))
                {
                    Sprite.Velocity.ForwardMomentium += forwardThrustToAdd;
                    Sprite.Velocity.ForwardMomentium.Clamp(-1, 1);
                }
                else if (GameEngine.Input.IsKeyPressed(SiPlayerKey.Reverse))
                {
                    Sprite.Velocity.ForwardMomentium -= forwardThrustToAdd;
                    Sprite.Velocity.ForwardMomentium.Clamp(-1, 1);
                }
                else
                {
                    float thrustToRemove = Sprite.Velocity.ForwardMomentium == 0 ? GameEngine.Settings.PlayerThrustRampDown
                        : GameEngine.Settings.PlayerThrustRampDown * Sprite.Velocity.ForwardMomentium;

                    if (Math.Abs(thrustToRemove) + 0.1 >= Math.Abs(Sprite.Velocity.ForwardMomentium))
                    {
                        Sprite.Velocity.ForwardMomentium = 0; //Don't overshoot the stop.
                    }
                    else Sprite.Velocity.ForwardMomentium -= thrustToRemove;
                }

                #endregion

                #region Strafing.

                var strafeAngle = SiPoint.PointFromAngleAtDistance360(new SiAngle(Sprite.Velocity.Angle - SiPoint.DEG_90_RADS), new SiPoint(1, 1));

                float strafeThrustToAdd = Sprite.Velocity.LateralMomentium == 0 ? GameEngine.Settings.PlayerThrustRampUp
                    : GameEngine.Settings.PlayerThrustRampUp * (1 - Sprite.Velocity.LateralMomentium);

                //Make player lateral momentium "build up" and fade-out.
                if (GameEngine.Input.IsKeyPressed(SiPlayerKey.StrafeLeft) && !GameEngine.Input.IsKeyPressed(SiPlayerKey.StrafeRight))
                {
                    Sprite.Velocity.LateralMomentium += strafeThrustToAdd;
                    Sprite.Velocity.LateralMomentium.Clamp(-1, 1);
                }
                else if (!GameEngine.Input.IsKeyPressed(SiPlayerKey.StrafeLeft) && GameEngine.Input.IsKeyPressed(SiPlayerKey.StrafeRight))
                {
                    Sprite.Velocity.LateralMomentium -= strafeThrustToAdd;
                    Sprite.Velocity.LateralMomentium.Clamp(-1, 1);
                }
                else //Ramp down to a stop:
                {
                    float thrustToRemove = Sprite.Velocity.LateralMomentium == 0 ? GameEngine.Settings.PlayerThrustRampDown
                        : GameEngine.Settings.PlayerThrustRampDown * Sprite.Velocity.LateralMomentium;

                    if (Math.Abs(thrustToRemove) + 0.1 >= Math.Abs(Sprite.Velocity.LateralMomentium))
                    {
                        Sprite.Velocity.LateralMomentium = 0; //Don't overshoot the stop.
                    }
                    else Sprite.Velocity.LateralMomentium -= thrustToRemove;
                }

                #endregion 

                if (Sprite.Velocity.AvailableBoost <= 0)
                {
                    Sprite.Velocity.AvailableBoost = 0;
                    Sprite.Velocity.IsBoostRecharging = true;
                }

                var totalForwardThrust =
                    (Sprite.Velocity.MaximumSpeed * Sprite.Velocity.ForwardMomentium)
                    + (Sprite.Velocity.MaximumBoostSpeed * Sprite.Velocity.ForwardBoostMomentium);

                displacementVector +=
                    (Sprite.Velocity.Angle * totalForwardThrust) +
                    (strafeAngle * Sprite.Velocity.MaximumSpeed * Sprite.Velocity.LateralMomentium);

                //We are going to restrict the rotation speed to a percentage of momentium.
                var rotationSpeed = GameEngine.Settings.MaxPlayerRotationSpeedDegrees
                    * ((Sprite.Velocity.LateralMomentium + Sprite.Velocity.ForwardMomentium) / 2);

                if (GameEngine.Input.IsKeyPressed(SiPlayerKey.RotateCounterClockwise) && !GameEngine.Input.IsKeyPressed(SiPlayerKey.RotateClockwise))
                {
                    Sprite.Rotate(-(rotationSpeed > 1.0 ? rotationSpeed : 1.0f));
                }
                if (!GameEngine.Input.IsKeyPressed(SiPlayerKey.RotateCounterClockwise) && GameEngine.Input.IsKeyPressed(SiPlayerKey.RotateClockwise))
                {
                    Sprite.Rotate(rotationSpeed > 1.0 ? rotationSpeed : 1.0f);
                }

                #region Sounds and Animation.

                if (Sprite.Velocity.ForwardBoostMomentium > 0.1)
                {
                    Sprite.ShipEngineBoostSound.Play();
                }
                else
                {
                    Sprite.ShipEngineBoostSound.Fade();
                }

                if (Sprite.Velocity.ForwardMomentium > 0.1)
                {
                    Sprite.ShipEngineRoarSound.Play();
                }
                else
                {
                    Sprite.ShipEngineRoarSound.Fade();
                }

                if (Sprite.ThrustAnimation != null)
                {
                    Sprite.ThrustAnimation.Visable = GameEngine.Input.IsKeyPressed(SiPlayerKey.Forward);
                }

                if (Sprite.BoostAnimation != null)
                {
                    Sprite.BoostAnimation.Visable =
                        GameEngine.Input.IsKeyPressed(SiPlayerKey.SpeedBoost)
                        && GameEngine.Input.IsKeyPressed(SiPlayerKey.Forward)
                        && Sprite.Velocity.AvailableBoost > 0 && Sprite.Velocity.IsBoostRecharging == false;
                }

                #endregion
            }

            //Scroll the background.
            GameEngine.Display.RenderWindowPosition += displacementVector;

            //Move the player in the direction of the background. This keeps the player visually in place, which is in the center screen.
            Sprite.Location += displacementVector;

            Sprite.RenewableResources.RenewAllResources(epoch);

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
