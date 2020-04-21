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


    class DownCounterEffect
    {

        private enum DigitType { digit0 = 0, digit1, digit2, digit3, digit4, digit5, digit6, digit7, digit8, digit9, colon1, colon2 };

        public int OrigoX { get; private set; }
        public int OrigoY { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        private const int SIZE = 13;
        private const int SEGMENTS_X = 6;
        private DateTime fGoalDate;
        private MyParticles[] fParticles;
        private LineConnector[] fDigits = new LineConnector[13];//As DigitType
        private DigitType[] fCurrentDigits;

        public DownCounterEffect(DateTime goalDate, int w, int h, int points)
        {
            fGoalDate = goalDate;
            fParticles = new MyParticles[12];
            fCurrentDigits = new DigitType[12];
            for (int i = 0; i < 12; i++)
            {
                fParticles[i] = new MyParticles(w, h, points);
                fParticles[i].Settings = ParticleSettings.None;
            }

            Width = w;
            Height = h;
            OrigoX = w / 2;
            OrigoY = h / 2;

        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            foreach (var particles in fParticles)
            {
                particles.Draw(spriteBatch, texture);
            }
        }

        public void Update(GameTime gameTime)
        {
            UpdateDigits(gameTime);
            foreach (var particles in fParticles)
            {
                particles.Update(gameTime);
            }
        }


        public void UpdateDigits(GameTime gameTime)
        {
            TimeSpan span = fGoalDate - DateTime.Now;
            int days = 0;
            int hours = 0;
            int minutes = 0;
            int seconds = 0;
            if (span.TotalSeconds > 0)
            {
                days = span.Days;
                hours = span.Hours;
                minutes = span.Minutes;
                seconds = span.Seconds;
                if (days > 999)
                {
                    days = 999;
                    hours = 99;
                    minutes = 99;
                    seconds = 99;
                }
            }
            DigitType colonType = DigitType.colon1;
            if ((seconds % 2) == 1)
                colonType = DigitType.colon2;

            int days1 = days / 100;
            int days2 = (days / 10) % 10;
            int days3 = days % 10;
            int hours1 = hours / 10;
            int hours2 = hours % 10;
            int minutes1 = minutes / 10;
            int minutes2 = minutes % 10;
            int seconds1 = seconds / 10;
            int seconds2 = seconds % 10;

            int index = 0;
            fCurrentDigits[index++] = (DigitType)days1;
            fCurrentDigits[index++] = (DigitType)days2;
            fCurrentDigits[index++] = (DigitType)days3;
            fCurrentDigits[index++] = colonType;
            fCurrentDigits[index++] = (DigitType)hours1;
            fCurrentDigits[index++] = (DigitType)hours2;
            fCurrentDigits[index++] = colonType;
            fCurrentDigits[index++] = (DigitType)minutes1;
            fCurrentDigits[index++] = (DigitType)minutes2;
            fCurrentDigits[index++] = colonType;
            fCurrentDigits[index++] = (DigitType)seconds1;
            fCurrentDigits[index++] = (DigitType)seconds2;

            //int  milliseconds = gameTime.TotalGameTime.Milliseconds;
            //double f = milliseconds / 1000.0f;

            int x0 = SIZE * SEGMENTS_X / 2;
            //int y0 = SIZE * SEGMENTS_X / 2;


            //if ((seconds1%2)==0)
            //{
            //    y0 = Height - SIZE * SEGMENTS_X;
            //}
            for (int digitIndex = 0; digitIndex < fCurrentDigits.Length; digitIndex++)
            {
                int ox = Width / 2 + (SIZE * SEGMENTS_X) * (digitIndex - 6);
                int y0 = SIZE * SEGMENTS_X / 2;
                int ms = gameTime.TotalGameTime.Seconds * 1000 + gameTime.TotalGameTime.Milliseconds + 50 * digitIndex;
                ms = ms % 60000;

                if (ms > 30000)
                {
                    y0 = Height - SIZE * SEGMENTS_X;
                }


                DigitType dType = fCurrentDigits[digitIndex];
                MyParticles particles = fParticles[digitIndex];
                LineConnector connector = GetDigit(dType);

                if (connector != null)
                {
                    var points = connector.DistributePoints(particles.Count, 0);
                    for (int i = 0; i < points.Count; i++)
                    {
                        var particle = particles[i];
                        var point = points[i];
                        particle.Destination = new Vector2(x0 + ox + point.X, y0 + point.Y);
                    }
                }
            }
        }

        private LineConnector GetDigit(DigitType digit)
        {
            int ox = -3;
            int oy = -3;
            int dx = SIZE;
            int dy = SIZE;

            if (fDigits[(int)digit] == null)
            {
                LineConnector lc = new LineConnector();
                fDigits[(int)digit] = lc;
                switch (digit)
                {
                    case DigitType.digit0:
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
                    case DigitType.digit1:
                        lc.Lines.Add(new Line());
                        lc.Lines[0].Points.Add(new Point((2 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((3 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((3 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((2 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((2 + ox) * dx, (0 + oy) * dy));
                        break;
                    case DigitType.digit2:
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
                    case DigitType.digit3:
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
                    case DigitType.digit4:
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
                    case DigitType.digit5:
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
                    case DigitType.digit6:
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
                    case DigitType.digit7:
                        lc.Lines.Add(new Line());
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (0 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((5 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((4 + ox) * dx, (7 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((4 + ox) * dx, (1 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (1 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((0 + ox) * dx, (0 + oy) * dy));
                        break;
                    case DigitType.digit8:
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
                    case DigitType.digit9:
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
                    case DigitType.colon1:
                        lc.Lines.Add(new Line());
                        lc.Lines.Add(new Line());
                        lc.Lines[0].Points.Add(new Point((2 + ox) * dx, (1 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((2 + ox) * dx, (3 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((2 + ox) * dx, (4 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((2 + ox) * dx, (6 + oy) * dy));
                        break;
                    case DigitType.colon2:
                        lc.Lines.Add(new Line());
                        lc.Lines.Add(new Line());
                        lc.Lines[0].Points.Add(new Point((3 + ox) * dx, (1 + oy) * dy));
                        lc.Lines[0].Points.Add(new Point((3 + ox) * dx, (3 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((3 + ox) * dx, (4 + oy) * dy));
                        lc.Lines[1].Points.Add(new Point((3 + ox) * dx, (6 + oy) * dy));
                        break;

                    default:
                        fDigits[(int)digit] = null;
                        break;
                }

            }
            return fDigits[(int)digit];
        }

    }
}
