using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// RW 2019-03-12

namespace MonoExperience.Engines.CountEngine
{


    class EvaEffect
    {

        public MyParticles[] Particles { get; private set; } = new MyParticles[4];

        public int OrigoX { get; private set; }
        public int OrigoY { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Radius { get; private set; }

        private int fTimePassedSinceLast = 0;
        private const double SCALE = 1.0f;

        private LineConnector[] fCharacters = new LineConnector[4];
        private int fStartType = 0;
        private bool fChanged = true;

        public EvaEffect(int w, int h, int points)
        {
            for (int i = 0; i < Particles.Length; i++)
            {
                Particles[i] = new MyParticles(w, h, points);
                Particles[i].Settings = ParticleSettings.None;
            }
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
            for (int i = 0; i < Particles.Length; i++)
            {
                Particles[i].Draw(spriteBatch, texture);
            }
        }

        public void Update(GameTime gameTime)
        {
            EvaUpdate(gameTime);
            for (int i = 0; i < Particles.Length; i++)
            {
                Particles[i].Update(gameTime);
            }
        }


        private void EvaUpdate(GameTime gameTime)
        {
            int milliseconds = gameTime.ElapsedGameTime.Milliseconds;
            fTimePassedSinceLast += milliseconds;
            if (fTimePassedSinceLast >= 4000)
            {
                fTimePassedSinceLast = 0;
                fStartType = (fStartType + 3) % 4;
                fChanged = true;
            }

            if (fChanged)
            {
                fChanged = false;
                int charType = fStartType;
                for (int i = 0; i < Particles.Length; i++)
                {
                    SetCharacterData(i, charType);
                    charType = (charType + 1) % 4;
                }
            }

        }

        private void SetCharacterData(int characterIndex, int characterType)
        {
            var particles = Particles[characterIndex];
            LineConnector connector = GetConnector(characterType);
            if (connector != null)
            {
                var points = connector.DistributePoints(particles.Count, 0);
                for (int i = 0; i < points.Count; i++)
                {
                    var particle = particles[i];
                    var point = points[i];
                    int position = 2 * Radius * i / particles.Count - Radius;
                    particle.Destination = new Vector2(point.X, point.Y);
                }
            }
        }

        private LineConnector GetConnector(int index)
        {
            int ox = -300 + Width / 2;
            int oy = -300 + Height / 2;

            if (fCharacters[index] == null)
            {
                LineConnector lc = new LineConnector();
                fCharacters[index] = lc;
                switch (index)
                {
                    case 0:
                        lc.Lines.Add(new Line());
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((141 + ox) * SCALE), Convert.ToInt16((163 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((225 + ox) * SCALE), Convert.ToInt16((163 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((217 + ox) * SCALE), Convert.ToInt16((192 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((210 + ox) * SCALE), Convert.ToInt16((173 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((171 + ox) * SCALE), Convert.ToInt16((173 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((171 + ox) * SCALE), Convert.ToInt16((219 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((200 + ox) * SCALE), Convert.ToInt16((218 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((209 + ox) * SCALE), Convert.ToInt16((205 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((209 + ox) * SCALE), Convert.ToInt16((243 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((199 + ox) * SCALE), Convert.ToInt16((228 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((172 + ox) * SCALE), Convert.ToInt16((230 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((172 + ox) * SCALE), Convert.ToInt16((276 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((226 + ox) * SCALE), Convert.ToInt16((256 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((226 + ox) * SCALE), Convert.ToInt16((286 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((139 + ox) * SCALE), Convert.ToInt16((286 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((151 + ox) * SCALE), Convert.ToInt16((278 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((151 + ox) * SCALE), Convert.ToInt16((174 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((141 + ox) * SCALE), Convert.ToInt16((163 + oy) * SCALE)));
                        break;
                    case 1:
                        lc.Lines.Add(new Line());
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((235 + ox) * SCALE), Convert.ToInt16((163 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((281 + ox) * SCALE), Convert.ToInt16((163 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((269 + ox) * SCALE), Convert.ToInt16((173 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((300 + ox) * SCALE), Convert.ToInt16((261 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((335 + ox) * SCALE), Convert.ToInt16((172 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((322 + ox) * SCALE), Convert.ToInt16((163 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((362 + ox) * SCALE), Convert.ToInt16((163 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((350 + ox) * SCALE), Convert.ToInt16((173 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((301 + ox) * SCALE), Convert.ToInt16((288 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((290 + ox) * SCALE), Convert.ToInt16((288 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((245 + ox) * SCALE), Convert.ToInt16((172 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((235 + ox) * SCALE), Convert.ToInt16((163 + oy) * SCALE)));
                        break;
                    case 2:
                        lc.Lines.Add(new Line());
                        lc.Lines.Add(new Line());
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((402 + ox) * SCALE), Convert.ToInt16((163 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((413 + ox) * SCALE), Convert.ToInt16((163 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((457 + ox) * SCALE), Convert.ToInt16((278 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((469 + ox) * SCALE), Convert.ToInt16((287 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((422 + ox) * SCALE), Convert.ToInt16((287 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((435 + ox) * SCALE), Convert.ToInt16((278 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((422 + ox) * SCALE), Convert.ToInt16((243 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((381 + ox) * SCALE), Convert.ToInt16((243 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((369 + ox) * SCALE), Convert.ToInt16((277 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((381 + ox) * SCALE), Convert.ToInt16((287 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((338 + ox) * SCALE), Convert.ToInt16((287 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((354 + ox) * SCALE), Convert.ToInt16((277 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((402 + ox) * SCALE), Convert.ToInt16((163 + oy) * SCALE)));
                        lc.Lines[1].Points.Add(new Point(Convert.ToInt16((403 + ox) * SCALE), Convert.ToInt16((188 + oy) * SCALE)));
                        lc.Lines[1].Points.Add(new Point(Convert.ToInt16((418 + ox) * SCALE), Convert.ToInt16((232 + oy) * SCALE)));
                        lc.Lines[1].Points.Add(new Point(Convert.ToInt16((386 + ox) * SCALE), Convert.ToInt16((232 + oy) * SCALE)));
                        lc.Lines[1].Points.Add(new Point(Convert.ToInt16((403 + ox) * SCALE), Convert.ToInt16((188 + oy) * SCALE)));
                        break;
                    case 3:
                        lc.Lines.Add(new Line());
                        lc.Lines.Add(new Line());
                        lc.Lines.Add(new Line());
                        lc.Lines.Add(new Line());
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((281 + ox) * SCALE), Convert.ToInt16((375 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((263 + ox) * SCALE), Convert.ToInt16((359 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((263 + ox) * SCALE), Convert.ToInt16((349 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((279 + ox) * SCALE), Convert.ToInt16((336 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((292 + ox) * SCALE), Convert.ToInt16((335 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((300 + ox) * SCALE), Convert.ToInt16((364 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((287 + ox) * SCALE), Convert.ToInt16((367 + oy) * SCALE)));
                        lc.Lines[0].Points.Add(new Point(Convert.ToInt16((281 + ox) * SCALE), Convert.ToInt16((375 + oy) * SCALE)));

                        lc.Lines[1].Points.Add(new Point(Convert.ToInt16((290 + ox) * SCALE), Convert.ToInt16((396 + oy) * SCALE)));
                        lc.Lines[1].Points.Add(new Point(Convert.ToInt16((285 + ox) * SCALE), Convert.ToInt16((408 + oy) * SCALE)));
                        lc.Lines[1].Points.Add(new Point(Convert.ToInt16((278 + ox) * SCALE), Convert.ToInt16((415 + oy) * SCALE)));
                        lc.Lines[1].Points.Add(new Point(Convert.ToInt16((265 + ox) * SCALE), Convert.ToInt16((415 + oy) * SCALE)));
                        lc.Lines[1].Points.Add(new Point(Convert.ToInt16((254 + ox) * SCALE), Convert.ToInt16((398 + oy) * SCALE)));
                        lc.Lines[1].Points.Add(new Point(Convert.ToInt16((253 + ox) * SCALE), Convert.ToInt16((386 + oy) * SCALE)));
                        lc.Lines[1].Points.Add(new Point(Convert.ToInt16((264 + ox) * SCALE), Convert.ToInt16((377 + oy) * SCALE)));
                        lc.Lines[1].Points.Add(new Point(Convert.ToInt16((281 + ox) * SCALE), Convert.ToInt16((378 + oy) * SCALE)));
                        lc.Lines[1].Points.Add(new Point(Convert.ToInt16((283 + ox) * SCALE), Convert.ToInt16((389 + oy) * SCALE)));
                        lc.Lines[1].Points.Add(new Point(Convert.ToInt16((290 + ox) * SCALE), Convert.ToInt16((396 + oy) * SCALE)));

                        lc.Lines[2].Points.Add(new Point(Convert.ToInt16((313 + ox) * SCALE), Convert.ToInt16((387 + oy) * SCALE)));
                        lc.Lines[2].Points.Add(new Point(Convert.ToInt16((323 + ox) * SCALE), Convert.ToInt16((392 + oy) * SCALE)));
                        lc.Lines[2].Points.Add(new Point(Convert.ToInt16((333 + ox) * SCALE), Convert.ToInt16((402 + oy) * SCALE)));
                        lc.Lines[2].Points.Add(new Point(Convert.ToInt16((332 + ox) * SCALE), Convert.ToInt16((412 + oy) * SCALE)));
                        lc.Lines[2].Points.Add(new Point(Convert.ToInt16((315 + ox) * SCALE), Convert.ToInt16((425 + oy) * SCALE)));
                        lc.Lines[2].Points.Add(new Point(Convert.ToInt16((302 + ox) * SCALE), Convert.ToInt16((425 + oy) * SCALE)));
                        lc.Lines[2].Points.Add(new Point(Convert.ToInt16((295 + ox) * SCALE), Convert.ToInt16((413 + oy) * SCALE)));
                        lc.Lines[2].Points.Add(new Point(Convert.ToInt16((296 + ox) * SCALE), Convert.ToInt16((397 + oy) * SCALE)));
                        lc.Lines[2].Points.Add(new Point(Convert.ToInt16((306 + ox) * SCALE), Convert.ToInt16((395 + oy) * SCALE)));
                        lc.Lines[2].Points.Add(new Point(Convert.ToInt16((313 + ox) * SCALE), Convert.ToInt16((387 + oy) * SCALE)));

                        lc.Lines[3].Points.Add(new Point(Convert.ToInt16((305+ox) * SCALE), Convert.ToInt16((365+oy) * SCALE)));
                        lc.Lines[3].Points.Add(new Point(Convert.ToInt16((318+ox) * SCALE), Convert.ToInt16((346+oy) * SCALE)));
                        lc.Lines[3].Points.Add(new Point(Convert.ToInt16((329+ox) * SCALE), Convert.ToInt16((346+oy) * SCALE)));
                        lc.Lines[3].Points.Add(new Point(Convert.ToInt16((336+ox) * SCALE), Convert.ToInt16((352+oy) * SCALE)));
                        lc.Lines[3].Points.Add(new Point(Convert.ToInt16((341+ox) * SCALE), Convert.ToInt16((364+oy) * SCALE)));
                        lc.Lines[3].Points.Add(new Point(Convert.ToInt16((342+ox) * SCALE), Convert.ToInt16((375+oy) * SCALE)));
                        lc.Lines[3].Points.Add(new Point(Convert.ToInt16((333+ox) * SCALE), Convert.ToInt16((384+oy) * SCALE)));
                        lc.Lines[3].Points.Add(new Point(Convert.ToInt16((315+ox) * SCALE), Convert.ToInt16((383+oy) * SCALE)));
                        lc.Lines[3].Points.Add(new Point(Convert.ToInt16((313+ox) * SCALE), Convert.ToInt16((372+oy) * SCALE)));
                        lc.Lines[3].Points.Add(new Point(Convert.ToInt16((305+ox) * SCALE), Convert.ToInt16((365+oy) * SCALE)));

                        break;
                    default:
                        fCharacters[index] = null;
                        break;
                }

            }
            return fCharacters[index];
        }



    }
}
