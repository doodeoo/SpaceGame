using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceShoot
{
    class EnemyShip : Ship
    {
        bool hit;
        int Health; 
        // Make a new class called FiringPattern to define the pattern of fire maybe?
        BoundingSphere hitSphere;
        private EnemyParticleEngine DestPartEngine;

        public EnemyShip(Vector2 pos, int health, List<WeaponObject> weapons, Vector2 speed)
        {
            this.weapons = weapons;
            hit = false;
            Position = pos;
            Health = health;
            this.CurSpeed = speed;
            Vector3 center = new Vector3(Position, 0);
            hitSphere = new BoundingSphere(center, 30);
        }

        internal Vector2 getPosition()
        {
            return Position;
        }

        internal void Initialize(EnemyParticleEngine destPartEngine)
        {
            DestPartEngine = destPartEngine;
        }

        internal void Update(GameTime gameTime)
        {
            double delta = gameTime.ElapsedGameTime.Milliseconds;
            foreach (WeaponObject curWep in weapons)
            {
                curWep.updateCooldown(delta);
            }
            hit = false;
            Position += CurSpeed;
            hitSphere.Center = new Vector3(Position, 0);

            DestPartEngine.EmitterLocation = new Vector2(Position.X + 20, Position.Y);
            DestPartEngine.Update();
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            DestPartEngine.Draw(spriteBatch);
            if (hit)
            {
                spriteBatch.Draw(texture, Position, Color.Yellow); // DO SOMETHING BETTER HERE
            }
            else
                spriteBatch.Draw(texture, Position, Color.White);
        }

        internal bool Intersects(BoundingSphere other)
        {
            return hitSphere.Intersects(other);
        }

        internal BoundingSphere getHitSphere()
        {
            return hitSphere;
        }

        internal void Damage(BulletObject curBullet)
        {
            hit = true;
            Health -= curBullet.GetDamage();
        }

        internal bool IsDead()
        {
            return Health <= 0;
        }

        public bool IsHit()
        {
            return hit;
        }
    }
}
