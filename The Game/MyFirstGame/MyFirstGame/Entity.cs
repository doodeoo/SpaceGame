using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics; // For debugging

namespace SpaceShoot
{
    class Entity
    {
        public Vector2 Position { get; set; }

        public Entity()
        {

        }

        public Entity(Vector2 spritePosition)
        {
            Position = spritePosition;
        }

        public void Initialize()
        {

        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
        
        }

        internal bool ProcessUpdate(float gameSpeed)
        {
            return true; // Do nothing, leave this to the more specific class
        }
    }
}
