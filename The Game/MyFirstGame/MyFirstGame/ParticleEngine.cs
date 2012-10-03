using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceShoot
{
    public class ParticleEngine
    {
        private Random r;
        public Vector2 EmitterLocation { get; set; }
        private List<Particle> particles;
        private List<Texture2D> textures;

        public ParticleEngine(List<Texture2D> textures, Vector2 location)
        {
            EmitterLocation = location;
            this.textures = textures;
            this.particles = new List<Particle>();
            r = new Random();
        }

        public void Update()
        {
            int total = 10;

            for (int i = 0; i < total; i++)
            {
                particles.Add(GenerateNewParticle());
            }

            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (particles[particle].Lifespan <= 0)
                {
                    particles.RemoveAt(particle);
                    particle--;
                }
            }
        }

        private Particle GenerateNewParticle()
        {
            Texture2D texture = textures[r.Next(textures.Count)];
            Vector2 position = EmitterLocation;
            Vector2 velocity = new Vector2(
                                    1f * (float)(r.NextDouble() * 4 - 2),
                                    1f * (float)(r.NextDouble() * 10 + 10));
            float angle = 0;
            float angularVelocity = 0.1f * (float)(r.NextDouble() * 2 - 1);
            Color color = new Color(
                        1f * (float)r.NextDouble(),
                        0f * (float)r.NextDouble(),
                        0f * (float)r.NextDouble());
            float size = (float)r.NextDouble();
            int Lifespan = 1 + r.Next(5);

            return new Particle(texture, position, velocity, angle, angularVelocity, color, size, Lifespan);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Begin();
            for (int index = 0; index < particles.Count; index++)
            {
                particles[index].Draw(spriteBatch);
            }
            //spriteBatch.End();
        }
    }
}