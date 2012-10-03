using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceShoot
{
    class DestructibleObject : Entity
    {
        bool hit;
        int cooldown;
        int Health, FireDamage, FireSpeed; // Make a new class called FiringPattern to define the pattern of fire maybe?
        Vector2 MoveSpeed;
        BoundingSphere hitSphere;
        private EnemyParticleEngine DestPartEngine;

        public DestructibleObject(Vector2 pos, int health, int fireDamage, int fireSpeed, Vector2 moveSpeed)
        {
            cooldown = fireSpeed;
            hit = false;
            Position = pos;
            Health = health;
            FireDamage = fireDamage;
            FireSpeed = fireSpeed;
            MoveSpeed = moveSpeed;
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

        internal void Update(float gameSpeed)
        {
            if (cooldown > 0)
                cooldown--;
            hit = false;
            Position += MoveSpeed * gameSpeed;
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

        public bool canFire()
        {
            return (cooldown == 0);
        }

        public void Fire()
        {
            cooldown = FireSpeed;
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

/*

 * Sort the different types of enemies

*/