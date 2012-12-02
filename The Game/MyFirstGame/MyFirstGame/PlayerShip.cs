using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace SpaceShoot
{
    class PlayerShip : Ship
    {
        private bool Active { get; set; }
        AnimatedSprite ShipAnimation;
        public bool atEdge { get; set; } // Kind of a hack :P
        public Vector2 WindowSize { get; set; }
        public Vector2 MaxSpeed { get; set; }
        public int Health { get; set; }
        public float Attraction { get; set; }
        public float GrabRadius { get; set; }
        public enum bank { none, left, right}
        public bank Bank;
        private ParticleEngine EngineAnimation;
        public bool firing { get; set; }
        public BoundingSphere Bounds { get; set; }

        public PlayerShip(Vector2 pos, int Health, Vector2 MaxSpeed, List<WeaponObject> weapons, float attraction, float grabRadius)
        {
            centerOffset = 5;
            friendly = true;
            Bounds = new BoundingSphere(new Vector3(Position, 0), 10);
            Bank = bank.none;
            atEdge = false;
            this.weapons = weapons;
            Position = pos;
            this.Health = Health;
            this.MaxSpeed = new Vector2(MaxSpeed.X, -MaxSpeed.Y);
            CurSpeed = new Vector2(0, 0); 
            this.Attraction = attraction;
            this.GrabRadius = grabRadius;
            firing = false;
        }

        public void Initialize(Vector2 windowSize, AnimatedSprite shipAnimation, ParticleEngine engineAnimation)
        {
            WindowSize = windowSize;
            ShipAnimation = shipAnimation;
            EngineAnimation = engineAnimation;
            Active = true;
        }

        public void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            Position = Position + CurSpeed;

            double delta = gameTime.ElapsedGameTime.Milliseconds;
            foreach (WeaponObject curWep in weapons)
            {
                curWep.updateCooldown(delta);
            }

            updateEngineAnimation();

            // making sure ship isn't out of window
            if (Position.X > WindowSize.X - 30) { Position = new Vector2(WindowSize.X - 30, Position.Y); }
            else if (Position.X < 0) { Position = new Vector2(0, Position.Y); }
            if (Position.Y > WindowSize.Y - 20) { Position = new Vector2(Position.X, WindowSize.Y - 20); }
            else if (Position.Y < 0) { Position = new Vector2(Position.X, 0); }
            
            Bounds = new BoundingSphere(new Vector3(Position, 0), Bounds.Radius);
            ZeroThrust();
        }

        private void updateEngineAnimation()
        {
            EngineAnimation.EmitterLocation = new Vector2(Position.X + 15, Position.Y + 5);
            EngineAnimation.Update();
        }

        new public void Draw(SpriteBatch spriteBatch)
        {
            ShipAnimation.DrawFrame(spriteBatch, Position, (int)Bank);
            EngineAnimation.Draw(spriteBatch);
            resetBank();
        }

        new public void fire(Queue<BulletObject> Bullets, Texture2D[] BulletTextures, Random r)
        {
            foreach (WeaponObject curWep in weapons)
            {
                if (curWep.isReady())
                {
                    Vector2 WeaponPosition = new Vector2(Position.X + 10, Position.Y);
                    WeaponPosition = new Vector2(WeaponPosition.X, WeaponPosition.Y - 10); // 10 is arb, based on texture
                    curWep.fire(Bullets, BulletTextures, WeaponPosition, CurSpeed, friendly, r);
                }
            }
            
            firing = false;
        }

        internal void ZeroThrust()
        {
            CurSpeed = Vector2.Zero;
        }

        internal void LeftThrust()
        {
            if (Position.X > 0)
            {
                Bank = bank.left; // for texture animation
                CurSpeed = new Vector2(-MaxSpeed.X, CurSpeed.Y); 
            }
            
        }

        internal void RightThrust()
        {
            if (Position.X < WindowSize.X-30)
            {
                Bank = bank.right;
                CurSpeed = new Vector2(MaxSpeed.X, CurSpeed.Y);
            }
        }

        internal void ForwardThrust()
        {
            CurSpeed = new Vector2(CurSpeed.X, MaxSpeed.Y);
        }

        internal void RearThrust()
        {
            CurSpeed = new Vector2(CurSpeed.X, -MaxSpeed.Y);
        }

        public bank getBank()
        {
            return Bank;
        }

        public void resetBank()
        {
            Bank = bank.none;
        }
    }
}
