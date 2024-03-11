using Si.Engine.Sprite.Player._Superclass;
using Si.Engine.TickController._Superclass;
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
        private readonly Stopwatch _inputDelay = new Stopwatch();

        public SpritePlayerBase Sprite { get; set; }

        public PlayerSpriteTickController(EngineCore engine)
            : base(engine)
        {
            _engine = engine;
            _inputDelay.Restart();
        }

        public void InstantiatePlayerClass(Type playerClassType)
        {
            Sprite.Cleanup();
            Sprite = SiReflection.CreateInstanceFromType<SpritePlayerBase>(playerClassType, new object[] { _engine });
            Sprite.Visable = false;
        }

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

                float LesserOf(float one, float two) => one > two ? one : two;

                float throttleFloor = 0.1f;
                float throttleCap = 0.45f; //This value will be considered max throttle. This is so we can move from 100% forward to 50$ forward and 50% rotation.
                float momentiumRampUp = Engine.Settings.PlayerVelocityRampUp * epoch;
                float momentiumRampDown = Engine.Settings.PlayerVelocityRampDown * epoch;

                float targetForwardAmount = (Engine.Input.GetAnalogAxisValue(SiPlayerKey.Reverse, SiPlayerKey.Forward) / throttleCap).Clamp(-1, 1);


                #region Forward and Reverse.

                if (targetForwardAmount > throttleFloor)
                {
                    Sprite.Velocity.ForwardVelocity += momentiumRampUp; //Make player forward velocity build-up.
                }
                else if (targetForwardAmount < -throttleFloor)
                {
                    Sprite.Velocity.ForwardVelocity -= momentiumRampUp; //Make player reverse velocity build-up.
                }
                else //No forward input was received, ramp down the forward velocity.
                {
                    if (Math.Abs(momentiumRampDown) >= Math.Abs(Sprite.Velocity.ForwardVelocity))
                    {
                        Sprite.Velocity.ForwardVelocity = 0; //Don't overshoot the stop.
                    }
                    else Sprite.Velocity.ForwardVelocity -= Sprite.Velocity.ForwardVelocity > 0 ? momentiumRampDown : -momentiumRampDown;
                }

                #endregion

                #region Forward Speed-Boost.

                if (Engine.Input.IsKeyPressed(SiPlayerKey.SpeedBoost))
                {
                    if (_inputDelay.ElapsedMilliseconds > 200)
                    {
                        _engine.Player?.Sprite?.ToggleSpeedBoost();
                        _inputDelay.Restart();
                    }
                }

                if (Sprite.UseSpeedBoost && Sprite.Velocity.ForwardVelocity >= throttleFloor
                    && Sprite.Velocity.AvailableBoost > 0 && Sprite.Velocity.IsBoostCoolingDown == false)
                {
                    Sprite.Velocity.ForwardBoostVelocity += momentiumRampUp; //Make player forward velocity build-up.

                    //Consume boost:
                    Sprite.Velocity.AvailableBoost -= 10 * epoch;
                    if (Sprite.Velocity.AvailableBoost < 0)
                    {
                        Sprite.Velocity.AvailableBoost = 0;
                    }
                }
                else //No forward input was received, ramp down the forward velocity.
                {
                    if (Math.Abs(momentiumRampDown) >= Math.Abs(Sprite.Velocity.ForwardBoostVelocity))
                    {
                        Sprite.Velocity.ForwardBoostVelocity = 0; //Don't overshoot the stop.
                    }
                    else Sprite.Velocity.ForwardBoostVelocity -= momentiumRampDown;

                    //Rebuild boost if its not being used.
                    if ((Sprite.UseSpeedBoost == false || Sprite.Velocity.ForwardVelocity <= throttleFloor)
                        && Sprite.Velocity.AvailableBoost < Engine.Settings.MaxPlayerBoostAmount)
                    {
                        Sprite.Velocity.AvailableBoost += (10 * epoch);
                        if (Sprite.Velocity.AvailableBoost > Engine.Settings.MaxPlayerBoostAmount)
                        {
                            Sprite.Velocity.AvailableBoost = Engine.Settings.MaxPlayerBoostAmount;
                        }

                        if (Sprite.Velocity.IsBoostCoolingDown && Sprite.Velocity.AvailableBoost >= Engine.Settings.PlayerBoostRebuildFloor)
                        {
                            Sprite.Velocity.IsBoostCoolingDown = false;
                        }
                    }
                }

                if (Sprite.Velocity.AvailableBoost <= 0)
                {
                    Sprite.Velocity.AvailableBoost = 0;
                    Sprite.Velocity.IsBoostCoolingDown = true;
                }

                #endregion

                #region Laterial Strafing.

                float targetLateralAmount = (Engine.Input.GetAnalogAxisValue(SiPlayerKey.StrafeRight, SiPlayerKey.StrafeLeft) / throttleCap).Clamp(-1, 1);

                Debug.WriteLine($"{targetLateralAmount}");

                if (targetLateralAmount > throttleFloor)
                {
                    Sprite.Velocity.LateralVelocity += momentiumRampUp; //Make player forward velocity build-up.
                }
                else if (targetLateralAmount < -throttleFloor)
                {
                    Sprite.Velocity.LateralVelocity -= momentiumRampUp; //Make player reverse velocity build-up.
                }
                else //No lateral input was received, ramp down the lateral velocity.
                {
                    if (Math.Abs(momentiumRampDown) >= Math.Abs(Sprite.Velocity.LateralVelocity))
                    {
                        Sprite.Velocity.LateralVelocity = 0; //Don't overshoot the stop.
                    }
                    else Sprite.Velocity.LateralVelocity -= Sprite.Velocity.LateralVelocity > 0 ? momentiumRampDown : -momentiumRampDown;
                }

                #endregion 

                #region Rotation.

                //We are going to restrict the rotation speed to a percentage of velocity.
                var rotationSpeed = Engine.Settings.MaxPlayerRotationSpeedDegrees
                    * ((Sprite.Velocity.LateralVelocity + Sprite.Velocity.ForwardVelocity) / 2);

                if (Engine.Input.IsKeyPressed(SiPlayerKey.RotateCounterClockwise) && !Engine.Input.IsKeyPressed(SiPlayerKey.RotateClockwise))
                {
                    Sprite.Rotate(-(rotationSpeed > 1.0 ? rotationSpeed : 1.0f));
                }

                if (!Engine.Input.IsKeyPressed(SiPlayerKey.RotateCounterClockwise) && Engine.Input.IsKeyPressed(SiPlayerKey.RotateClockwise))
                {
                    Sprite.Rotate(rotationSpeed > 1.0 ? rotationSpeed : 1.0f);
                }

                #endregion

                #region Sounds and Animation.

                if (Sprite.Velocity.ForwardBoostVelocity >= throttleFloor)
                    Sprite.ShipEngineBoostSound.Play();
                else Sprite.ShipEngineBoostSound.Fade();

                if (Sprite.Velocity.ForwardVelocity >= throttleFloor)
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
                        && Sprite.UseSpeedBoost
                        && Sprite.Velocity.AvailableBoost > 0
                        && Sprite.Velocity.IsBoostCoolingDown == false;
                }

                #endregion
            }

            var displacementVector = Sprite.Velocity.MovementVector;

            //Scroll the background.
            Engine.Display.RenderWindowPosition += displacementVector;

            //Move the player in the direction of the background. This keeps the player visually in place, which is in the center screen.
            Sprite.Location += displacementVector;

            Sprite.RenewableResources.RenewAllResources(epoch);

            return displacementVector;
        }

        public void ResetAndShow()
        {
            Sprite.Reset();

            Engine.Sprites.PlayerStatsText.Visable = true;
            Engine.Sprites.RenderRadar = true;
            Sprite.Visable = true;
            Sprite.ShipEngineIdleSound.Play();
            Sprite.AllSystemsGoSound.Play();
        }

        public void Show()
        {
            Engine.Sprites.PlayerStatsText.Visable = true;
            Engine.Sprites.RenderRadar = true;
            Sprite.Visable = true;
            Sprite.ShipEngineIdleSound.Play();
            Sprite.AllSystemsGoSound.Play();
        }

        public void Hide()
        {
            Engine.Sprites.PlayerStatsText.Visable = false;
            Engine.Sprites.RenderRadar = false;
            Sprite.Visable = false;
            Sprite.ShipEngineIdleSound.Stop();
            Sprite.ShipEngineRoarSound.Stop();
        }
    }
}
