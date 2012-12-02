using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SpaceShoot
{
    class Ship : Entity
    {
        internal int centerOffset;
        internal List<WeaponObject> weapons;
        internal bool friendly = false;
        internal Vector2 CurSpeed { get; set; }

        public void fire(Queue<BulletObject> Bullets, Texture2D[] BulletTextures, Random r)
        {
            foreach (WeaponObject curWep in weapons)
            {
                if (curWep.isReady())
                {
                    Vector2 WeaponPosition = new Vector2(Position.X + 10, Position.Y);
                    WeaponPosition = new Vector2(WeaponPosition.X+1, WeaponPosition.Y+30); // 10 is arb, based on texture
                    curWep.fire(Bullets, BulletTextures, WeaponPosition, CurSpeed, friendly, r);
                }
            }
        }
    }
}
