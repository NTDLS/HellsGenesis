using System;
using System.Windows;

namespace AI2D.Types
{
    public class AngleD
    {
        public static double DegreeOffset = 90;
        public static double RadianOffset = (Math.PI / 180) * DegreeOffset; //1.5707963267948966

        public AngleD()
        {
        }

        public AngleD(double angleDegree)
        {
            _degree = angleDegree;
        }

        public Vector Vector
        {
            get
            {
                return new Vector(X, Y);
            }
        }

        public double _degree = 0;
        public double Degree
        {
            get
            {
                return _degree;
            }
            set
            {
                _degree = (value % 360);
            }
        }

        public double Radian
        {
            get
            {
                //Rotate the angle counter-clockwise by 90 degrees. All of our graphics math should assume this.
                return ((Math.PI / 180) * _degree) - RadianOffset;
            }
        }

        public double RadianUnadjusted
        {
            get
            {
                return ((Math.PI / 180) * _degree);
            }
        }

        public double X
        {
            get
            {
                return Math.Cos(Radian);
            }
        }

        public double Y
        {
            get
            {
                return Math.Sin(Radian);
            }
        }
    }
}
