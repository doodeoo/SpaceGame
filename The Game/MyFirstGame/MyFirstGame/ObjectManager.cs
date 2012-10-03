using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace SpaceShoot
{
    class ObjectManager
    {
        Queue<Entity> Objects;

        public ObjectManager()
        {
            Objects = new Queue<Entity>();
        }

        public void DrawAll(SpriteBatch spriteBatch)
        {
            int length = Objects.Count;
            Debug.WriteLine("--------------");
            for (int i = 0; i < length; i++)
            {
                Debug.WriteLine(i);
                Entity cur = Objects.Dequeue();
                if (true) //cur.ProcessUpdate())
                {
                    cur.Draw(spriteBatch);
                    Objects.Enqueue(cur);
                }
            }
        }

        internal void Add(Entity enemy)
        {
            Objects.Enqueue(enemy);
        }

        internal void ClearAll()
        {
            Objects = new Queue<Entity>();
        }
    }
}
