using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// RW 2019-03-10

namespace MonoExperience.Engines.CountEngine
{


    class SideEffect
    {

        public MyParticles Particles { get; private set; }

        public int OrigoX { get; private set; }
        public int OrigoY { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Radius { get; private set; }

        private int fLastSecond = 0;
        

        public SideEffect(int w, int h, int points)
        {
            Particles = new MyParticles(w, h, points);
            Particles.Settings = ParticleSettings.Shaky;

            Width = w;
            Height = h;
            OrigoX = w / 2;
            OrigoY = h / 2;

            if (Width > Height)
            {
                Radius = Convert.ToInt32(Height / 2 * 0.6);
            }
            else
            {
                Radius = Convert.ToInt32(Width / 2 * 0.6);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            Particles.Draw(spriteBatch, texture);
        }

        public void Update(GameTime gameTime)
        {
            SideUpdate(gameTime);
            Particles.Update(gameTime);
        }

        private void SideUpdate(GameTime gameTime)
        {
            int milliseconds = gameTime.ElapsedGameTime.Milliseconds;
            int second = gameTime.TotalGameTime.Seconds;
            if (second != fLastSecond)
            {
                switch (second % 10)
                {
                    case 0:
                        SetDestinationToLeftSide();
                        break;
                    case 1:
                        SetDestinationToBottomSide();
                        break;
                    case 2:
                        SetDestinationToRightSide();
                        break;
                    case 3:
                        SetDestinationToTopSide();
                        break;
                    case 4:
                        SetDestinationToCircle();
                        break;
                    case 5:
                        SetDestinationToCircleOpposite();
                        break;
                }
            }
        }

        private void SetDestinationToLeftSide()
        {
            for (int i = 0; i < Particles.Count; i++)
            {
                var particle = Particles[i];
                int position = 2 * Radius * i / Particles.Count - Radius;
                particle.Destination = new Vector2(OrigoX - Radius, OrigoY + position);
            }
        }

        private void SetDestinationToRightSide()
        {
            for (int i = 0; i < Particles.Count; i++)
            {
                var particle = Particles[i];
                int position = 2 * Radius * i / Particles.Count - Radius;
                particle.Destination = new Vector2(OrigoX + Radius, OrigoY - position);
            }
        }

        private void SetDestinationToBottomSide()
        {
            for (int i = 0; i < Particles.Count; i++)
            {
                var particle = Particles[i];
                int position = 2 * Radius * i / Particles.Count - Radius;
                particle.Destination = new Vector2(OrigoX - position, OrigoY + Radius);
            }
        }

        private void SetDestinationToTopSide()
        {
            for (int i = 0; i < Particles.Count; i++)
            {
                var particle = Particles[i];
                int position = 2 * Radius * i / Particles.Count - Radius;
                particle.Destination = new Vector2(OrigoX + position, OrigoY - Radius);
            }
        }

        private void SetDestinationToCircle()
        {
            for (int i = 0; i < Particles.Count; i++)
            {
                var particle = Particles[i];
                double angle = Math.PI * 2.0f * i / Particles.Count;
                int x = Convert.ToInt32(200.0f * Math.Cos(angle));
                int y = Convert.ToInt32(200.0f * Math.Sin(angle));
                particle.Destination = new Vector2(OrigoX + x, OrigoY + y);
            }
        }

        private void SetDestinationToCircleOpposite()
        {
            for (int i = 0; i < Particles.Count; i++)
            {
                var particle = Particles[i];
                double angle = -Math.PI * 2.0f * i / Particles.Count;
                int x = Convert.ToInt32(200.0f * Math.Cos(angle));
                int y = Convert.ToInt32(200.0f * Math.Sin(angle));
                particle.Destination = new Vector2(OrigoX + x, OrigoY + y);
            }
        }



    }
}
