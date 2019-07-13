using AI2D.Engine;
using AI2D.Types;
using System;
using System.Drawing;

namespace AI2D.GraphicObjects.Enemies
{
    public class BaseEnemy : BaseGraphicObject
    {
        public int CollisionDamage { get; set; } = 25;
        public int ScorePoints { get; private set; } = 25;
        public ObjRadarPositionIndicator RadarPositionIndicator { get; set; }
        public ObjRadarPositionTextBlock RadarPositionText { get; set; }

        public BaseEnemy(Core core, int hitPoints, int scoreMultiplier)
            : base(core)
        {
            Velocity.ThrottlePercentage = 1;
            Initialize();

            HitPoints = hitPoints;
            ScorePoints = HitPoints * scoreMultiplier;

            RadarPositionIndicator = _core.Actors.AddNewRadarPositionIndicator();
            RadarPositionIndicator.Visable = false;
            RadarPositionText = _core.Actors.AddNewRadarPositionTextBlock("Consolas", Brushes.Red, 8, 0, 0);
        }

        public static int GetGenericHP()
        {
            return Utility.Random.Next(Constants.Limits.MinEnemyHealth, Constants.Limits.MaxEnemyHealth);
        }

        public virtual void ApplyMotion(PointD frameAppliedOffset)
        {
            if (X < -Constants.Limits.EnemySceneDistanceLimit
                || X >= _core.Display.VisibleSize.Width + Constants.Limits.EnemySceneDistanceLimit
                || Y < -Constants.Limits.EnemySceneDistanceLimit
                || Y >= _core.Display.VisibleSize.Height + Constants.Limits.EnemySceneDistanceLimit)
            {
                ReadyForDeletion = true;
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

                    var offset = Utility.AngleFromPointAtDistance(new AngleD(requiredAngle), new PointD(200, 200));

                    RadarPositionText.Location = _core.Actors.Player.Location + offset + new PointD(25, 25);
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

        public virtual void ApplyIntelligence(PointD frameAppliedOffset)
        {
            if (CurrentWeapon != null && _core.Actors.Player != null)
            {
                CurrentWeapon.ApplyIntelligence(frameAppliedOffset, _core.Actors.Player); //Enemy lock-on to Player. :O
            }
        }

        public override void Cleanup()
        {
            if (RadarPositionIndicator != null)
            {
                RadarPositionIndicator.ReadyForDeletion = true;
                RadarPositionText.ReadyForDeletion = true;
            }
            base.Cleanup();
        }
    }
}