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
                _degree = (value % 360);
            }
        }

        public double Radian
        {
            get
            {
                //Rotate the radian counter-clockwise by 90 degrees. All of our graphics math should assume this.
                return (Math.PI / 180) * (_degree - 90);
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
