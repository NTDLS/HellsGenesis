using Si.Engine.Sprite.Player._Superclass;
using Si.Engine.TickController._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;
using System;
using static Si.Library.SiConstants;

namespace Si.Engine.TickController.PlayerSpriteTickController
{
    /// <summary>
    /// This is the controller for the single local player.
    /// </summary>
    public class PlayerSpriteTickController : PlayerSpriteTickControllerBase<SpritePlayerBase>
    {
        private readonly EngineCore _engine;

        public SpritePlayerBase Sprite { get; set; }

        public PlayerSpriteTickController(EngineCore engine)
            : base(engine)
        {
            _engine = engine;
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
                #region Weapons Fire.

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

                #region Forward and Reverse.

                float forwardVelocityToAdd = Sprite.Velocity.ForwardVelocity == 0 ? Engine.Settings.PlayerVelocityRampUp
                    : Engine.Settings.PlayerVelocityRampUp * (1 - Sprite.Velocity.ForwardVelocity);

                //Make player forward velocity "build up" and fade-out.
                if (Engine.Input.IsKeyPressed(SiPlayerKey.Forward))
                {
                    Sprite.Velocity.ForwardVelocity = (Sprite.Velocity.ForwardVelocity + forwardVelocityToAdd).Clamp(-1, 1);
                }
                else if (Engine.Input.IsKeyPressed(SiPlayerKey.Reverse))
                {
                    Sprite.Velocity.ForwardVelocity = (Sprite.Velocity.ForwardVelocity - forwardVelocityToAdd).Clamp(-1, 1);
                }
                else
                {
                    float velocityToRemove = Sprite.Velocity.ForwardVelocity == 0 ? Engine.Settings.PlayerVelocityRampDown
                        : Engine.Settings.PlayerVelocityRampDown * Sprite.Velocity.ForwardVelocity;

                    if (Math.Abs(velocityToRemove) >= Math.Abs(Sprite.Velocity.ForwardVelocity))
                    {
                        Sprite.Velocity.ForwardVelocity = 0; //Don't overshoot the stop.
                    }
                    else Sprite.Velocity.ForwardVelocity -= velocityToRemove;
                }

                #endregion

                #region Forward Speed-Boost.

                float forwardBoostVelocityToAdd = Sprite.Velocity.ForwardBoostVelocity == 0 ? Engine.Settings.PlayerVelocityRampUp
                    : Engine.Settings.PlayerVelocityRampUp * (1 - Sprite.Velocity.ForwardBoostVelocity);

                //Make player forward velocity "build up" and fade-out.
                if (Engine.Input.IsKeyPressed(SiPlayerKey.SpeedBoost) && Engine.Input.IsKeyPressed(SiPlayerKey.Forward)
                    && Sprite.Velocity.AvailableBoost > 0 && Sprite.Velocity.IsBoostCoolingDown == false)
                {
                    Sprite.Velocity.ForwardBoostVelocity = (Sprite.Velocity.ForwardBoostVelocity + forwardBoostVelocityToAdd).Clamp(-1, 1);

                    Sprite.Velocity.AvailableBoost -= Sprite.Velocity.MaximumBoostSpeed * Sprite.Velocity.ForwardBoostVelocity;
                    if (Sprite.Velocity.AvailableBoost < 0)
                    {
                        Sprite.Velocity.AvailableBoost = 0;
                    }

                }
                else
                {
                    float velocityToRemove = Sprite.Velocity.ForwardBoostVelocity == 0 ? Engine.Settings.PlayerVelocityRampDown
                        : Engine.Settings.PlayerVelocityRampDown * Sprite.Velocity.ForwardBoostVelocity;

                    if (Math.Abs(velocityToRemove) + 0.1 >= Math.Abs(Sprite.Velocity.ForwardBoostVelocity))
                    {
                        Sprite.Velocity.ForwardBoostVelocity = 0; //Don't overshoot the stop.
                    }
                    else Sprite.Velocity.ForwardBoostVelocity -= velocityToRemove;

                    if (Engine.Input.IsKeyPressed(SiPlayerKey.SpeedBoost) == false
                        && Sprite.Velocity.AvailableBoost < Engine.Settings.MaxPlayerBoostAmount)
                    {
                        Sprite.Velocity.AvailableBoost = (Sprite.Velocity.AvailableBoost + 1000.0f * epoch / 1000.0f).Clamp(0, Engine.Settings.MaxPlayerBoostAmount);

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

                #region Strafing.

                Sprite.Velocity.LateralAngle.Radians = Sprite.Velocity.ForwardAngle.Radians - SiPoint.RADIANS_90;

                float strafeVelocityToAdd = Sprite.Velocity.LateralVelocity == 0 ? Engine.Settings.PlayerVelocityRampUp
                    : Engine.Settings.PlayerVelocityRampUp * (1 - Sprite.Velocity.LateralVelocity);

                //Make player lateral velocity "build up" and fade-out.
                if (Engine.Input.IsKeyPressed(SiPlayerKey.StrafeLeft) && !Engine.Input.IsKeyPressed(SiPlayerKey.StrafeRight))
                {
                    Sprite.Velocity.LateralVelocity = (Sprite.Velocity.LateralVelocity + strafeVelocityToAdd).Clamp(-1, 1);
                }
                else if (!Engine.Input.IsKeyPressed(SiPlayerKey.StrafeLeft) && Engine.Input.IsKeyPressed(SiPlayerKey.StrafeRight))
                {
                    Sprite.Velocity.LateralVelocity = (Sprite.Velocity.LateralVelocity - strafeVelocityToAdd).Clamp(-1, 1);
                }
                else //Ramp down to a stop:
                {
                    float velocityToRemove = Sprite.Velocity.LateralVelocity == 0 ? Engine.Settings.PlayerVelocityRampDown
                        : Engine.Settings.PlayerVelocityRampDown * Sprite.Velocity.LateralVelocity;

                    if (Math.Abs(velocityToRemove) >= Math.Abs(Sprite.Velocity.LateralVelocity))
                    {
                        Sprite.Velocity.LateralVelocity = 0; //Don't overshoot the stop.
                    }
                    else Sprite.Velocity.LateralVelocity -= velocityToRemove;
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

                if (Sprite.Velocity.ForwardBoostVelocity > 0.1)
                {
                    Sprite.ShipEngineBoostSound.Play();
                }
                else
                {
                    Sprite.ShipEngineBoostSound.Fade();
                }

                if (Sprite.Velocity.ForwardVelocity > 0.1)
                {
                    Sprite.ShipEngineRoarSound.Play();
                }
                else
                {
                    Sprite.ShipEngineRoarSound.Fade();
                }

                if (Sprite.ThrusterAnimation != null)
                {
                    Sprite.ThrusterAnimation.Visable = Engine.Input.IsKeyPressed(SiPlayerKey.Forward);
                }

                if (Sprite.BoostAnimation != null)
                {
                    Sprite.BoostAnimation.Visable =
                        Engine.Input.IsKeyPressed(SiPlayerKey.SpeedBoost)
                        && Engine.Input.IsKeyPressed(SiPlayerKey.Forward)
                        && Sprite.Velocity.AvailableBoost > 0 && Sprite.Velocity.IsBoostCoolingDown == false;
                }

                #endregion
            }

            var displacementVector = Sprite.Velocity.Vector;

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
