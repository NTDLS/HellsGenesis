using AI2D.Engine;
using AI2D.Types;
using Algorithms;
using System;
using System.Drawing;
using System.Linq;

namespace AI2D.Actors.Enemies
{
    public class EnemyBase : ActorBase
    {
        public int CollisionDamage { get; set; } = 25;
        public int ScorePoints { get; private set; } = 25;
        public ActorRadarPositionIndicator RadarPositionIndicator { get; set; }
        public ActorRadarPositionTextBlock RadarPositionText { get; set; }
        public ActorAnimation ThrustAnimation { get; private set; }

        public EnemyBase(Core core, int hitPoints, int scoreMultiplier)
            : base(core)
        {
            Velocity.ThrottlePercentage = 1;
            Initialize();

            //Brain = BrainBase.GetBrain();

            this.RadarDotSize = new Point<int>(4, 4);
            this.RadarDotColor = Color.FromArgb(200, 100, 100);

            base.SetHitPoints(hitPoints);
            ScorePoints = HitPoints * scoreMultiplier;

            RadarPositionIndicator = _core.Actors.AddNewRadarPositionIndicator();
            RadarPositionIndicator.Visable = false;
            RadarPositionText = _core.Actors.AddNewRadarPositionTextBlock("Consolas", Brushes.Red, 8, new Point<double>());

            string _thrustAniPath = @"..\..\..\Assets\Graphics\Animation\AirThrust32x32.png";
            var playMode = new ActorAnimation.PlayMode()
            {
                Replay = ActorAnimation.ReplayMode.LoopedPlay,
                DeleteActorAfterPlay = false,
                ReplayDelay = new TimeSpan(0)
            };
            ThrustAnimation = new ActorAnimation(_core, _thrustAniPath, new Size(32, 32), 10, playMode);

            ThrustAnimation.Reset();
            _core.Actors.PlaceAnimationOnTopOf(ThrustAnimation, this);

            var pointRight = Utility.AngleFromPointAtDistance(base.Velocity.Angle + 180, new Point<double>(20, 20));
            ThrustAnimation.Velocity.Angle.Degrees = this.Velocity.Angle.Degrees - 180;
            ThrustAnimation.X = this.X + pointRight.X;
            ThrustAnimation.Y = this.Y + pointRight.Y;
        }

        public virtual void BeforeCreate()
        {
        }
        public virtual void AfterCreate()
        {
        }

        public override void RotationChanged()
        {
            PositionChanged();
        }

        public override void PositionChanged()
        {
            if (ThrustAnimation != null && ThrustAnimation.Visable)
            {
                var pointRight = Utility.AngleFromPointAtDistance(base.Velocity.Angle + 180, new Point<double>(20, 20));
                ThrustAnimation.Velocity.Angle.Degrees = this.Velocity.Angle.Degrees - 180;
                ThrustAnimation.X = this.X + pointRight.X;
                ThrustAnimation.Y = this.Y + pointRight.Y;
            }
        }

        public static int GetGenericHP()
        {
            return Utility.Random.Next(Constants.Limits.MinEnemyHealth, Constants.Limits.MaxEnemyHealth);
        }

        public virtual void ApplyMotion(Point<double> frameAppliedOffset)
        {
            if (X < -Constants.Limits.EnemySceneDistanceLimit
                || X >= _core.Display.VisibleSize.Width + Constants.Limits.EnemySceneDistanceLimit
                || Y < -Constants.Limits.EnemySceneDistanceLimit
                || Y >= _core.Display.VisibleSize.Height + Constants.Limits.EnemySceneDistanceLimit)
            {
                QueueForDelete();;
                return;
            }

            X += (Velocity.Angle.X * (Velocity.MaxSpeed * Velocity.ThrottlePercentage)) - frameAppliedOffset.X;
            Y += (Velocity.Angle.Y * (Velocity.MaxSpeed * Velocity.ThrottlePercentage)) - frameAppliedOffset.Y;

            if (RadarPositionIndicator != null)
            {
                if (X < 0 || X >= _core.Display.VisibleSize.Width || Y < 0 || Y >= _core.Display.VisibleSize.Height)
                {
                    RadarPositionText.DistanceValue = Math.Abs(DistanceTo(_core.Actors.Player));

                    RadarPositionText.Visable = true;
                    RadarPositionIndicator.Visable = true;

                    double requiredAngle = _core.Actors.Player.AngleTo(this);

                    var offset = Utility.AngleFromPointAtDistance(new Angle<double>(requiredAngle), new Point<double>(200, 200));

                    RadarPositionText.Location = _core.Actors.Player.Location + offset + new Point<double>(25, 25);
                    RadarPositionIndicator.Velocity.Angle.Degrees = requiredAngle;

                    RadarPositionIndicator.Location = _core.Actors.Player.Location + offset;
                    RadarPositionIndicator.Velocity.Angle.Degrees = requiredAngle;
                }
                else
                {
                    RadarPositionText.Visable = false;
                    RadarPositionIndicator.Visable = false;
                }
            }
        }

        public virtual void ApplyIntelligence(Point<double> frameAppliedOffset)
        {
            if (SelectedSecondaryWeapon != null && _core.Actors.Player != null)
            {
                SelectedSecondaryWeapon.ApplyIntelligence(frameAppliedOffset, _core.Actors.Player); //Enemy lock-on to Player. :O
            }
        }

        public override void Cleanup()
        {
            if (RadarPositionIndicator != null)
            {
                RadarPositionIndicator.QueueForDelete();;
                RadarPositionText.QueueForDelete();;
            }
            if (ThrustAnimation != null)
            {
                ThrustAnimation.QueueForDelete();;
            }
            base.Cleanup();
        }

        #region AI

        private DateTime? _lastDecisionTime = null;
        public NeuralNetwork Brain { get; private set; }
        public double MinimumTravelDistanceBeforeDamage { get; set; } = 20;
        public double MaxObserveDistance { get; set; } = 100;
        public double VisionToleranceDegrees { get; set; } = 25;
        public int MillisecondsBetweenDecisions { get; set; } = 50;
        public float DecisionSensitivity { get; set; } = (float)Utility.RandomNumber(0.25, 0.55);

        /// <summary>
        /// This is not currently used. Can be applied to perform AI logistics.
        /// </summary>
        public void ApplyAIIntelligence()
        {
            DateTime now = DateTime.UtcNow;

            if (_lastDecisionTime == null || (now - (DateTime)_lastDecisionTime).TotalMilliseconds >= MillisecondsBetweenDecisions)
            {
                var decidingFactors = GetVisionInputs();

                var decisions = this.Brain.FeedForward(decidingFactors);

                if (decisions[AI.BrainBase.Outputs.ShouldRotate] >= DecisionSensitivity)
                {
                    var rotateAmount = decisions[AI.BrainBase.Outputs.RotateLeftOrRightAmount];

                    if (decisions[AI.BrainBase.Outputs.RotateLeftOrRight] >= DecisionSensitivity)
                    {
                        this.Rotate(45 * rotateAmount);
                    }
                    else
                    {
                        this.Rotate(-45 * rotateAmount);
                    }
                }

                if (decisions[AI.BrainBase.Outputs.ShouldSpeedUpOrDown] >= DecisionSensitivity)
                {
                    double speedFactor = decisions[AI.BrainBase.Outputs.SpeedUpOrDownAmount];
                    this.Velocity.ThrottlePercentage += (speedFactor / 5.0);
                }
                else
                {
                    double speedFactor = decisions[AI.BrainBase.Outputs.SpeedUpOrDownAmount];
                    this.Velocity.ThrottlePercentage += -(speedFactor / 5.0);
                }

                if (this.Velocity.ThrottlePercentage < 0)
                {
                    this.Velocity.ThrottlePercentage = 0;
                }
                if (this.Velocity.ThrottlePercentage == 0)
                {
                    this.Velocity.ThrottlePercentage = 0.10;
                }

                _lastDecisionTime = now;
            }

            if (this.IsOnScreen == false)
            {
                //Kill this bug:
                this.Visable = false;
            }

            var intersections = Intersections();

            if (intersections.Any())
            {
                this.QueueForDelete();
            }
        }


        /// <summary>
        /// Looks around and gets neuralnetwork inputs for visible proximity objects.
        /// </summary>
        /// <returns></returns>
        private float[] GetVisionInputs()
        {
            var scenerio = new float[5];

            //The closeness is expressed as a percentage of how close to the other object they are. 100% being touching 0% being 1 pixel from out of range.
            foreach (var other in _core.Actors.Collection.Where(o=> o is EnemyBase))
            {
                if (other == this)
                {
                    continue;
                }

                double distance = this.DistanceTo(other);
                double percentageOfCloseness = 1 - (distance / MaxObserveDistance);

                if (this.IsPointingAt(other, VisionToleranceDegrees, MaxObserveDistance, -90))
                {
                    scenerio[AI.BrainBase.Inputs.ObjTo90Left] = (float)
                        (percentageOfCloseness > scenerio[AI.BrainBase.Inputs.ObjTo90Left] ? percentageOfCloseness : scenerio[AI.BrainBase.Inputs.ObjTo90Left]);
                }

                if (this.IsPointingAt(other, VisionToleranceDegrees, MaxObserveDistance, -45))
                {
                    scenerio[AI.BrainBase.Inputs.ObjTo45Left] = (float)
                        (percentageOfCloseness > scenerio[AI.BrainBase.Inputs.ObjTo45Left] ? percentageOfCloseness : scenerio[AI.BrainBase.Inputs.ObjTo45Left]);
                }

                if (this.IsPointingAt(other, VisionToleranceDegrees, MaxObserveDistance, 0))
                {
                    scenerio[AI.BrainBase.Inputs.ObjAhead] = (float)
                        (percentageOfCloseness > scenerio[AI.BrainBase.Inputs.ObjAhead] ? percentageOfCloseness : scenerio[AI.BrainBase.Inputs.ObjAhead]);
                }

                if (this.IsPointingAt(other, VisionToleranceDegrees, MaxObserveDistance, +45))
                {
                    scenerio[AI.BrainBase.Inputs.ObjTo45Right] = (float)
                        (percentageOfCloseness > scenerio[AI.BrainBase.Inputs.ObjTo45Right] ? percentageOfCloseness : scenerio[AI.BrainBase.Inputs.ObjTo45Right]);
                }

                if (this.IsPointingAt(other, VisionToleranceDegrees, MaxObserveDistance, +90))
                {
                    scenerio[AI.BrainBase.Inputs.ObjTo90Right] = (float)
                        (percentageOfCloseness > scenerio[AI.BrainBase.Inputs.ObjTo90Right] ? percentageOfCloseness : scenerio[AI.BrainBase.Inputs.ObjTo90Right]);
                }
            }

            return scenerio;
        }

        #endregion

    }
}