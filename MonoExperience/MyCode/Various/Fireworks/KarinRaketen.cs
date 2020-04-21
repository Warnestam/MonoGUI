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
 * File:		KarinRaketen
 * Purpose:		Firework done by instructions from my daughter Karin
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2009-05-24  RW	
 * 
 * History:		2009-05-24  RW  Created
 * 
 */

namespace MonoExperience.Fireworks
{
    class KarinRaketen : IFirework, IFireEngine
    {

        private enum Stage { Rocket, TwoRocket, Explosion };

        #region Private members

        private const Single MAIN_ROCKET_TIME = 1.5f;
        private const Single SUB_ROCKET_TIME = 1.5f;
        private const Single EXPLOSION_TIME = 0.8f;

        private Stage fStage;
        private Particle fMainRocket;
        private Particle[] fSubRockets;

        //private List<Particle> fFlares = new List<Particle>();
        //private List<Particle> fFlareRemoveList = new List<Particle>();
        private Vector2 fStartPosition;
        private Vector2 fStartVelocity;
        private DateTime fStartTime;
        private DateTime fStageTime;

        private bool fHasStarted;
        private bool fHasEnded;
        //private bool fNoMoreFlares;

        #endregion

        #region Static members

        private static Texture2D fRocketTexture;
        private static Texture2D fSubRocketTexture;
        private static Texture2D fExplosionTexture;
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
            fMainRocket = new Particle(fStartPosition, fStartVelocity, fRocketTexture);
            fStage = Stage.Rocket;
            fStartTime = DateTime.Now;
            fStageTime = DateTime.Now;

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

                switch (fStage)
                {
                    case Stage.Rocket:
                        fMainRocket.Move(t);
                        if ((DateTime.Now - fStageTime).TotalSeconds > MAIN_ROCKET_TIME)
                        {
                            fStage = Stage.TwoRocket;
                            fStageTime = DateTime.Now;
                            Vector2 p1 = new Vector2(fMainRocket.Position.X + 16, fMainRocket.Position.Y);
                            Vector2 p2 = new Vector2(fMainRocket.Position.X + 16, fMainRocket.Position.Y);
                            Vector2 v1 = new Vector2(fMainRocket.Velocity.X - 10.0f, fMainRocket.Velocity.Y);
                            Vector2 v2 = new Vector2(fMainRocket.Velocity.X + 10.0f, fMainRocket.Velocity.Y);
                            fSubRockets = new Particle[2];
                            fSubRockets[0] = new Particle(p1, v1, fSubRocketTexture);
                            fSubRockets[1] = new Particle(p2, v2, fSubRocketTexture);

                        }
                        if (fMainRocket.Position.Y > fStartPosition.Y)
                        {
                            fHasEnded = true;
                        }

                        break;
                    case Stage.TwoRocket:
                        fSubRockets[0].Move(t);
                        fSubRockets[1].Move(t);
                        if ((DateTime.Now - fStageTime).TotalSeconds > SUB_ROCKET_TIME)
                        {
                            fStage = Stage.Explosion;
                            fStageTime = DateTime.Now;
                            fSubRockets[0].Position.X += 16;
                            fSubRockets[1].Position.X += 16;
                            fSubRockets[0].Texture = fExplosionTexture;
                            fSubRockets[1].Texture = fExplosionTexture;
                        }
                        break;
                    case Stage.Explosion:
                        fSubRockets[0].Move(t);
                        fSubRockets[1].Move(t);
                        if ((DateTime.Now - fStageTime).TotalSeconds > EXPLOSION_TIME)
                        {
                            fHasEnded = true;
                        }
                        break;
                }
            }
        }

        void IFirework.Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (fHasStarted && !fHasEnded)
            {
                switch (fStage)
                {
                    case Stage.Rocket:
                        fSpriteBatch.Draw(fMainRocket.Texture, fMainRocket.Position, Color.White);
                        break;
                    case Stage.TwoRocket:
                        fSpriteBatch.Draw(fSubRockets[0].Texture, fSubRockets[0].Position, Color.White);
                        fSpriteBatch.Draw(fSubRockets[1].Texture, fSubRockets[1].Position, Color.White);
                        break;
                    case Stage.Explosion:
                        float aliveFactor = Convert.ToSingle((DateTime.Now - fStageTime).TotalSeconds / EXPLOSION_TIME);
                        if (aliveFactor > 1)
                            aliveFactor = 1;

                        //aliveFactor = 0.1f;

                        float scale = 4 * aliveFactor; // scale 0-4, size 0-1024 px
                        float halfSize = 512 * aliveFactor;
                        
                        byte alfa = Convert.ToByte(255 - 255 * aliveFactor);
                        Color color = Color.White;
                        color.A = alfa;

                        Vector2 p1 = new Vector2(
                            fSubRockets[0].Position.X - halfSize,
                            fSubRockets[0].Position.Y - halfSize);
                        Vector2 p2 = new Vector2(
                            fSubRockets[1].Position.X - halfSize,
                            fSubRockets[1].Position.Y - halfSize);
                        fSpriteBatch.Draw(fSubRockets[0].Texture, p1, null, color, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
                        fSpriteBatch.Draw(fSubRockets[1].Texture, p2, null, color, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
                        //fSpriteBatch.Draw(fSubRockets[0].Texture, fSubRockets[0].Position, color, );
                        //fSpriteBatch.Draw(fSubRockets[1].Texture, fSubRockets[1].Position, color);
                        break;
                }
                
            }
        }

        #endregion

        #region IFireEngine Members

        IFirework IFireEngine.CreateFirework()
        {
            return new KarinRaketen();
        }

        void IFireEngine.LoadContent(Game game, SpriteBatch spriteBatch)
        {
            KarinRaketen.fSpriteBatch = spriteBatch;
            KarinRaketen.fRocketTexture = game.Content.Load<Texture2D>(@"Fireworks\KarinRaketen\blue64");
            KarinRaketen.fSubRocketTexture = game.Content.Load<Texture2D>(@"Fireworks\KarinRaketen\red32");
            KarinRaketen.fExplosionTexture = game.Content.Load<Texture2D>(@"Fireworks\KarinRaketen\circle256");
        }

        #endregion
    }
}
