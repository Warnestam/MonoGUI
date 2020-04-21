using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// RW 2019-03-10


namespace MonoExperience.Engines.CountEngine
{

    [Flags]
    public enum ParticleSettings { None = 0x00, Shaky = 0x01 }

    class MyParticles
    {
        private List<MyParticle> fParticles = new List<MyParticle>();
        public Random Random { get; private set; } = new System.Random();
        public Color ParticleColor { get; set; } = Color.AliceBlue;
        public int Count { get => fParticles.Count; }
        public MyParticle this[int index] { get => fParticles[index]; }
        public ParticleSettings Settings { get; set; }


        public MyParticles(int width, int height, int particles)
        {
            Settings = ParticleSettings.None;

            int ox = width >> 1;
            int oy = height >> 1;
            for (int i = 0; i < particles; i++)
            {
                fParticles.Add(new MyParticle(this, new Vector2(ox, oy)));
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            foreach (var particle in fParticles)
            {
                spriteBatch.Draw(texture, particle.DrawPosition, ParticleColor);
            }
            spriteBatch.End();
        }

        public void SetDestination(int particleIndex, Vector2 destination)
        {
            fParticles[particleIndex].Destination = destination;
        }

        public void Update(GameTime gameTime)
        {
            var milliseconds = gameTime.ElapsedGameTime.TotalMilliseconds;
            foreach (var particle in fParticles)
            {
                particle.Update(milliseconds);
            }

        }



    }
}


