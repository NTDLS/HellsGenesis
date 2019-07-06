using AI2D.GraphicObjects;
using AI2D.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI2D.Weapons
{
    public interface IWeapon
    {
        string Name { get; }
        int RoundQuantity { get; set; }
        bool CanFire { get; }

        bool Fire();

        void SetOwner(BaseGraphicObject owner);
    }
}

