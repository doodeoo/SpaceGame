using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SpaceShoot
{
    
    class WeaponObject
    {
        bool friendly;
        int bulletType;
        int bulletDamage;
        float fireVelocity;
        public double spread { get; set; }
        double bulletAcceleration;
        double maxcooldown;
        double curcooldown;

        public WeaponObject()
        {
            friendly = false; // default option is for enemy ships
            bulletType = 0; // default bullet type is 1
            bulletDamage = 25; // default
            fireVelocity = -10; // will later be multiplied by game speed, negative for enemies
            spread = 10; // 10 is an experimental value for the default
            bulletAcceleration = 0; // for self-propelled bullets, otherwise default to 0
            maxcooldown = 0; // default as fast as possible
            curcooldown = maxcooldown; // default to max
        }

        public WeaponObject(bool friendly)
            : this()
        {
            friendly = true;
        }

        public WeaponObject(bool friendly,
                            int bulletType,
                            int bulletDamage,
                            float fireVelocity,
                            double spread,
                            double bulletAcceleration,
                            double maxcooldown)
        {
            this.friendly = friendly;
            this.bulletType = bulletType;
            this.bulletDamage = bulletDamage;
            this.fireVelocity = fireVelocity;
            this.spread = spread;
            this.bulletAcceleration = bulletAcceleration;
            this.maxcooldown = maxcooldown;
            curcooldown = maxcooldown;
        }

        // delta? 5 * 1 / gameSpeed ???
        internal void updateCooldown(double delta)
        {
            if (curcooldown > 0 && !(curcooldown < 0)) curcooldown -= delta;
        }

        internal void fire(Queue<BulletObject> Bullets, Texture2D[] BulletTextures, Vector2 WeaponPosition, Vector2 ShipSpeed, bool friendly, Random r)
        {
            curcooldown = maxcooldown;
            Vector2 bulletVelocity = new Vector2(ShipSpeed.X/2, ShipSpeed.Y/2); // not strictly physical but looks better
            bulletVelocity.X += (float)(spread * r.NextDouble() - spread/2);
            bulletVelocity.Y -= fireVelocity;
            Bullets.Enqueue(new BulletObject(BulletTextures[bulletType], WeaponPosition, bulletVelocity, friendly));
        }

        public bool isReady()
        {
            return curcooldown <= 0;
        }
    }
}
