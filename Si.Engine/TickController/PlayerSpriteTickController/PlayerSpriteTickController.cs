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
        private bool _allowLockPlayerAngleToNearbyEnemy = true;
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
            var displacementVector = new SiPoint();

            Sprite.IsLockedOnSoft = false;
            Sprite.IsLockedOnHard = false;

            if (Sprite.Visable)
            {
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
                if (Engine.Input.IsKeyPressed(SiPlayerKey.SpeedBoost))
                {
                    if (_inputDelay.ElapsedMilliseconds > 200)
                    {
                        _engine.Player?.Sprite?.ToggleSpeedBoost();
                        _inputDelay.Restart();
                    }
                }

                //Sprite.PrimaryWeapon.ApplyIntelligence();
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

                #region LockPlayerAngleToNearbyEnemy.
                //This needs some work. It works, but its odd - the movement is rigid.

                if (Engine.Settings.LockPlayerAngleToNearbyEnemy)
                {
                    if (Engine.Input.IsKeyPressed(SiPlayerKey.RotateCounterClockwise) == false
                        && Engine.Input.IsKeyPressed(SiPlayerKey.RotateClockwise) == false)
                    {
                        if (_allowLockPlayerAngleToNearbyEnemy)
                        {
                            var enemies = Engine.Sprites.Enemies.Visible();
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

                float LesserOf(float one, float two) => one > two ? one : two;

                float momentiumRampUp = Engine.Settings.PlayerThrustRampUp * epoch;
                float momentiumRampDown = Engine.Settings.PlayerThrustRampDown * epoch;

                #region Forward and Reverse Momentium.

                float targetForwardAmount = Math.Abs(Engine.Input.InputAmount(SiPlayerKey.Forward));
                if (targetForwardAmount > 0)
                {
                    //Handle adding forward momentium.
                    if (targetForwardAmount > Sprite.Velocity.ForwardMomentium)
                    {
                        Sprite.Velocity.ForwardMomentium += LesserOf(momentiumRampUp, targetForwardAmount);
                    }
                    //Handle backing off of forward momentium.
                    else if (targetForwardAmount < Sprite.Velocity.ForwardMomentium - 0.1f)
                    {
                        if (momentiumRampDown > Math.Abs(Sprite.Velocity.ForwardMomentium))
                        {
                            Sprite.Velocity.ForwardMomentium = 0; //Don't overshoot the stop.
                        }
                        else Sprite.Velocity.ForwardMomentium -= LesserOf(momentiumRampDown, targetForwardAmount);
                    }
                }

                float targetReverseAmount = Math.Abs(Engine.Input.InputAmount(SiPlayerKey.Reverse));
                if (targetReverseAmount > 0)
                {
                    //Handle adding reverse momentium.
                    if (targetReverseAmount > -Sprite.Velocity.ForwardMomentium)
                    {
                        Sprite.Velocity.ForwardMomentium -= LesserOf(momentiumRampUp, targetReverseAmount);
                    }
                    //Handle backing off of reverse momentium.
                    else if (targetReverseAmount < -Sprite.Velocity.ForwardMomentium + -0.1f)
                    {
                        if (momentiumRampDown > Math.Abs(Sprite.Velocity.ForwardMomentium))
                        {
                            Sprite.Velocity.ForwardMomentium = 0; //Don't overshoot the stop.
                        }
                        else Sprite.Velocity.ForwardMomentium += LesserOf(momentiumRampDown, targetReverseAmount);
                    }
                }

                //If there is no forward or reverse thrust, then back off of the momentium.
                if (targetForwardAmount == 0 && targetReverseAmount == 0)
                {
                    if (Sprite.Velocity.ForwardMomentium > 0)
                    {
                        Sprite.Velocity.ForwardMomentium -= momentiumRampDown;
                        if (Sprite.Velocity.ForwardMomentium < 0) //Make sure we do not overshoot the stop.
                        {
                            Sprite.Velocity.ForwardMomentium = 0;
                        }
                    }
                    else if (Sprite.Velocity.ForwardMomentium < 0)
                    {
                        Sprite.Velocity.ForwardMomentium += momentiumRampDown;
                        if (Sprite.Velocity.ForwardMomentium > 0) //Make sure we do not overshoot the stop.
                        {
                            Sprite.Velocity.ForwardMomentium = 0;
                        }
                    }
                }

                //Slow the crawl to a stop.
                if (Sprite.Velocity.ForwardMomentium.IsBetween(-0.0001f, 0.0001f))
                {
                    Sprite.Velocity.ForwardMomentium = 0;
                }

                #endregion

                #region Forward Boost (NOT YET WORKING REVIEWED FOR GAMPAD).

                float forwardBoostThrustToAdd = Sprite.Velocity.ForwardBoostMomentium == 0 ? Engine.Settings.PlayerThrustRampUp
                    : Engine.Settings.PlayerThrustRampUp * (1 - Sprite.Velocity.ForwardBoostMomentium);

                //Make player forward momentium "build up" and fade-out.
                if (_engine.Player?.Sprite.UseSpeedBoost == true && Engine.Input.IsKeyPressed(SiPlayerKey.Forward)
                    && Sprite.Velocity.AvailableBoost > 0 && Sprite.Velocity.IsBoostCoolingDown == false)
                {
                    Sprite.Velocity.ForwardBoostMomentium = (Sprite.Velocity.ForwardBoostMomentium + forwardBoostThrustToAdd).Clamp(-1, 1);

                    Sprite.Velocity.AvailableBoost -= Sprite.Velocity.MaximumBoostSpeed * Sprite.Velocity.ForwardBoostMomentium;
                    if (Sprite.Velocity.AvailableBoost < 0)
                    {
                        Sprite.Velocity.AvailableBoost = 0;
                    }

                }
                else
                {
                    float thrustToRemove = Sprite.Velocity.ForwardBoostMomentium == 0 ? Engine.Settings.PlayerThrustRampDown
                        : Engine.Settings.PlayerThrustRampDown * Sprite.Velocity.ForwardBoostMomentium;

                    if (Math.Abs(thrustToRemove) + 0.1 >= Math.Abs(Sprite.Velocity.ForwardBoostMomentium))
                    {
                        Sprite.Velocity.ForwardBoostMomentium = 0; //Don't overshoot the stop.
                    }
                    else Sprite.Velocity.ForwardBoostMomentium -= thrustToRemove;

                    if (_engine.Player?.Sprite.UseSpeedBoost != true && Sprite.Velocity.AvailableBoost < Engine.Settings.MaxPlayerBoostAmount)
                    {
                        Sprite.Velocity.AvailableBoost = (Sprite.Velocity.AvailableBoost + 1000.0f * epoch / 1000.0f).Clamp(0, Engine.Settings.MaxPlayerBoostAmount);

                        if (Sprite.Velocity.IsBoostCoolingDown && Sprite.Velocity.AvailableBoost >= Engine.Settings.PlayerBoostRebuildFloor)
                        {
                            Sprite.Velocity.IsBoostCoolingDown = false;
                        }
                    }
                }

                #endregion

                #region Strafing / Lateral Momentium.

                var strafeAngle = SiPoint.PointFromAngleAtDistance360(new SiAngle(Sprite.Velocity.Angle - SiPoint.RADIANS_90), new SiPoint(1, 1));

                float targetStrafeRightAmount = Math.Abs(Engine.Input.InputAmount(SiPlayerKey.StrafeLeft));
                if (targetStrafeRightAmount > 0)
                {
                    //Handle adding Right lateral momentium.
                    if (targetStrafeRightAmount > Sprite.Velocity.LateralMomentium)
                    {
                        Sprite.Velocity.LateralMomentium += momentiumRampUp;
                    }
                    //Handle backing off of Right lateral momentium.
                    else if (targetStrafeRightAmount < Sprite.Velocity.LateralMomentium + 0.1f)
                    {
                        if (momentiumRampDown > Math.Abs(Sprite.Velocity.LateralMomentium))
                        {
                            Sprite.Velocity.LateralMomentium = 0; //Don't overshoot the stop.
                        }
                        else Sprite.Velocity.LateralMomentium -= momentiumRampDown;
                    }
                }

                float targetStrafeLeftAmount = Math.Abs(Engine.Input.InputAmount(SiPlayerKey.StrafeRight));
                if (targetStrafeLeftAmount > 0)
                {
                    //Handle adding Left lateral momentium.
                    if (targetStrafeLeftAmount > -Sprite.Velocity.LateralMomentium)
                    {
                        Sprite.Velocity.LateralMomentium -= momentiumRampUp;
                    }
                    //Handle backing off of Left lateral momentium.
                    else if (targetStrafeLeftAmount < -Sprite.Velocity.LateralMomentium + -0.1f)
                    {
                        if (momentiumRampDown > Math.Abs(Sprite.Velocity.LateralMomentium))
                        {
                            Sprite.Velocity.LateralMomentium = 0; //Don't overshoot the stop.
                        }
                        else Sprite.Velocity.LateralMomentium += momentiumRampDown;
                    }
                }

                //If there is no forward or reverse thrust, then back off of the momentium.
                if (targetStrafeLeftAmount == 0 && targetStrafeRightAmount == 0)
                {
                    if (Sprite.Velocity.LateralMomentium > 0)
                    {
                        Sprite.Velocity.LateralMomentium -= momentiumRampDown;
                        if (Sprite.Velocity.LateralMomentium < 0) //Make sure we do not overshoot the stop.
                        {
                            Sprite.Velocity.LateralMomentium = 0;
                        }
                    }
                    else if (Sprite.Velocity.LateralMomentium < 0)
                    {
                        Sprite.Velocity.LateralMomentium += momentiumRampDown;
                        if (Sprite.Velocity.LateralMomentium > 0) //Make sure we do not overshoot the stop.
                        {
                            Sprite.Velocity.LateralMomentium = 0;
                        }
                    }
                }

                //Slow the lateral crawl to a stop.
                if (Sprite.Velocity.LateralMomentium.IsBetween(-0.0001f, 0.0001f))
                {
                    Sprite.Velocity.LateralMomentium = 0;
                }

                #endregion

                if (Sprite.Velocity.AvailableBoost <= 0)
                {
                    Sprite.Velocity.AvailableBoost = 0;
                    Sprite.Velocity.IsBoostCoolingDown = true;
                }

                var totalForwardMomentium =
                    Sprite.Velocity.MaximumSpeed * Sprite.Velocity.ForwardMomentium
                    + Sprite.Velocity.MaximumBoostSpeed * Sprite.Velocity.ForwardBoostMomentium;

                displacementVector +=
                    (Sprite.Velocity.Angle * totalForwardMomentium) + //Forward / Reverse.
                    (strafeAngle * Sprite.Velocity.MaximumSpeed * Sprite.Velocity.LateralMomentium); //Left/Right Strafe.

                //We are going to restrict the rotation speed to a percentage of momentium.
                var rotationSpeed = Engine.Settings.MaxPlayerRotationSpeedDegrees
                    * ((Sprite.Velocity.LateralMomentium + Sprite.Velocity.ForwardMomentium) / 2);

                float rotateClockwiseAmount = Math.Abs(Engine.Input.InputAmount(SiPlayerKey.RotateCounterClockwise));
                float rotateCounterClockwiseAmount = Math.Abs(Engine.Input.InputAmount(SiPlayerKey.RotateClockwise));


                if (rotateClockwiseAmount > 0 && rotateCounterClockwiseAmount == 0)
                {
                    Sprite.Rotate(-((rotationSpeed > 1.0 ? rotationSpeed : 1.0f) * rotateClockwiseAmount));
                }
                if (rotateClockwiseAmount == 0 && rotateCounterClockwiseAmount > 0)
                {
                    Sprite.Rotate((rotationSpeed > 1.0 ? rotationSpeed : 1.0f) * rotateCounterClockwiseAmount);
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
                    Sprite.ThrustAnimation.Visable = Engine.Input.IsKeyPressed(SiPlayerKey.Forward);
                }

                if (Sprite.BoostAnimation != null)
                {
                    Sprite.BoostAnimation.Visable =
                        _engine.Player?.Sprite.UseSpeedBoost == true
                        && Engine.Input.IsKeyPressed(SiPlayerKey.Forward)
                        && Sprite.Velocity.AvailableBoost > 0 && Sprite.Velocity.IsBoostCoolingDown == false;
                }

                #endregion
            }

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
