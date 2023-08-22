using AI2D.Actors;
using AI2D.Types;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace AI2D.Engine
{
    internal class Utility
    {
        #region Graphics.

        public static dynamic DynamicCast(dynamic source, Type dest) => Convert.ChangeType(source, dest);

        public static Bitmap RotateImageWithClipping(Bitmap bmp, double angle, Color backgroundColor)
        {
            Bitmap rotatedImage = new Bitmap(bmp.Width, bmp.Height, backgroundColor == Color.Transparent ?
                                             PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb);

            rotatedImage.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                // Fill in the specified background color if necessary
                if (backgroundColor != Color.Transparent)
                {
                    g.Clear(backgroundColor);
                }

                // Set the rotation point to the center in the matrix
                g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                // Rotate
                g.RotateTransform((float)angle);
                // Restore rotation point in the matrix
                g.TranslateTransform(-bmp.Width / 2, -bmp.Height / 2);
                // Draw the image on the bitmap
                g.DrawImage(bmp, new Point(0, 0));
            }

            return rotatedImage;
        }

        public static Bitmap RotateImageWithUpsize(Image inputImage, double angleDegrees, Color backgroundColor)
        {
            // Test for zero rotation and return a clone of the input image
            if (angleDegrees == 0f)
                return (Bitmap)inputImage.Clone();

            // Set up old and new image dimensions, assuming upsizing not wanted and clipping OK
            int oldWidth = inputImage.Width;
            int oldHeight = inputImage.Height;

            double angleRadians = angleDegrees * Math.PI / 180d;
            double cos = Math.Abs(Math.Cos(angleRadians));
            double sin = Math.Abs(Math.Sin(angleRadians));
            int newWidth = (int)Math.Round(oldWidth * cos + oldHeight * sin);
            int newHeight = (int)Math.Round(oldWidth * sin + oldHeight * cos);

            // Create the new bitmap object. If background color is transparent it must be 32-bit, 
            //  otherwise 24-bit is good enough.
            Bitmap newBitmap = new Bitmap(newWidth, newHeight, backgroundColor == Color.Transparent ?
                                             PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb);
            newBitmap.SetResolution(inputImage.HorizontalResolution, inputImage.VerticalResolution);

            // Create the Graphics object that does the work
            using (Graphics graphicsObject = Graphics.FromImage(newBitmap))
            {
                graphicsObject.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsObject.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphicsObject.SmoothingMode = SmoothingMode.HighQuality;

                // Fill in the specified background color if necessary
                if (backgroundColor != Color.Transparent)
                    graphicsObject.Clear(backgroundColor);

                // Set up the built-in transformation matrix to do the rotation and maybe scaling
                graphicsObject.TranslateTransform(newWidth / 2f, newHeight / 2f);

                graphicsObject.RotateTransform((float)angleDegrees);
                graphicsObject.TranslateTransform(-oldWidth / 2f, -oldHeight / 2f);

                // Draw the result 
                graphicsObject.DrawImage(inputImage, 0, 0);
            }

            return newBitmap;
        }

        public static Image ResizeImage(Image image, int new_width, int new_height)
        {
            Bitmap new_image = new Bitmap(new_width, new_height);
            Graphics g = Graphics.FromImage((Image)new_image);
            g.InterpolationMode = InterpolationMode.High;
            g.DrawImage(image, 0, 0, new_width, new_height);
            return new_image;
        }

        #endregion

        #region Math.

        /// <summary>
        /// Calculates a point at a given angle and a given distance.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public static Point<double> AngleFromPointAtDistance(Angle<double> angle, Point<double> distance)
        {
            return new Point<double>(
                (Math.Cos(angle.Radians) * distance.X),
                (Math.Sin(angle.Radians) * distance.Y));
        }

        /// <summary>
        /// Calculates the angle of one objects location to another location.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static double AngleTo(ActorBase from, ActorBase to)
        {
            return Point<double>.AngleTo(from.Location, to.Location);
        }

        public static double AngleTo(Point<double> from, ActorBase to)
        {
            return Point<double>.AngleTo(from, to.Location);
        }

        public static double AngleTo(ActorBase from, Point<double> to)
        {
            return Point<double>.AngleTo(from.Location, to);
        }

        public static bool IsPointingAway(ActorBase fromObj, ActorBase atObj, double toleranceDegrees)
        {
            var deltaAngle = Math.Abs(DeltaAngle(fromObj, atObj));
            return deltaAngle < 180 + toleranceDegrees && deltaAngle > 180 - toleranceDegrees;
        }

        public static bool IsPointingAway(ActorBase fromObj, ActorBase atObj, double toleranceDegrees, double maxDistance)
        {
            return IsPointingAway(fromObj, atObj, toleranceDegrees) && DistanceTo(fromObj, atObj) <= maxDistance;
        }

        public static bool IsPointingAt(ActorBase fromObj, ActorBase atObj, double toleranceDegrees)
        {
            var deltaAngle = Math.Abs(DeltaAngle(fromObj, atObj));
            return deltaAngle < toleranceDegrees || deltaAngle > (360 - toleranceDegrees);
        }

        public static bool IsPointingAt(ActorBase fromObj, ActorBase atObj, double toleranceDegrees, double maxDistance, double offsetAngle = 0)
        {
            var deltaAngle = Math.Abs(DeltaAngle(fromObj, atObj, offsetAngle));
            if (deltaAngle < toleranceDegrees || deltaAngle > (360 - toleranceDegrees))
            {
                return DistanceTo(fromObj, atObj) <= maxDistance;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromObj"></param>
        /// <param name="atObj"></param>
        /// <param name="offsetAngle">-90 degrees would be looking off te left-hand side of the object</param>
        /// <returns></returns>
        public static double DeltaAngle(ActorBase fromObj, ActorBase atObj, double offsetAngle = 0)
        {
            double fromAngle = fromObj.Velocity.Angle.Degrees + offsetAngle;

            double angleTo = AngleTo(fromObj, atObj);

            if (fromAngle < 0) fromAngle = (0 - fromAngle);
            if (angleTo < 0)
            {
                angleTo = (0 - angleTo);
            }

            angleTo = fromAngle - angleTo;

            if (angleTo < 0)
            {
                angleTo = 360.0 - (Math.Abs(angleTo) % 360.0);
            }

            return angleTo;
        }

        public static double DistanceTo(ActorBase from, ActorBase to)
        {
            return Point<double>.DistanceTo(from.Location, to.Location);
        }

        #endregion

        #region Random.


        public static bool ChanceIn(int n)
        {
            return (Random.Next(0, n * 10) % n) == n / 2;
        }


        public static Random Random = new Random();
        public static bool FlipCoin()
        {
            return Random.Next(0, 1000) >= 500;
        }

        public static Double RandomNumber(double min, double max)
        {
            return Random.NextDouble() * (max - min) + min;
        }

        public static int RandomNumber(int min, int max)
        {
            return Random.Next(0, 1000) % max;
        }

        /// <summary>
        /// This POS is just awful. It doesnt even accept negative input. Or respect it
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int RandomNumberNegative(int min, int max)
        {
            if (FlipCoin())
            {
                return -(Random.Next(0, 1000) % max);
            }
            return Random.Next(0, 1000) % max;
        }

        #endregion
    }
}
