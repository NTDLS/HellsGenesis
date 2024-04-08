namespace Si.Engine.Sprite.Enemy.Boss
{
    /*
    /// <summary>
    /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    /// !!! This is OLD code and is provided as an example, this should not be used !!!
    /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    /// </summary>
    internal class SpriteEnemySpectre : SpriteEnemyBossBase
    {
        public const int hullHealth = 100;
        public const int bountyMultiplier = 15;

        private readonly SpriteAttachment _leftGun;
        private readonly SpriteAttachment _rightGun;
        private readonly SpriteAttachment _rightThrust;
        private readonly SpriteAttachment _leftThrust;

        private readonly float _initialMaxpeed;
        private readonly string _assetPath = @"Sprites\Enemy\Boss\Spectre\";

        public SpriteEnemySpectre(EngineCore engine)
            : base(engine)
        {
            _leftGun = Attach(_assetPath + "Gun.Left.png");
            _rightGun = Attach(_assetPath + "Gun.Right.png");
            _leftThrust = Attach(_assetPath + "Jet.png");
            _rightThrust = Attach(_assetPath + "Jet.png");

            SetImageAndLoadMetadata(@"Sprites\Enemy\Boss\Spectre\Hull.png");

            _initialMaxpeed = Velocity.MaximumSpeed;
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
                    var pointLeft = SiPoint.PointFromAngleAtDistance360(Direction - SiPoint.RADIANS_90, new SiPoint(25, 25));
                    _leftGun.Direction.Degrees = Direction.Degrees;
                    _leftGun.Location += pointLeft;
                }

                if (_rightGun.IsDeadOrExploded == false)
                {
                    var pointRight = SiPoint.PointFromAngleAtDistance360(Direction + SiPoint.RADIANS_90, new SiPoint(25, 25));
                    _rightGun.Direction.Degrees = Direction.Degrees;
                    _rightGun.Location += pointRight;
                }

                if (_leftThrust.IsDeadOrExploded == false)
                {
                    var pointLeft = SiPoint.PointFromAngleAtDistance360(Direction - SiPoint.DegreesToRadians(135), new SiPoint(35, 35));
                    _leftThrust.Direction.Degrees = Direction.Degrees;
                    _leftThrust.Location += pointLeft;
                }

                if (_rightThrust.IsDeadOrExploded == false)
                {
                    var pointRight = SiPoint.PointFromAngleAtDistance360(Direction + SiPoint.DegreesToRadians(135), new SiPoint(35, 35));
                    _rightThrust.Direction.Degrees = Direction.Degrees;
                    _rightThrust.Location += pointRight;
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
        private SiVector fallToAngleRadians;
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
                        Direction += 1;
                    }
                    else if (deltaAngle < 0)
                    {
                        Direction -= 1;
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
                    fallToAngleRadians = Direction + new SiVector(180.0f + SiRandom.Between(0, 10)).Radians;
                    fallbackDistance = baseFallbackDistance * (SiRandom.NextFloat() + 1);
                }
            }

            if (mode == AIMode.MovingToFallback)
            {
                var deltaAngle = Direction - fallToAngleRadians;

                if (deltaAngle.Degrees > 10)
                {
                    if (deltaAngle.Degrees >= 180.0) //We might as well turn around clock-wise
                    {
                        Direction += 1;
                    }
                    else if (deltaAngle.Degrees < 180.0) //We might as well turn around counter clock-wise
                    {
                        Direction -= 1;
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
                        Direction += 1;
                    }
                    else if (deltaAngle < 0)
                    {
                        Direction -= 1;
                    }
                }
                else
                {
                    mode = AIMode.Approaching;
                    distanceToKeep = baseDistanceToKeep * (SiRandom.NextFloat() + 1);
                }
            }

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

        #endregion
    }
    */
}
