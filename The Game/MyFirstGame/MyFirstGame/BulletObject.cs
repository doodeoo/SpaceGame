using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpaceShoot
{
    class BulletObject : Entity
    {
        Vector2 velocity;
        int damage = 25; // temporary
        Texture2D texture;
        bool friendly;

        public BulletObject(Texture2D texture, Vector2 position, Vector2 velocity, bool friendly)
        {
            this.texture = texture;
            this.friendly = friendly;
            Position = position;
            this.velocity = velocity; 
        }

        public bool isFriendly()
        {
            return friendly;
        }

        // delta?
        new public void Update()
        {
            Position = Position + velocity;
        }

        new public void Draw(SpriteBatch batch)
        {
            batch.Draw(texture, Position, Color.White);
        }

        internal int GetDamage()
        {
            return damage;
        }
    }
}