﻿using Si.Engine.Sprite._Superclass;
using Si.Library;
using Si.Library.Mathematics.Geometry;
using System;
using System.Drawing;

namespace Si.GameEngine.Sprite.SupportingClasses
{
    /// <summary>
    /// Contains the sprite and the bounds that it is predicted to occupy after ApplyMotion().
    /// Keep in mind that this is rudimentary in the way that it predicts the next location but decisively so.
    /// </summary>
    public class PredictedSpriteRegion
    {
        /// <summary>
        /// Reference to the sprite.
        /// </summary>
        public SpriteInteractiveBase Sprite { get; set; }

        /// <summary>
        /// Size of the referenced sprite.
        /// </summary>
        public Size Size => Sprite.Size;

        /// <summary>
        /// Predicted location after next call to ApplyMotion().
        /// </summary>
        public SiPoint Location { get; set; }

        /// <summary>
        /// Predicted bounds after next call to ApplyMotion().
        /// </summary>
        public RectangleF Bounds => new(
                Location.X - Size.Width / 2.0f,
                Location.Y - Size.Height / 2.0f,
                Size.Width, Size.Height);

        public PredictedSpriteRegion(SpriteInteractiveBase sprite, float epoch)
        {
            Sprite = sprite;
            Location = sprite.Location + sprite.Velocity.MovementVector * epoch;
        }

        /// <summary>
        /// Determines if two axis-aligned bounding boxes (AABB) intersect.
        /// </summary>
        /// <param name="otherObject"></param>
        /// <returns></returns>
        public bool IntersectsAABB(PredictedSpriteRegion otherObject) =>
            Bounds.IntersectsWith(otherObject.Bounds);

        /// <summary>
        /// Determines if two (non-axix-aligned) rectangles interset using Separating Axis Theorem (SAT).
        /// </summary>
        /// <param name="otherObject"></param>
        /// <returns></returns>
        public bool IntersectsSAT(PredictedSpriteRegion otherObject)
            => SiCollisionDetection.IntersectsRotated(Bounds, Sprite.Velocity.ForwardAngle.Radians,
                otherObject.Bounds, otherObject.Sprite.Velocity.ForwardAngle.Radians);
    }
}
