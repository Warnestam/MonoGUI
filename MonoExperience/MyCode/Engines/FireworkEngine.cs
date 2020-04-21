using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using System.Threading;
using MonoExperience.Fireworks;

/*
 * File:		FireworkEngine
 * Purpose:		A fireworks engine
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2009-05-24  RW	
 * 
 * History:		2009-05-24  RW  Created
 *              2010-06-21  RW  Converted to XNA 4.0
 *              2010-07-xx  RW  using DynamicPrimitiveLines instead of PrimitiveLineExtra
 *              2010-+7-15  RW  Halt and AddFireworks added
 *              2020-04-01  RW  Moved to MonoExperience
 */


namespace MonoExperience
{
    /// <summary>
    /// A fireworks engine
    /// </summary>
    public class FireworkEngine : BaseEngine
    {

        #region Constants

        private const double GRAVITY = 500.0f;

        #endregion
        
        #region Private members

        private SpriteBatch fSpriteBatch;
        private List<IFirework> fFireworks = new List<IFirework>();
        private List<IFirework> fRemoveList = new List<IFirework>();
        private Random fRandom = new Random();
        private bool fHalted = false;

        private IFireEngine[] fFireEngines = 
        {
            new StandardRocket(),
            new StandardRocket2(),
            new StandardRocket3(),
            new BurningRocket(),
            new KarinRaketen()
        };

        #endregion

        #region Constructor

        public FireworkEngine(EngineContainer cnt) : base(cnt)
        {
            // TODO: Construct any child components here
        }

        #endregion

        #region Public methods

        public void AddFireworks()
        {
            Vector2 position = new Vector2(
                fSpriteBatch.GraphicsDevice.Viewport.Width / 2,
                fSpriteBatch.GraphicsDevice.Viewport.Height);
            Vector2 velocity = new Vector2(
                Convert.ToSingle((fRandom.NextDouble()-0.5f) * 40.0f), 
                Convert.ToSingle(-50.0 - 30 * fRandom.NextDouble()));
            
            IFireEngine engine = fFireEngines[fRandom.Next(fFireEngines.Length)];
            
            IFirework firework = engine.CreateFirework();
            firework.SetStartPosition(position, velocity);
            fFireworks.Add(firework);
        }

        public void AddFireworks(int count)
        {
            int deltaX = fSpriteBatch.GraphicsDevice.Viewport.Width / (count + 1);
            Vector2 velocity = new Vector2(
                Convert.ToSingle((fRandom.NextDouble() - 0.5f) * 40.0f),
                Convert.ToSingle(-50.0 - 30 * fRandom.NextDouble()));
            int x = 0;
            int engineIndex = fRandom.Next(fFireEngines.Length);
            for (int i = 0; i < count; i++)
            {
                x += deltaX;

                Vector2 position = new Vector2(
                    x,
                    fSpriteBatch.GraphicsDevice.Viewport.Height);

                IFireEngine engine = fFireEngines[engineIndex];
                IFirework firework = engine.CreateFirework();
                firework.SetStartPosition(position, velocity);
                fFireworks.Add(firework);
            }

           
        }

        #endregion

        #region DrawableGameComponent

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        
        /// <summary>
        /// Loads any component specific content
        /// </summary>
        protected override void LoadContent()
        {
            fSpriteBatch = new SpriteBatch(Game.GraphicsDevice);
           
            foreach (IFireEngine engine in fFireEngines)
                engine.LoadContent(Game, fSpriteBatch);

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (this.Manager.KeyPressed(Keys.H))
            {
                fHalted = !fHalted;
            }
            else if (this.Manager.IsKeyDown(Keys.A))
            {
                AddFireworks();
            }

            if (!fHalted)
            {
                int freq = 30;
                int rand = fRandom.Next(freq);
                if (rand == 0)
                {
                    int type = fRandom.Next(20);
                    if (type == 0)
                    {
                        AddFireworks(fRandom.Next(50));
                    }
                    else
                    {
                        AddFireworks();
                    }
                }
            }

            foreach (IFirework firework in fFireworks)
            {
                if (firework.HasStarted())
                {
                    firework.Update(gameTime);
                    if (firework.HasEnded())
                    {
                        fRemoveList.Add(firework);
                    }
                }
                else
                {
                    firework.IgniteFirework();
                }
            }
            if (fRemoveList.Count > 0)
            {
                foreach (IFirework firework in fRemoveList)
                {
                    fFireworks.Remove(firework);
                }
                fRemoveList.Clear();
            }

            base.Update(gameTime);

        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            fSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            foreach (IFirework firework in fFireworks)
            {
                firework.Draw(gameTime);
            }
            fSpriteBatch.End();
            base.Draw(gameTime);
        }

        #endregion

        #region BaseEngine

        public override string GetName()
        {
            return "Fireworks 1.0";
        }

        public override string GetHelp()
        {
            return "H - Toggle auto fireworks\nA - Add firworks manually";
        }

        public override string GetInfo()
        {
            return String.Format("Engines: {0}\nFireworks: {1}",
               fFireEngines.Length, fFireworks.Count);
        }

        public override string GetAbout()
        {
            return @"Creating some fireworks";
        }


        public override void DisplayChanged()
        {
           

        }

        #endregion

    }
}