using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI2D.Types
{
    public enum KeyPressState
    {
        Up = 0,
        Down = 1
    }

    public enum PlayerKey
    {
        Forward,
        Reverse,
        Fire,
        RotateClockwise,
        RotateCounterClockwise,
        Escape
    }
}
