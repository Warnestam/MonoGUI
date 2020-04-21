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
 * File:		BurningRocket
 * Purpose:		A IFirework effect
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2009-05-24  RW	
 * 
 * History:		2009-05-24  RW  Created
 * 
 */
namespace MonoExperience.Fireworks
{
    class BurningRocket: IFirework, IFireEngine
    {

        #region Private members

        private const Single FLARE_TIME = 2.0f;//seconds

        private Particle fRocket;
        private List<Particle> fFlares = new List<Particle>();
        private List<Particle> fFlareRemoveList = new List<Particle>();
        private Vector2 fStartPosition;
        private Vector2 fStartVelocity;
        private DateTime fStartTime;

        private bool fHasStarted;
        private bool fHasEnded;
        private bool fNoMoreFlares;

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

                if (!fNoMoreFlares)
                {
                    if (fRandom.Next(2) == 0)
                    {
                        Vector2 position = new Vector2(
                            fRocket.Position.X + 8,
                            fRocket.Position.Y + 8);
                        Vector2 velocity = new Vector2(
                            Convert.ToSingle(5.0 * (fRandom.NextDouble() - 0.5f)),
                            Convert.ToSingle(5.0 * (fRandom.NextDouble() - 0.5f)));
                        Particle flare = new Particle(position, velocity, fFlareTexture);
                        fFlares.Add(flare);
                    }
                }
                foreach (Particle particle in fFlares)
                {
                    particle.Move(t);
                    if ((DateTime.Now - particle.StartDate).TotalSeconds > FLARE_TIME)
                    {
                        fFlareRemoveList.Add(particle);
                    }
                }
                if (fFlareRemoveList.Count > 0)
                {
                    foreach (Particle particle in fFlareRemoveList)
                    {
                        fFlares.Remove(particle);
                    }
                    fFlareRemoveList.Clear();
                }


                fRocket.Move(t);
                if (fRocket.Position.Y > fStartPosition.Y)
                {
                    fNoMoreFlares = true;
                    if (fNoMoreFlares && fFlares.Count == 0)
                        fHasEnded = true;
                }
                else if (fRocket.Velocity.Y > 2)
                {
                    fNoMoreFlares = true;
                }
            }
        }

        void IFirework.Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (fHasStarted && !fHasEnded)
            {
                foreach (Particle particle in fFlares)
                {
                    float aliveFactor = Convert.ToSingle((DateTime.Now - particle.StartDate).TotalSeconds / FLARE_TIME);
                    if (aliveFactor > 1)
                        aliveFactor = 1;
                    byte alfa = Convert.ToByte(255 - 255 * aliveFactor);
                    Color color = Color.White;
                    color.A = alfa;
                    fSpriteBatch.Draw(particle.Texture, particle.Position, color);
                }
                fSpriteBatch.Draw(fRocket.Texture, fRocket.Position, Color.White);
            }
        }

        #endregion

        #region IFireEngine Members

        IFirework IFireEngine.CreateFirework()
        {
            return new BurningRocket();
        }

        void IFireEngine.LoadContent(Game game, SpriteBatch spriteBatch)
        {
            BurningRocket.fSpriteBatch = spriteBatch;
            BurningRocket.fRocketTexture = game.Content.Load<Texture2D>(@"Fireworks\BurningRocket\main");
            BurningRocket.fFlareTexture = game.Content.Load<Texture2D>(@"Fireworks\BurningRocket\flare");
        }

        #endregion

    }
}
