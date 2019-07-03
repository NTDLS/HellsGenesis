using AI2D.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AI2D
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());

            /*
            for (double inDegree = 0; inDegree < 359; inDegree++)
            {
                double inRadian = inDegree / (180.0 / Math.PI);

                double inX = Math.Cos(inRadian);
                double inY = Math.Sin(-inRadian);

                double outRadian = Math.Atan2(-inY, inX); //?
                //double outDegree = outRadian * (180.0 / Math.PI); //?
                //double outX = Math.Sin(outRadian);
                //double outY = -Math.Cos(outRadian);

                Console.WriteLine(

                      $"{ inDegree.ToString("0.0000")}, "
                    //+ $"x{ inX.ToString("0.0000")}, "
                    //+ $"y{ inY.ToString("0.0000")}, "
                    + $"r{inRadian.ToString("0.0000")}, "
                    //+ $"x{ outX.ToString("0.0000")}, "
                    //+ $"y{ outY.ToString("0.0000")}, "
                    + $"r{outRadian.ToString("0.0000")}"

                    );

            }
            */

            var points = Engine.Utility.AngleToXY(0);
            Console.WriteLine($"x{ points.X.ToString("0.0000")}, y{ points.Y.ToString("0.0000")}, ");

            points = Engine.Utility.AngleToXY(45);
            Console.WriteLine($"x{ points.X.ToString("0.0000")}, y{ points.Y.ToString("0.0000")}, ");

            points = Engine.Utility.AngleToXY(90);
            Console.WriteLine($"x{ points.X.ToString("0.0000")}, y{ points.Y.ToString("0.0000")}, ");

            points = Engine.Utility.AngleToXY(135);
            Console.WriteLine($"x{ points.X.ToString("0.0000")}, y{ points.Y.ToString("0.0000")}, ");

            points = Engine.Utility.AngleToXY(180);
            Console.WriteLine($"x{ points.X.ToString("0.0000")}, y{ points.Y.ToString("0.0000")}, ");

            points = Engine.Utility.AngleToXY(225);
            Console.WriteLine($"x{ points.X.ToString("0.0000")}, y{ points.Y.ToString("0.0000")}, ");

            points = Engine.Utility.AngleToXY(270);
            Console.WriteLine($"x{ points.X.ToString("0.0000")}, y{ points.Y.ToString("0.0000")}, ");

            points = Engine.Utility.AngleToXY(315);
            Console.WriteLine($"x{ points.X.ToString("0.0000")}, y{ points.Y.ToString("0.0000")}, ");


            Console.ReadKey();
        }
    }
}