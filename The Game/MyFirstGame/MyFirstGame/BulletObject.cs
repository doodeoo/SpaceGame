using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpaceShoot
{
    class BulletObject : Entity
    {
        Vector2 bulletSpeed;
        int damage = 25; // temporary
        Texture2D Texture;
        static Boolean alternate = true;

        public BulletObject(Texture2D texture1, Texture2D texture2, Vector2 spritePosition, Vector2 shipSpeed, float velocity, Random r)
        {
            if (alternate)
                Texture = texture1;
            else
                Texture = texture2;
            alternate = !alternate;

            Position = spritePosition;
            Position = new Vector2(Position.X + r.Next(4,12), Position.Y - 10);
            bulletSpeed = new Vector2(shipSpeed.X / 2 + r.Next(1), -velocity); // The /2 on the ship speed doesn't actually make sense in terms of physics, but it looks good!
        }

        new public void Update()
        {
            Position = Position + bulletSpeed;
        }

        new public void Draw(SpriteBatch batch)
        {
            batch.Draw(Texture, Position, Color.White);
        }

        internal int GetDamage()
        {
            return damage;
        }
    }
}