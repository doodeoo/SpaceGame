using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SpaceShoot
{
    class BoonObject : Entity
    {
        Vector2 Speed;
        enum boonTypes { money, material, bonus, none }
        public Enum type { get; set; }
        public BoonObject(Vector2 pos, bool bonusPossible, Random r)  
        {
            int R = r.Next(100);

            #region Loot Dropping Algorithm
            type = boonTypes.none;
            if (R < 5 && bonusPossible) // 5% chance that loot will be bonus, but only if bonus is possible
                type = boonTypes.bonus;
            else if (R < 20)
                type = boonTypes.material; // 25% chance loot is material (20 if bonus round)
            else if (R < 80)
                type = boonTypes.money; // (75-25) 50% chance loot is money (45 if bonus round)
            #endregion

            float x = pos.X - (r.Next(20) - 10);
            float y = pos.Y - (r.Next(20) - 10);
            Position = new Vector2(x, y);
            Speed = new Vector2(0f, 1.25f); 
        }

        public void Update(float gameSpeed, Vector2 shipPos, float attraction)
        {
            
            float dist = Vector2.Distance(shipPos, Position);
            if (dist < attraction) {
                Position += Speed / 2 * gameSpeed;
                Position += (shipPos - Position) * 5 / dist * gameSpeed;
            } else
                Position += Speed * gameSpeed;
        }

        public void Draw(SpriteBatch batch, Texture2D[] textures)
        {
            if (((boonTypes)type).Equals(boonTypes.none))
                throw new MissingFieldException("The Boon has no type!!");

            batch.Draw(textures[(int)(boonTypes)type], Position, Color.White);
        }

        internal bool isCollected(BoundingSphere boundingSphere, float radius)
        {
            Vector3 thisIn3D = new Vector3(Position, 0);
            return (Vector3.Distance(boundingSphere.Center, thisIn3D) < radius);
        }
    }
}
