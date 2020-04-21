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
 * File:		StandardRocket3
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
    class StandardRocket3: IFirework, IFireEngine
    {

        #region Private members

        private const Single FLARE_TIME = 2.0f;//seconds

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
                float t = Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
                
                if (fHasExploded)
                {
                    foreach (Particle particle in fFlares)
                    {
                        particle.Move(t);
                        if (fRandom.Next(10)==0)
                            particle.Color = new Color(
                                Convert.ToByte(128 + fRandom.Next(128)),
                                Convert.ToByte(128 + fRandom.Next(128)),
                                Convert.ToByte(128 + fRandom.Next(128)));

                    }
                    if ((DateTime.Now - fFlareTime).TotalSeconds > FLARE_TIME)
                        fHasEnded = true;
                    
                    
                }
                else
                {
                    fRocket.Move(t);
                    if (fRocket.Position.Y > fStartPosition.Y)
                        fHasEnded = true;
                    else if (fRocket.Velocity.Y > 0.1)
                    {
                        fHasExploded = true;
                        fFlareTime = DateTime.Now;
                        int flares = fRandom.Next(100);
                        
                        fFlares = new List<Particle>();
                        for (int i = 0; i < flares; i++)
                        {
                            Vector2 velocity;
                            double r = Math.PI * 1.5f + 0.9f*(fRandom.NextDouble()-0.5f);// fRandom.NextDouble() * 2 * Math.PI;
                            double speed = fRandom.NextDouble();
                            double sx = 40.0f*Math.Cos(r) * speed;
                            double sy = 40.0f*Math.Sin(r) * speed;

                            velocity.X = /*fRocket.Velocity.X +*/ Convert.ToSingle(sx);
                            velocity.Y = fRocket.Velocity.Y + Convert.ToSingle(sy);
                            Particle particle = new Particle(
                                fRocket.Position,
                                velocity,
                                fFlareTexture);
                            particle.Color = new Color(255, 0, 0);
                            
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
            return new StandardRocket3();
        }

        void IFireEngine.LoadContent(Game game, SpriteBatch spriteBatch)
        {
            StandardRocket3.fSpriteBatch = spriteBatch;
            StandardRocket3.fRocketTexture = game.Content.Load<Texture2D>(@"Fireworks\StandardRocket3\flare4");
            StandardRocket3.fFlareTexture = game.Content.Load<Texture2D>(@"Fireworks\StandardRocket3\flare4");
        }

        #endregion
    }
}
