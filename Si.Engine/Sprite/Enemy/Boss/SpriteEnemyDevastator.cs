using Si.Engine.Sprite.Enemy.Boss._Superclass;
using Si.Engine.Sprite.Weapon;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Enemy.Boss
{
    /// <summary>
    /// 100% Experimental
    /// </summary>
    internal class SpriteEnemyDevastator : SpriteEnemyBossBase
    {
        public const int hullHealth = 100;
        public const int bountyMultiplier = 15;

        private readonly SpriteAttachment _leftCannon;
        private readonly SpriteAttachment _rightCannon;

        private SpriteAttachment _turret;

        private readonly SpriteAttachment _leftGun;
        private readonly SpriteAttachment _rightGun;

        private readonly SpriteAttachment _rightThrust;
        private readonly SpriteAttachment _leftThrust;

        private readonly float _initialMaxpeed;
        private readonly string _assetPath = @"Graphics\Enemy\Boss\Devastator\";

        public SpriteEnemyDevastator(EngineCore engine)
            : base(engine)
        {
            _leftCannon = Attach(_assetPath + "Gun.Cannon.Left.png", true, 3);
            _rightCannon = Attach(_assetPath + "Gun.Cannon.Right.png", true, 3);
            _leftGun = Attach(_assetPath + "Gun.Left.png", true, 3);
            _rightGun = Attach(_assetPath + "Gun.Right.png", true, 3);
            _leftThrust = Attach(_assetPath + "Jet.Left.png", true, 3);
            _rightThrust = Attach(_assetPath + "Jet.Right.png", true, 3);

            InitializeSpriteFromMetadata(@"Graphics\Enemy\Boss\Devastator\Hull.png");

            _initialMaxpeed = Velocity.MaximumSpeed;
        }

        public override void AfterCreate()
        {
            _turret = Attach(_assetPath + "Gun.Turret.png", true, 3);
            base.AfterCreate();
        }

        public override void VelocityChanged()
        {
            if (_leftThrust != null && _rightThrust != null)
            {
                bool visibleThrust = Velocity.ForwardVelocity > 0;

                if (_leftThrust.IsDeadOrExploded == false)
                {
                    _leftThrust.Visable = visibleThrust;
                }
                if (_rightThrust.IsDeadOrExploded == false)
                {
                    _rightThrust.Visable = visibleThrust;
                }
            }
        }

        public override void LocationChanged()
        {
            if (_leftGun != null && _rightGun != null)
            {
                if (_leftGun.IsDeadOrExploded == false)
                {
                    var pointLeft = SiPoint.PointFromAngleAtDistance360(Velocity.ForwardAngle - SiPoint.RADIANS_90, new SiPoint(25, 25));
                    _leftGun.Velocity.ForwardAngle.Degrees = Velocity.ForwardAngle.Degrees;
                    _leftGun.Location += pointLeft;
                }

                if (_rightGun.IsDeadOrExploded == false)
                {
                    var pointRight = SiPoint.PointFromAngleAtDistance360(Velocity.ForwardAngle + SiPoint.RADIANS_90, new SiPoint(25, 25));
                    _rightGun.Velocity.ForwardAngle.Degrees = Velocity.ForwardAngle.Degrees;
                    _rightGun.Location += pointRight;
                }

                if (_leftThrust.IsDeadOrExploded == false)
                {
                    var pointLeft = SiPoint.PointFromAngleAtDistance360(Velocity.ForwardAngle - SiPoint.DegreesToRadians(135), new SiPoint(35, 35));
                    _leftThrust.Velocity.ForwardAngle.Degrees = Velocity.ForwardAngle.Degrees;
                    _leftThrust.Location += pointLeft;
                }

                if (_rightThrust.IsDeadOrExploded == false)
                {
                    var pointRight = SiPoint.PointFromAngleAtDistance360(Velocity.ForwardAngle + SiPoint.DegreesToRadians(135), new SiPoint(35, 35));
                    _rightThrust.Velocity.ForwardAngle.Degrees = Velocity.ForwardAngle.Degrees;
                    _rightThrust.Location += pointRight;
                }

                if (_turret.IsDeadOrExploded == false)
                {
                    var pointRight = SiPoint.PointFromAngleAtDistance360(Velocity.ForwardAngle, new SiPoint(0, 0));
                    _turret.Velocity.ForwardAngle.Degrees = AngleTo360(_engine.Player.Sprite);
                    _turret.Location += pointRight;
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

        private const float baseDistanceToKeep = 200;
        private float distanceToKeep = baseDistanceToKeep * (SiRandom.NextFloat() + 1);
        private const float baseFallbackDistance = 800;
        private float fallbackDistance;
        private SiAngle fallToAngleRadians;
        private AIMode mode = AIMode.Approaching;
        private int roundsToFireBeforeTailing = 0;
        private int hpRemainingBeforeTailing = 0;

        public override void ApplyIntelligence(float epoch, SiPoint displacementVector)
        {
            base.ApplyIntelligence(epoch, displacementVector);

            float distanceToPlayer = SiPoint.DistanceTo(this, _engine.Player.Sprite);

            //We have no engines. :(
            if (_leftThrust.IsDeadOrExploded && _rightThrust.IsDeadOrExploded)
            {
                mode = AIMode.LameDuck;
            }

            //If we get down to one engine, slowly cut the max thrust to half of what it originally was. If we lose both, reduce it to 1.
            int thrustHandicap = (_leftThrust.IsDeadOrExploded ? 0 : 1) + (_rightThrust.IsDeadOrExploded ? 0 : 1);
            if (thrustHandicap == 1 && Velocity.MaximumSpeed > _initialMaxpeed / 2)
            {
                Velocity.MaximumSpeed -= 0.5f;
            }
            if (thrustHandicap == 0 && Velocity.MaximumSpeed > 1)
            {
                Velocity.MaximumSpeed -= 0.5f;
                if (Velocity.MaximumSpeed < 1)
                {
                    Velocity.MaximumSpeed = 1;
                }
            }

            if (mode == AIMode.LameDuck)
            {
                if (distanceToPlayer > 2500)
                {
                    Explode();
                }

                //Keep pointing at the player.
                var deltaAngle = DeltaAngleDegrees(_engine.Player.Sprite);

                if (deltaAngle.IsNotBetween(-10, 10))
                {
                    if (deltaAngle >= 0)
                    {
                        Velocity.ForwardAngle += 1;
                    }
                    else if (deltaAngle < 0)
                    {
                        Velocity.ForwardAngle -= 1;
                    }
                }

                //Try to stay close.
                if (distanceToPlayer > 300)
                {
                    Velocity.ForwardVelocity += 0.05f;
                    if (Velocity.ForwardVelocity > 1)
                    {
                        Velocity.ForwardVelocity = 1;
                    }
                }
                else
                {
                    //Slow to a stop when close.
                    Velocity.ForwardVelocity -= 0.05f;
                    if (Velocity.ForwardVelocity < 0)
                    {
                        Velocity.ForwardVelocity = 0;
                    }
                }
            }
            else if (mode == AIMode.Approaching)
            {
                if (distanceToPlayer > distanceToKeep)
                {
                    PointAtAndGoto(_engine.Player.Sprite);
                }
                else
                {
                    mode = AIMode.Tailing;
                    roundsToFireBeforeTailing = 15;
                    hpRemainingBeforeTailing = HullHealth;
                }
            }

            if (mode == AIMode.Tailing)
            {
                PointAtAndGoto(_engine.Player.Sprite);

                //Stay on the players tail.
                if (distanceToPlayer > distanceToKeep + 300)
                {
                    Velocity.ForwardVelocity = 1;
                    mode = AIMode.Approaching;
                }
                else
                {
                    Velocity.ForwardVelocity -= 0.05f;
                    if (Velocity.ForwardVelocity < 0)
                    {
                        Velocity.ForwardVelocity = 0;
                    }
                }

                //We we get too close, do too much damage or they fire at us enough, they fall back and come in again
                if (distanceToPlayer < distanceToKeep / 2.0
                    || hpRemainingBeforeTailing - HullHealth > 2
                    || roundsToFireBeforeTailing <= 0)
                {
                    Velocity.ForwardVelocity = 1;
                    mode = AIMode.MovingToFallback;
                    fallToAngleRadians = Velocity.ForwardAngle + new SiAngle(180.0f + SiRandom.Between(0, 10)).Radians;
                    fallbackDistance = baseFallbackDistance * (SiRandom.NextFloat() + 1);
                }
            }

            if (mode == AIMode.MovingToFallback)
            {
                var deltaAngle = Velocity.ForwardAngle - fallToAngleRadians;

                if (deltaAngle.Degrees > 10)
                {
                    if (deltaAngle.Degrees >= 180.0) //We might as well turn around clock-wise
                    {
                        Velocity.ForwardAngle += 1;
                    }
                    else if (deltaAngle.Degrees < 180.0) //We might as well turn around counter clock-wise
                    {
                        Velocity.ForwardAngle -= 1;
                    }
                }

                if (distanceToPlayer > fallbackDistance)
                {
                    mode = AIMode.MovingToApproach;
                }
            }

            if (mode == AIMode.MovingToApproach)
            {
                var deltaAngle = DeltaAngleDegrees(_engine.Player.Sprite);

                if (deltaAngle.IsNotBetween(-10, 10))
                {
                    if (deltaAngle >= 0)
                    {
                        Velocity.ForwardAngle += 1;
                    }
                    else if (deltaAngle < 0)
                    {
                        Velocity.ForwardAngle -= 1;
                    }
                }
                else
                {
                    mode = AIMode.Approaching;
                    distanceToKeep = baseDistanceToKeep * (SiRandom.NextFloat() + 1);
                }
            }

            if (IsHostile)
            {
                if (distanceToPlayer < 1000 && (_rightGun?.IsDeadOrExploded == false || _leftGun?.IsDeadOrExploded == false))
                {
                    if (distanceToPlayer > 500 && HasWeaponAndAmmo<WeaponDualVulcanCannon>())
                    {
                        bool isPointingAtPlayer = IsPointingAt(_engine.Player.Sprite, 2.0f);
                        if (isPointingAtPlayer)
                        {
                            if (FireWeapon<WeaponDualVulcanCannon>())
                            {
                                roundsToFireBeforeTailing++;
                            }
                        }
                    }
                    else if (distanceToPlayer > 0 && HasWeaponAndAmmo<WeaponVulcanCannon>())
                    {
                        bool isPointingAtPlayer = IsPointingAt(_engine.Player.Sprite, 2.0f);
                        if (isPointingAtPlayer)
                        {
                            if (FireWeapon<WeaponVulcanCannon>())
                            {
                                roundsToFireBeforeTailing++;
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
