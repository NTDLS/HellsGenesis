using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI2D.Types
{
    public enum KeyPressState
    {
        Up,
        Down
    }
    public enum FiredFromType
    {
        Unspecified,
        Player,
        Enemy
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
