using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceShoot
{
    class PlayerShip : Entity
    {
        private bool Active { get; set; }
        AnimatedSprite ShipAnimation;
        public bool atEdge { get; set; } // Kind of a hack :P
        public Vector2 WindowSize { get; set; }
        public Vector2 MaxSpeed { get; set; }
        public Vector2 CurSpeed { get; set; }
        public int Health { get; set; }
        public float Attraction { get; set; }
        public float GrabRadius { get; set; }
        public enum bank { none, left, right}
        public bank Bank;
        private ParticleEngine EngineAnimation;
        public BoundingSphere Bounds { get; set; }

        public PlayerShip()
        {
            Bounds = new BoundingSphere(new Vector3(Position, 0), 10);
            Bank = bank.none;
            atEdge = false;
        }

        public PlayerShip(Vector2 pos, int Health, Vector2 MaxSpeed, float attraction, float grabRadius)
            : this()
        {
            Position = pos;
            this.Health = Health;
            this.MaxSpeed = new Vector2(MaxSpeed.X, -MaxSpeed.Y);
            CurSpeed = new Vector2(0, 0); 
            this.Attraction = attraction;
            this.GrabRadius = grabRadius;
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
            atEdge = false;
            if (Position.X == WindowSize.X || Position.X == 0) { atEdge = true; }
            Position = Position + CurSpeed;

            updateEngineAnimation();

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

        internal void ZeroThrust()
        {
            CurSpeed = Vector2.Zero;
        }

        internal void LeftThrust()
        {
            Bank = bank.left;
            CurSpeed = new Vector2(-MaxSpeed.X, CurSpeed.Y);
        }

        internal void RightThrust()
        {
            Bank = bank.right;
            CurSpeed = new Vector2(MaxSpeed.X, CurSpeed.Y);
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
