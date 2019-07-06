using System;
using System.Windows;

namespace AI2D.Types
{
    public class AngleD
    {
        public AngleD()
        {
        }

        public Vector Vector
        {
            get
            {
                return new Vector(X, Y);
            }
        }

        public AngleD(double angleDegree)
        {
            _degree = angleDegree;
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
                _degree = value % 360;
            }
        }

        public double Radian
        {
            get
            {
                return (Math.PI / 180) * _degree;
            }
        }

        public double X
        {
            get
            {
                //Rotate the radian by 90 degrees.
                return Math.Cos(Radian - (90 * (Math.PI / 180)));
            }
        }

        public double Y
        {
            get
            {
                //Rotate the radian by 90 degrees.
                return Math.Sin(Radian - (90 * (Math.PI / 180)));
            }
        }
    }
}
