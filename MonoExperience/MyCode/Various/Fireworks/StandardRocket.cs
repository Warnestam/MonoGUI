using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;



/*
 * File:		StandardRocket
 * Purpose:		A firework example
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2009-05-24  RW	
 * 
 * History:		2009-05-24  RW  Created
 * 
 */

namespace MonoExperience.Fireworks
{
    class StandardRocket: IFirework, IFireEngine
    {

        #region Private members

        private const Single FLARE_TIME = 3.0f;//seconds

        private Particle fRocket;
        private List<Particle> fFlares;
        private Vector2 fStartPosition;
        private Vector2 fStartVelocity;
        private DateTime fStartTime;
        private DateTime fFlareTime;

        private bool fHasStarted;
        private bool fHasEnded;
        private bool fHasExploded;

        #endregion

        #region Static members

        private static Texture2D fRocketTexture;
        private static Texture2D fFlareTexture;
        private static SpriteBatch fSpriteBatch;
        private static Random fRandom = new Random();

        #endregion

        #region IFirework Members

        bool IFirework.HasStarted()
        {
            return fHasStarted;
        }

        bool IFirework.HasEnded()
        {
            return fHasEnded;
        }

        void IFirework.IgniteFirework()
        {
            //Vector2 velocity = new Vector2(
            //    Convert.ToSingle((fRandom.NextDouble()-0.5f) * 50.0f), 
            //    Convert.ToSingle(-30.0 - 30 * fRandom.NextDouble()));
            //Particle particle = new Particle(position, velocity, fRocketTexture);

            fRocket = new Particle(fStartPosition, fStartVelocity, fRocketTexture);

            fStartTime = DateTime.Now;

            fHasStarted = true;
        }

        void IFirework.SetStartPosition(Vector2 position, Vector2 velocity)
        {
            fStartPosition = position;
            fStartVelocity = velocity;
        }

        void IFirework.Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (fHasStarted && !fHasEnded)
            {
                double t = 5*(DateTime.Now - fStartTime).TotalSeconds;
                
                if (fHasExploded)
                {
                    foreach (Particle particle in fFlares)
                    {
                        particle.Position.X += particle.Velocity.X;
                        particle.Position.Y += particle.Velocity.Y;
                        particle.Velocity.X = particle.Velocity.X * 0.99f;
                        particle.Velocity.Y = particle.Velocity.Y * 0.99f;
                    }
                    if ((DateTime.Now - fFlareTime).TotalSeconds > FLARE_TIME)
                        fHasEnded = true;
                }
                else
                {
                    double v = fStartVelocity.Y + Particle.GRAVITY * t;
                    double y = fStartPosition.Y + fStartVelocity.Y * t + (Particle.GRAVITY * t * t / 2);
                    double x = fStartPosition.X + fStartVelocity.X * t;
                    fRocket.Position = new Vector2(Convert.ToSingle(x), Convert.ToSingle(y));
                    if (y > fStartPosition.Y)
                        fHasEnded = true;
                    else if (v > 0.1)
                    {
                        fHasExploded = true;
                        fFlareTime = DateTime.Now;
                        int flares = fRandom.Next(500);
                        
                        fFlares = new List<Particle>();
                        int colorType = 0;
                        for (int i = 0; i < flares; i++)
                        {
                            Vector2 velocity;
                            double r = fRandom.NextDouble() * 2 * Math.PI;
                            double speed = fRandom.NextDouble() * 4.0f;
                            double sx = Math.Sin(r) * speed;
                            double sy = Math.Cos(r) * speed;

                            velocity.X = 0.02f * fRocket.Velocity.X + Convert.ToSingle(sx);
                            velocity.Y = 0.02f * fRocket.Velocity.Y + Convert.ToSingle(sy);
                            Particle particle = new Particle(
                                fRocket.Position,
                                velocity,
                                fFlareTexture);
                            switch (colorType)
                            {
                                case 0:
                                    particle.Color = new Color(255, 192, 64);
                                    break;
                                case 1:
                                    particle.Color = new Color(128,192,255);
                                    break;
                            }
                            colorType++;
                            if (colorType == 2)
                                colorType = 0;
                            fFlares.Add(particle);
                        }
                    }
                }
            }
        }

        void IFirework.Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (fHasStarted && !fHasEnded)
            {
                if (fHasExploded)
                {
                    float aliveFactor = Convert.ToSingle((DateTime.Now - fFlareTime).TotalSeconds / FLARE_TIME);
                    if (aliveFactor > 1)
                        aliveFactor = 1;
                    byte alfa = Convert.ToByte(255-255 * aliveFactor);
                        
                    foreach (Particle particle in fFlares)
                    {
                        Color color = particle.Color;
                        color.A = alfa;
                        fSpriteBatch.Draw(particle.Texture, particle.Position, color);
                    }
                }
                else
                {
                    fSpriteBatch.Draw(fRocket.Texture, fRocket.Position, Color.White);
                }
            }
        }

        #endregion

        #region IFireEngine Members

        IFirework IFireEngine.CreateFirework()
        {
            return new StandardRocket();
        }

        void IFireEngine.LoadContent(Game game, SpriteBatch spriteBatch)
        {
            StandardRocket.fSpriteBatch = spriteBatch;
            StandardRocket.fRocketTexture = game.Content.Load<Texture2D>(@"Fireworks\StandardRocket\flare4");
            StandardRocket.fFlareTexture = game.Content.Load<Texture2D>(@"Fireworks\StandardRocket\flare4");
        }

        #endregion
    }
}
