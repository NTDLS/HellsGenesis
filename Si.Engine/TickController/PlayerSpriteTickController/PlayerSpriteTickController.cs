using Si.Engine.Sprite.Player;
using Si.Engine.Sprite.Player._Superclass;
using Si.Engine.TickController._Superclass;
using Si.GameEngine.Persistent;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;
using System;
using System.Diagnostics;
using static Si.Library.SiConstants;

namespace Si.Engine.TickController.PlayerSpriteTickController
{
    /// <summary>
    /// This is the controller for the single local player.
    /// </summary>
    public class PlayerSpriteTickController : PlayerSpriteTickControllerBase<SpritePlayerBase>
    {
        private readonly EngineCore _engine;
        private readonly Stopwatch _inputDelay = new();

        public PlayerStats Stats { get; set; } = new(); //This should be saved.
        public SpritePlayerBase Sprite { get; set; }

        public PlayerSpriteTickController(EngineCore engine)
            : base(engine)
        {
            //This is where the player is created.
            Sprite = new SpriteDebugPlayer(engine) { Visable = false };
            engine.Sprites.Add(Sprite);
            _engine = engine;
            _inputDelay.Restart();
        }

        public void InstantiatePlayerClass(Type playerClassType)
        {
            //Remove the player from the sprite collection.
            Sprite.QueueForDelete();
            Sprite.Cleanup();

            Sprite = SiReflection.CreateInstanceFromType<SpritePlayerBase>(playerClassType, new object[] { _engine });
            Sprite.Visable = false;
            _engine.Sprites.Add(Sprite); //Add the player back to the sprite collection.
        }

        private float _forwardVelocity = 0;
        private float _boostForwardVelocity = 0;
        private float _lateralVelocity = 0;

        /// <summary>
        /// Moves the player taking into account any inputs and returns a X,Y describing the amount and direction of movement.
        /// </summary>
        /// <returns></returns>
        public override SiPoint ExecuteWorldClockTick(float epoch)
        {
            Sprite.IsLockedOnSoft = false;
            Sprite.IsLockedOnHard = false;

            if (Sprite.Visable)
            {
                #region Weapons Selection and Fire.

                if (Engine.Input.IsKeyPressed(SiPlayerKey.SwitchWeaponLeft))
                {
                    if (_inputDelay.ElapsedMilliseconds > 200)
                    {
                        _engine.Player?.Sprite?.SelectPreviousAvailableUsableSecondaryWeapon();
                        _inputDelay.Restart();
                    }
                }
                if (Engine.Input.IsKeyPressed(SiPlayerKey.SwitchWeaponRight))
                {
                    if (_inputDelay.ElapsedMilliseconds > 200)
                    {
                        _engine.Player?.Sprite?.SelectNextAvailableUsableSecondaryWeapon();
                        _inputDelay.Restart();
                    }
                }

                Sprite.SelectedSecondaryWeapon?.ApplyIntelligence(epoch);

                if (Engine.Input.IsKeyPressed(SiPlayerKey.PrimaryFire))
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

                if (Engine.Input.IsKeyPressed(SiPlayerKey.SecondaryFire))
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

                #endregion

                // We have to do some creative stuff here since we allow forward/reverse and right/left strafing.
                // No other sprite can strafe, so we're going to make all of this a special case. In the end, the
                //  gathered inputs here are baked into the player sprite's Travel.Velocity just like any other sprite.

                float throttleFloor = 0.01f;
                float throttleCap = 0.70f; // 70% will be considered max throttle in any direction, this is because
                                           //   the combined forward and lateral can only be as much as 0.707 each.
                float velocityRampUp = Engine.Settings.PlayerVelocityRampUp * epoch;
                float velocityRampDown = Engine.Settings.PlayerVelocityRampDown * epoch;

                #region Forward and Reverse.

                float targetForwardAmount = (Engine.Input.GetAnalogAxisValue(SiPlayerKey.Reverse, SiPlayerKey.Forward) / throttleCap).Clamp(-1, 1);

                if (targetForwardAmount > throttleFloor)
                {
                    if (_forwardVelocity <= targetForwardAmount) //The target forward throttle is more than we have applied: ramp-up.
                    {
                        _forwardVelocity = (_forwardVelocity + velocityRampUp).Clamp(-1, targetForwardAmount); //Make player forward velocity build-up.
                    }
                    else //The target forward throttle is less than we have applied: ramp-down.
                    {
                        _forwardVelocity = (_forwardVelocity - velocityRampDown).Clamp(targetForwardAmount, 1);
                    }
                }
                else if (targetForwardAmount < -throttleFloor)
                {
                    if (_forwardVelocity >= targetForwardAmount) //The target reverse throttle is more than we have applied: ramp-up.
                    {
                        _forwardVelocity = (_forwardVelocity - velocityRampUp).Clamp(targetForwardAmount, 1); //Make player forward velocity build-up.
                    }
                    else //The target reverse throttle is less than we have applied: ramp-down.
                    {
                        _forwardVelocity = (_forwardVelocity + velocityRampDown).Clamp(targetForwardAmount, 1);
                    }
                }
                else //No forward input was received, ramp down the forward velocity.
                {
                    if (Math.Abs(velocityRampDown) >= Math.Abs(_forwardVelocity))
                    {
                        _forwardVelocity = 0; //Don't overshoot the stop.
                    }
                    else _forwardVelocity -= _forwardVelocity > 0 ? velocityRampDown : -velocityRampDown;
                }

                #endregion

                #region Forward Speed-Boost.

                if (Engine.Input.IsKeyPressed(SiPlayerKey.SpeedBoost)
                    && _forwardVelocity >= throttleFloor
                    && Sprite.RenewableResources.Observe(Sprite.BoostResourceName) > 0)
                {
                    var boostAmount = Sprite.RenewableResources.Consume(Sprite.BoostResourceName, epoch);

                    if (_boostForwardVelocity < (Sprite.MaxThrottle - 1.0f))
                    {
                        _boostForwardVelocity += boostAmount;
                    }
                }
                else if (_boostForwardVelocity > 0)
                {
                    //Ramp down the over-throttle.
                    _boostForwardVelocity -= velocityRampDown;
                }

                _boostForwardVelocity = _boostForwardVelocity.Clamp(0, Sprite.MaxThrottle - 1.0f);

                #endregion

                #region Laterial Strafing.

                float targetLateralAmount = (Engine.Input.GetAnalogAxisValue(SiPlayerKey.StrafeLeft, SiPlayerKey.StrafeRight) / throttleCap).Clamp(-1, 1);

                if (targetLateralAmount >= throttleFloor) //Strafe right.
                {
                    if (_lateralVelocity <= targetLateralAmount) //The target lateral throttle is more than we have applied: ramp-up.
                    {
                        _lateralVelocity = (_lateralVelocity + velocityRampUp).Clamp(-1, targetLateralAmount); //Make player lateral velocity build-up.
                    }
                    else //The target lateral throttle is less than we have applied: ramp-down.
                    {
                        _lateralVelocity = (_lateralVelocity - velocityRampDown).Clamp(targetLateralAmount, 1);
                    }
                }
                else if (targetLateralAmount <= -throttleFloor) //Strafe left.
                {
                    if (_lateralVelocity >= targetLateralAmount) //The target reverse lateral throttle is more than we have applied: ramp-up.
                    {
                        _lateralVelocity = (_lateralVelocity - velocityRampUp).Clamp(targetLateralAmount, 1); //Make player forward velocity build-up.
                    }
                    else //The target reverse lateral throttle is less than we have applied: ramp-down.
                    {
                        _lateralVelocity = (_lateralVelocity + velocityRampDown).Clamp(targetLateralAmount, 1);
                    }
                }
                else //No lateral input was received, ramp down the lateral velocity.
                {
                    if (Math.Abs(velocityRampDown) >= Math.Abs(_lateralVelocity))
                    {
                        _lateralVelocity = 0; //Don't overshoot the stop.
                    }
                    else _lateralVelocity -= _lateralVelocity > 0 ? velocityRampDown : -velocityRampDown;
                }

                #endregion

                #region Rotation.

                float targetRotationAmount = (Engine.Input.GetAnalogAxisValue(SiPlayerKey.RotateCounterClockwise, SiPlayerKey.RotateClockwise) / throttleCap).Clamp(-1, 1);

                var rotationSpeed = Engine.Settings.MaxPlayerRotationSpeedDegrees * targetRotationAmount * epoch;

                if (rotationSpeed > 0)
                {
                    Sprite.Rotate(rotationSpeed);
                }
                else if (rotationSpeed < 0)
                {
                    Sprite.Rotate(rotationSpeed);
                }

                #endregion

                #region Sounds and Animation.

                if (_boostForwardVelocity > 0)
                    Sprite.ShipEngineBoostSound.Play();
                else Sprite.ShipEngineBoostSound.Fade();

                if (_forwardVelocity >= throttleFloor)
                    Sprite.ShipEngineRoarSound.Play();
                else Sprite.ShipEngineRoarSound.Fade();

                if (Sprite.ThrusterAnimation != null)
                {
                    Sprite.ThrusterAnimation.Visable = (targetForwardAmount >= throttleFloor);
                }

                if (Sprite.BoostAnimation != null)
                {
                    Sprite.BoostAnimation.Visable =
                        (targetForwardAmount >= throttleFloor)
                        && Engine.Input.IsKeyPressed(SiPlayerKey.SpeedBoost)
                        && _boostForwardVelocity > 0
                        && Sprite.RenewableResources.IsCoolingDown(Sprite.BoostResourceName) == false;
                }

                #endregion
            }

            Sprite.RenewableResources.RenewAllResources(epoch);

            Sprite.Throttle = 1 + _boostForwardVelocity;

            Sprite.MovementVector = (Sprite.MakeMovementVector() * _forwardVelocity) //Forward / Reverse
                            + (Sprite.MakeMovementVector(Sprite.Direction + SiPoint.RADIANS_90) * _lateralVelocity);  //Lateral strafing.

            Sprite.PerformCollisionDetection(epoch);

            var displacementVector = Sprite.MovementVector * epoch;

            //Scroll the background.
            Engine.Display.RenderWindowPosition += displacementVector;

            //Move the player in the direction of the background. This keeps the player visually in place, which is in the center screen.
            Sprite.Location += displacementVector;

            return displacementVector;
        }

        public void ResetAndShow()
        {
            Sprite.Reset();

            Engine.Sprites.TextBlocks.PlayerStatsText.Visable = true;
            Engine.Sprites.RenderRadar = true;
            Sprite.Visable = true;
            Sprite.ShipEngineIdleSound.Play();
            Sprite.AllSystemsGoSound.Play();
        }

        public void Show()
        {
            Engine.Sprites.TextBlocks.PlayerStatsText.Visable = true;
            Engine.Sprites.RenderRadar = true;
            Sprite.Visable = true;
            Sprite.ShipEngineIdleSound.Play();
            Sprite.AllSystemsGoSound.Play();
        }

        public void Hide()
        {
            Engine.Sprites.TextBlocks.PlayerStatsText.Visable = false;
            Engine.Sprites.RenderRadar = false;
            Sprite.Visable = false;
            Sprite.ShipEngineIdleSound.Stop();
            Sprite.ShipEngineRoarSound.Stop();
        }
    }
}
