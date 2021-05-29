using AI2D.Actors.Enemies.AI;
using AI2D.Engine;
using AI2D.Types;
using AI2D.Weapons;
using Algorithms;
using System.Drawing;
using System.Linq;

namespace AI2D.Actors.Enemies
{
    /// <summary>
    /// 100% Experimental
    /// </summary>
    public class EnemyScarab : EnemyBase
    {
        public const int ScoreMultiplier = 15;

        private ActorAttachment _leftCannon;
        private ActorAttachment _rightCannon;

        private ActorAttachment _turret;

        private ActorAttachment _leftGun;
        private ActorAttachment _rightGun;

        private ActorAttachment _rightThrust;
        private ActorAttachment _leftThrust;

        private double _initialMaxpeed;

        string _imagesPath = @"..\..\..\Assets\Graphics\Enemy\Scarab\";

        public EnemyScarab(Core core)
            : base(core, EnemyBase.GetGenericHP(), ScoreMultiplier)
        {
            this.ThrustAnimation.QueueForDelete();

            _leftCannon  = this.Attach(_imagesPath + "Scarab.Gun.Cannon.Left.png", true, 3);
            _rightCannon = this.Attach(_imagesPath + "Scarab.Gun.Cannon.Right.png", true, 3);
            _leftGun = this.Attach(_imagesPath + "Scarab.Gun.Left.png", true, 3);
            _rightGun = this.Attach(_imagesPath + "Scarab.Gun.Right.png", true, 3);
            _leftThrust = this.Attach(_imagesPath + "Scarab.Jet.Left.png", true, 3);
            _rightThrust = this.Attach(_imagesPath + "Scarab.Jet.Right.png", true, 3);

            base.SetHitPoints(Utility.Random.Next(Constants.Limits.MinEnemyHealth, Constants.Limits.MaxEnemyHealth));

            _initialMaxpeed = Utility.Random.Next(Constants.Limits.MaxSpeed - 2, Constants.Limits.MaxSpeed); //Upper end of the speed spectrum

            Velocity.MaxSpeed = _initialMaxpeed;

            SetImage(_imagesPath + "Scarab.Hull.png");

            AddSecondaryWeapon(new WeaponVulcanCannon(_core)
            {
                RoundQuantity = 1000,
                FireDelayMilliseconds = 250
            });

            AddSecondaryWeapon(new WeaponDualVulcanCannon(_core)
            {
                RoundQuantity = 500,
                FireDelayMilliseconds = 500
            });

            SelectSecondaryWeapon(typeof(WeaponVulcanCannon));
        }

        public override void BeforeCreate()
        {
            base.BeforeCreate();
        }

        public override void AfterCreate()
        {
            _turret = this.Attach(_imagesPath + "Scarab.Gun.Turret.png", true, 3);
            base.AfterCreate();
        }

        public override void VelocityChanged()
        {
            if (_leftThrust != null && _rightThrust != null)
            {
                bool visibleThrust = (Velocity.ThrottlePercentage > 0);

                if (_leftThrust.IsDead == false)
                {
                    _leftThrust.Visable = visibleThrust;
                }
                if (_rightThrust.IsDead == false)
                {
                    _rightThrust.Visable = visibleThrust;
                }
            }
        }

        public override void PositionChanged()
        {
            if (_leftGun != null && _rightGun != null)
            {
                if (_leftGun.IsDead == false)
                {
                    var pointLeft = Utility.AngleFromPointAtDistance(base.Velocity.Angle - 90, new Point<double>(25, 25));
                    _leftGun.Velocity.Angle.Degrees = this.Velocity.Angle.Degrees;
                    _leftGun.X = this.X + pointLeft.X;
                    _leftGun.Y = this.Y + pointLeft.Y;
                }

                if (_rightGun.IsDead == false)
                {
                    var pointRight = Utility.AngleFromPointAtDistance(base.Velocity.Angle + 90, new Point<double>(25, 25));
                    _rightGun.Velocity.Angle.Degrees = this.Velocity.Angle.Degrees;
                    _rightGun.X = this.X + pointRight.X;
                    _rightGun.Y = this.Y + pointRight.Y;
                }

                if (_leftThrust.IsDead == false)
                {
                    var pointLeft = Utility.AngleFromPointAtDistance(base.Velocity.Angle - 135, new Point<double>(35, 35));
                    _leftThrust.Velocity.Angle.Degrees = this.Velocity.Angle.Degrees;
                    _leftThrust.X = this.X + pointLeft.X;
                    _leftThrust.Y = this.Y + pointLeft.Y;
                }

                if (_rightThrust.IsDead == false)
                {
                    var pointRight = Utility.AngleFromPointAtDistance(base.Velocity.Angle + 135, new Point<double>(35, 35));
                    _rightThrust.Velocity.Angle.Degrees = this.Velocity.Angle.Degrees;
                    _rightThrust.X = this.X + pointRight.X;
                    _rightThrust.Y = this.Y + pointRight.Y;
                }

                if (_turret.IsDead == false)
                {
                    var pointRight = Utility.AngleFromPointAtDistance(base.Velocity.Angle, new Point<double>(0, 0));
                    _turret.Velocity.Angle.Degrees = this.AngleTo(_core.Actors.Player);
                    _turret.X = this.X + pointRight.X;
                    _turret.Y = this.Y + pointRight.Y;
                }
            }
        }

        #region Artificial Intelligence.

        private enum AIMode
        {
            Approaching,
            Tailing,
            MovingToFallback,
            MovingToApproach,
            LameDuck
        }

        private const double baseDistanceToKeep = 200;
        private double distanceToKeep = baseDistanceToKeep * (Utility.Random.NextDouble() + 1);
        private const double baseFallbackDistance = 800;
        private double fallbackDistance;
        private Angle<double> fallToAngle;
        private AIMode mode = AIMode.Approaching;
        private int bulletsRemainingBeforeTailing = 0;
        private int hpRemainingBeforeTailing = 0;

        public override void ApplyIntelligence(Point<double> frameAppliedOffset)
        {
            base.ApplyIntelligence(frameAppliedOffset);

            double distanceToPlayer = Utility.DistanceTo(this, _core.Actors.Player);

            //We have no engines. :(
            if (_leftThrust.IsDead && _rightThrust.IsDead)
            {
                mode = AIMode.LameDuck;
            }

            //If we get down to one engine, slowly cut the max thrust to half of what it originally was. If we lose both, reduce it to 1.
            int thrustHandicap = (_leftThrust.IsDead ? 0 : 1) + (_rightThrust.IsDead ? 0 : 1);
            if (thrustHandicap == 1 && Velocity.MaxSpeed > _initialMaxpeed / 2)
            {
                Velocity.MaxSpeed -= 0.5;
            }
            if (thrustHandicap == 0 && Velocity.MaxSpeed > 1)
            {
                Velocity.MaxSpeed -= 0.5;
                if (Velocity.MaxSpeed < 1)
                {
                    Velocity.MaxSpeed = 1;
                }
            }

            if (mode == AIMode.LameDuck)
            {
                if (distanceToPlayer > 2500)
                {
                    Explode();
                }

                //Keep pointing at the player.
                var deltaAngle = DeltaAngle(_core.Actors.Player);

                if (deltaAngle > 10)
                {
                    if (deltaAngle >= 180.0)
                    {
                        Velocity.Angle += 1;
                    }
                    else if (deltaAngle < 180.0)
                    {
                        Velocity.Angle -= 1;
                    }
                }

                //Try to stay close.
                if (distanceToPlayer > 300)
                {
                    Velocity.ThrottlePercentage += 0.05;
                    if (Velocity.ThrottlePercentage > 1)
                    {
                        Velocity.ThrottlePercentage = 1;
                    }
                }
                else
                {
                    //Slow to a stop when close.
                    Velocity.ThrottlePercentage -= 0.05;
                    if (Velocity.ThrottlePercentage < 0)
                    {
                        Velocity.ThrottlePercentage = 0;
                    }
                }
            }
            else if (mode == AIMode.Approaching)
            {
                if (distanceToPlayer > distanceToKeep)
                {
                    MoveInDirectionOf(_core.Actors.Player);
                }
                else
                {
                    mode = AIMode.Tailing;
                    bulletsRemainingBeforeTailing = this.TotalAvailableSecondaryWeaponRounds();
                    hpRemainingBeforeTailing = this.HitPoints;
                }
            }

            if (mode == AIMode.Tailing)
            {
                MoveInDirectionOf(_core.Actors.Player);

                //Stay on the players tail.
                if (distanceToPlayer > distanceToKeep + 300)
                {
                    Velocity.ThrottlePercentage = 1;
                    mode = AIMode.Approaching;
                }
                else
                {
                    Velocity.ThrottlePercentage -= 0.05;
                    if (Velocity.ThrottlePercentage < 0)
                    {
                        Velocity.ThrottlePercentage = 0;
                    }
                }

                //We we get too close, do too much damage or they fire at us enough, they fall back and come in again
                if (distanceToPlayer < (distanceToKeep / 2.0)
                    || (hpRemainingBeforeTailing - this.HitPoints) > 2
                    || (bulletsRemainingBeforeTailing - this.TotalAvailableSecondaryWeaponRounds()) > 15)
                {
                    Velocity.ThrottlePercentage = 1;
                    mode = AIMode.MovingToFallback;
                    fallToAngle = Velocity.Angle + (180.0 + Utility.RandomNumberNegative(0, 10));
                    fallbackDistance = baseFallbackDistance * (Utility.Random.NextDouble() + 1);
                }
            }

            if (mode == AIMode.MovingToFallback)
            {
                var deltaAngle = Velocity.Angle - fallToAngle;

                if (deltaAngle.Degrees > 10)
                {
                    if (deltaAngle.Degrees >= 180.0) //We might as well turn around clock-wise
                    {
                        Velocity.Angle += 1;
                    }
                    else if (deltaAngle.Degrees < 180.0) //We might as well turn around counter clock-wise
                    {
                        Velocity.Angle -= 1;
                    }
                }

                if (distanceToPlayer > fallbackDistance)
                {
                    mode = AIMode.MovingToApproach;
                }
            }

            if (mode == AIMode.MovingToApproach)
            {
                var deltaAngle = DeltaAngle(_core.Actors.Player);

                if (deltaAngle > 10)
                {
                    if (deltaAngle >= 180.0)
                    {
                        Velocity.Angle += 1;
                    }
                    else if (deltaAngle < 180.0)
                    {
                        Velocity.Angle -= 1;
                    }
                }
                else
                {
                    mode = AIMode.Approaching;
                    distanceToKeep = baseDistanceToKeep * (Utility.Random.NextDouble() + 1);
                }
            }

            if (distanceToPlayer < 700 && (_rightGun.IsDead == false || _leftGun.IsDead == false))
            {
                if (distanceToPlayer > 200 && HasSecondaryWeaponAndAmmo(typeof(WeaponVulcanCannon)))
                {
                    bool isPointingAtPlayer = IsPointingAt(_core.Actors.Player, 8.0);
                    if (isPointingAtPlayer)
                    {
                        SelectSecondaryWeapon(typeof(WeaponVulcanCannon));
                        SelectedSecondaryWeapon?.Fire();
                    }
                }
                else if (distanceToPlayer > 0 && HasSecondaryWeaponAndAmmo(typeof(WeaponDualVulcanCannon)))
                {
                    bool isPointingAtPlayer = IsPointingAt(_core.Actors.Player, 8.0);
                    if (isPointingAtPlayer)
                    {
                        SelectSecondaryWeapon(typeof(WeaponDualVulcanCannon));
                        SelectedSecondaryWeapon?.Fire();
                    }
                }
            }
        }

        #endregion
    }
}
