using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceShoot
{
    class Ship : Entity
    {
        internal int centerOffset;
        internal List<WeaponObject> weapons;
        internal bool friendly = false;
        
    }
}
