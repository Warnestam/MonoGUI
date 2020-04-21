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


    class SecondEffect
    {

        public MyParticles Particles1 { get; private set; }
        public MyParticles Particles2 { get; private set; }

        public int OrigoX { get; private set; }
        public int OrigoY { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Radius { get; private set; }

        private int fLastSecond = 0;
        private const int SIZE = 30;
        private const int SEGMENTS_X = 6;

        private LineConnector[] fDigits = new LineConnector[10];


        public SecondEffect(int w, int h, int points)
        {
            Particles1 = new MyParticles(w, h, points);
            Particles1.Settings = ParticleSettings.None;
            Particles2 = new MyParticles(w, h, points);
            Particles2.Settings = ParticleSettings.None;

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
            Particles1.Draw(spriteBatch, texture);
            Particles2.Draw(spriteBatch, texture);
        }

        public void Update(GameTime gameTime)
        {
            DigitUpdate(gameTime);
            Particles1.Update(gameTime);
            Particles2.Update(gameTime);
        }


        public void DigitUpdate(GameTime gameTime)
        {
            int milliseconds = gameTime.TotalGameTime.Milliseconds;
            int second = gameTime.TotalGameTime.Seconds;
            int d1 = second / 10;
            int d2 = second % 10;
            DrawDigit(0, d1, milliseconds);
            DrawDigit(1, d2, milliseconds);
        }

        private void DrawDigit(int digitIndex, int digit, double milliseconds)
        {
            MyParticles particles = null;

            int ox = 0;
            int dx = SIZE * SEGMENTS_X;

            if (digitIndex==0)
            {
                particles = Particles1;
                ox = OrigoX - dx/2;
            }
            else
            {
                particles = Particles2;
                ox = OrigoX + dx/2;
            }

     
            LineConnector connector = GetDigit(digit);
            if (connector != null)
            {
                double f = milliseconds / 1000.0f;
              
                var points = connector.DistributePoints(particles.Count, f);
                for (int i = 0; i < points.Count; i++)
                {
                    var particle = particles[i];
                    var point = points[i];
                    int position = 2 * Radius * i / particles.Count - Radius;
                    particle.Destination = new Vector2(ox + point.X, OrigoY + point.Y);
                }
            }

        }

        private LineConnector GetDigit(int digit)
        {
            int ox = -3;
            int oy = -3;
            int dx = SIZE;
            int dy = SIZE;

            if (fDigits[digit] == null)
            {
                LineConnector lc = new LineConnector();
                fDigits[digit] = lc;
                switch (digit)
                {
                    case 0:
                        lc.Lines.Add(new Line());
                        lc.Lines.Add(new Line());
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((1 + ox) * dx, (1 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((4 + ox) * dx, (1 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((4 + ox) * dx, (6 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((1 + ox) * dx, (6 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((1 + ox) * dx, (1 + oy) * dy));
                        break;
                    case 1:
                        lc.Lines.Add(new Line());
                        lc.Lines[0].Points.Add(new Point((2 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((3 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((3 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((2 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((2 + ox) * dx, (0 + oy) * dy));
                        break;
                    case 2:
                        lc.Lines.Add(new Line());
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (4 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((1 + ox) * dx, (4 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((1 + ox) * dx, (6 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (6 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (3 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((4 + ox) * dx, (3 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((4 + ox) * dx, (1 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (1 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (0 + oy) * dy));
                        break;
                    case 3:
                        lc.Lines.Add(new Line());
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (6 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((4 + ox) * dx, (6 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((4 + ox) * dx, (4 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (4 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (3 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((4 + ox) * dx, (3 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((4 + ox) * dx, (1 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (1 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (0 + oy) * dy));
                        break;
                    case 4:
                        lc.Lines.Add(new Line());
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (4 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((4 + ox) * dx, (4 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((4 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((4 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((4 + ox) * dx, (3 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((1 + ox) * dx, (3 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((1 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (0 + oy) * dy));
                        break;
                    case 5:
                        lc.Lines.Add(new Line());
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (1 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((1 + ox) * dx, (1 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((1 + ox) * dx, (3 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (3 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (6 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((4 + ox) * dx, (6 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((4 + ox) * dx, (4 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (4 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (0 + oy) * dy));
                        break;
                    case 6:
                        lc.Lines.Add(new Line());
                        lc.Lines.Add(new Line());
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (1 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((1 + ox) * dx, (1 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((1 + ox) * dx, (3 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (3 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((1 + ox) * dx, (4 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((4 + ox) * dx, (4 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((4 + ox) * dx, (6 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((1 + ox) * dx, (6 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((1 + ox) * dx, (1 + oy) * dy));
                        break;
                    case 7:
                        lc.Lines.Add(new Line());
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((4 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((4 + ox) * dx, (1 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (1 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (0 + oy) * dy));
                        break;
                    case 8:
                        lc.Lines.Add(new Line());
                        lc.Lines.Add(new Line());
                        lc.Lines.Add(new Line());
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((1 + ox) * dx, (1 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((4 + ox) * dx, (1 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((4 + ox) * dx, (3 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((1 + ox) * dx, (3 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((1 + ox) * dx, (1 + oy) * dy));
                        lc.Lines[2].Points.Add(new Point((1 + ox) * dx, (4 + oy) * dy));
                        lc.Lines[2].Points.Add(new Point((4 + ox) * dx, (4 + oy) * dy));
                        lc.Lines[2].Points.Add(new Point((4 + ox) * dx, (6 + oy) * dy));
                        lc.Lines[2].Points.Add(new Point((1 + ox) * dx, (6 + oy) * dy));
                        lc.Lines[2].Points.Add(new Point((1 + ox) * dx, (1 + oy) * dy));
                        break;
                    case 9:
                        lc.Lines.Add(new Line());
                        lc.Lines.Add(new Line());
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((4 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((4 + ox) * dx, (4 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (4 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((1 + ox) * dx, (1 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((4 + ox) * dx, (1 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((4 + ox) * dx, (3 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((1 + ox) * dx, (3 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((1 + ox) * dx, (1 + oy) * dy));
                        break;
                    default:
                        fDigits[digit] = null;
                        break;
                }

            }
            return fDigits[digit];
        }
        
    }
}
