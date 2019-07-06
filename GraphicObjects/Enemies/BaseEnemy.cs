using AI2D.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI2D.GraphicObjects.Enemies
{
    public class BaseEnemy : BaseGraphicObject
    {
        public int CollisionDamage { get; set; } = 50;

        public BaseEnemy(Core core)
            : base(core)
        {
        }
    }
}