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


    class DigitEffect
    {

        public MyParticles Particles { get; private set; }

        public int OrigoX { get; private set; }
        public int OrigoY { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Radius { get; private set; }

        private int fLastSecond = 0;
        private LineConnector[] fDigits = new LineConnector[10];


        public DigitEffect(int w, int h, int points)
        {
            Particles = new MyParticles(w, h, points);
            Particles.Settings = ParticleSettings.None;

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
            DigitUpdate(gameTime);
            Particles.Update(gameTime);
        }


        public void DigitUpdate(GameTime gameTime)
        {
            int milliseconds = gameTime.ElapsedGameTime.Milliseconds;
            int second = gameTime.TotalGameTime.Seconds;
            int digit = second % 10;

            bool rotate = true;
            
            if (rotate)
            {
                LineConnector connector = GetDigit(digit);
                if (connector != null)
                {
                    double f = gameTime.TotalGameTime.Milliseconds / 1000.0f;
                    //double f = (gameTime.TotalGameTime.Seconds % 10) / 10.0f;

                    var points = connector.DistributePoints(Particles.Count, f);
                    for (int i = 0; i < points.Count; i++)
                    {
                        var particle = Particles[i];
                        var point = points[i];
                        int position = 2 * Radius * i / Particles.Count - Radius;
                        particle.Destination = new Vector2(OrigoX + point.X, OrigoY + point.Y);
                    }
                }
            }
            else if (second != fLastSecond)
            {
                LineConnector connector = GetDigit(digit);
                if (connector != null)
                {
                    var points = connector.DistributePoints(Particles.Count, 0.0f);
                    for (int i = 0; i < points.Count; i++)
                    {
                        var particle = Particles[i];
                        var point = points[i];
                        int position = 2 * Radius * i / Particles.Count - Radius;
                        particle.Destination = new Vector2(OrigoX + point.X, OrigoY + point.Y);
                    }
                }
            }

        }

        private LineConnector GetDigit(int digit)
        {
            int ox = -3;
            int oy = -3;
            int dx = 30;
            int dy = 30;

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
