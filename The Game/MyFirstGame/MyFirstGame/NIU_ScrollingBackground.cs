using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceShoot
{
    class NIU_ScrollingBackground
    {
        private Vector2 screenpos, origin, texturesize;
        private Texture2D mytexture;
        private int screenheight;

        public void Load(GraphicsDevice device, Texture2D backgroundTexture)
        {
            mytexture = backgroundTexture;
            screenheight = device.Viewport.Height;
            int screenwidth = device.Viewport.Width;
            //Set the origin so that we're drawing from the center of the top edge
            origin = new Vector2(mytexture.Width / 2, 0);
            //Set the screen position to the center of the screen.
            screenpos = new Vector2(screenwidth / 2, screenheight / 2);
            // offset to draw the texture, when necessary.
            texturesize = new Vector2(0, mytexture.Height);
        }

        public void Update(float deltaY)
        {
            screenpos.Y += deltaY;
            screenpos.Y = screenpos.Y % mytexture.Height;
        }

        public void Draw(SpriteBatch batch)
        {
            //draw the texture, if it is still onscreen.
            if (screenpos.Y < screenheight)
            {
                batch.Draw( mytexture, screenpos, null, 
                    Color.White, 0, origin, 1, SpriteEffects.None, 0f);
            }
            //draw the texture a second time, behind the first,
            //to create the scrolling illusion
            batch.Draw(mytexture, screenpos - texturesize, null,
                Color.White, 0, origin, 1, SpriteEffects.None, 0f);
        }
    }
}
