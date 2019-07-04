using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI2D.Types
{
    public class SizeD
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public SizeD()
        {

        }

        public SizeD(double width, double height)
        {
            Width = width;
            Height = height;
        }
    }
}
