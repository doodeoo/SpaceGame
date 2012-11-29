using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceShoot
{
    class Background
    {
        private Texture2D mytexture; 
        private Random r;
        private int maxY;
        private int maxX;
        // !! Change these queues to something with indexes and implement THREADING
        private Queue<Vector2> stars;
        private Queue<Vector2> bgStars;

        public Background(int x, int y, Texture2D starTexture)
        {
            mytexture = starTexture;
            r = new Random();
            this.maxY = y;
            this.maxX = x;
            stars = new Queue<Vector2>();
            bgStars = new Queue<Vector2>();
            GenerateInitial(); // Part of Initial Loading
        }

        public void Update(float deltaY)
        {
            ScrollStars(deltaY);
            GenerateStars(r.Next(2), deltaY);
        }

        private void GenerateInitial()
        {
            while (bgStars.Count < 350) // Experimentally determined value for the number of bgStars onscreen before they fill it
            {
                ScrollStars(2);
                GenerateStars(r.Next(2), 2);
            }
        }

        private void GenerateStars(int num, float deltaY)
        {
            for (int i = 0; i < num; i++)
            {
                stars.Enqueue(new Vector2(r.Next(maxX), 0));
                if ((int)(deltaY / (1.5f)) != 0)
                {
                    bgStars.Enqueue(new Vector2(r.Next(maxX), 0));
                }
            }
        }

        private void ScrollStars(float deltaY)
        {
            for (int i = 0; i < stars.Count; i++)
            {
                Vector2 cur = stars.Dequeue();
                cur.Y += deltaY;
                if (cur.Y <= maxY)
                    stars.Enqueue(cur);
            } 
            for (int i = 0; i < bgStars.Count; i++)
            {
                Vector2 cur = bgStars.Dequeue();
                cur.Y += (int)(deltaY/(1.5f));
                if (cur.Y <= maxY)
                    bgStars.Enqueue(cur);
            }
        }

        public void Draw(SpriteBatch batch)
        {
            DrawHelper(batch, stars, 3, Color.White);
            DrawHelper(batch, bgStars, 2, Color.DarkGray);
        }

        private void DrawHelper(SpriteBatch batch, Queue<Vector2> set, int size, Color color)
        {
            for (int i = 0; i < set.Count; i++)
            {
                Vector2 starLoc = set.Dequeue();
                Rectangle starRect = new Rectangle((int)starLoc.X, (int)starLoc.Y, size, size);
                batch.Draw(mytexture, starRect, color);
                set.Enqueue(starLoc);
            }
        }
    }
}
