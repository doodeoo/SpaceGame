using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace SpaceShoot
{
    public class AnimatedSprite
    {
        public Texture2D Texture { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int currentFrame { get;  set; }
        public int totalFrames;

        public AnimatedSprite(Texture2D texture, int rows, int columns)
        {
            Texture = texture;
            Columns = columns;
            Rows = rows;
            currentFrame = 0;
            totalFrames = Rows * Columns;
        }

        public void Update()
        {
            currentFrame++;
            if (currentFrame == totalFrames)
                currentFrame = 0;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = currentFrame / Columns;
            int column = currentFrame % Columns;


            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);

            // DEBUGGING
            // Debug.WriteLine("Column: " + column);
            // Debug.WriteLine("Width: " + width);
            // Debug.WriteLine(sourceRectangle);

            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);

            //spriteBatch.Begin();
            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);
            //spriteBatch.End();
        }

        public void DrawFrame(SpriteBatch spriteBatch, Vector2 location, int frame)
        {
            try
            {
                this.currentFrame = frame;
                this.Draw(spriteBatch, location);
            }
            catch (Exception e)
            {
                Debug.Write("Could not draw specified frame: " + e);
            }
        }
    }
}