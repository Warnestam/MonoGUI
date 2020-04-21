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
 * File:		Star80Engine
 * Purpose:		2D stars
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2016-04-25  RW	
 * 
 * History:		2016-04-26  RW  Created
 *              2020-04-08  RW  Moved to MonoExperience
 * 
 */
namespace MonoExperience
{

    /// <summary>
    /// Simple 2D game engine
    /// </summary>
    public class Star80Engine : BaseEngine
    {

        #region Classes

        class Star
        {
            public Vector2 Position { get; set; }

            public Vector2 Velocity { get; set; }

            public Color Color { get; set; }
            
            public float Scale { get; set; }
        }

        enum MyMode { FixedStars, ScaledStars};

        #endregion

        #region Constant


        #endregion

        #region Private members

        private SpriteBatch fSpriteBatch;
        private Random fRandom = new Random();

        private Matrix fWorldMatrix;
        private Matrix fViewMatrix;
        private Matrix fProjectionMatrix;

        private Matrix fTargetWorldMatrix;
        private Matrix fTargetViewMatrix;
        private Matrix fTargetProjectionMatrix;

        private RenderTarget2D fWorkTarget;

        private Rectangle fScreenRectangle;
        private int fScreenWidth;
        private int fScreenHeight;

        private Texture2D fStarTexture;
        
        private List<Star> fStars;
        private TimeSpan fStarTime = TimeSpan.MinValue;
        private TimeSpan fKeyTime = TimeSpan.MinValue;

        private int fStarsPerSecond = 100;

        private bool fHalted;

        private MyMode fMode = MyMode.FixedStars;

        #endregion

        #region Constructor

        /// <summary>
        /// Create the engine
        /// </summary>
        /// <param name="game"></param>
        public Star80Engine(EngineContainer cnt) : base(cnt)
        {
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

            InitApp();
        }

        protected override void Dispose(bool disposing)
        {
            //
            base.Dispose(disposing);
        }




        /// <summary>
        /// Loads any component specific content
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            fSpriteBatch = new SpriteBatch(Game.GraphicsDevice);


            fStarTexture = Game.Content.Load<Texture2D>("Stars/star");

            UpdateScale();

        }

        private void UpdateScale()
        {
            InitializeTransform();

            fScreenWidth = GraphicsDevice.Viewport.Width;
            fScreenHeight = GraphicsDevice.Viewport.Height;
            fScreenRectangle = new Rectangle(0, 0, fScreenWidth, fScreenHeight);
            fWorkTarget = new RenderTarget2D(GraphicsDevice, fScreenWidth, fScreenHeight, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None,
                0,
                RenderTargetUsage.PreserveContents);

        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            HandleInput(gameTime);
            base.Update(gameTime);

            if (!fHalted)
            {
                UpdateStars(gameTime);
            }


        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            RenderStars();       

            base.Draw(gameTime);
        }

        #endregion

        #region BaseEngine

        public override string GetName()
        {
            return "Star 80 Engine";
        }

        public override string GetHelp()
        {
            return @"
A - Increase star rate
Z - Decrease star rate
M - Change mode
H - Halt";
        }

        public override string GetInfo()
        {
            return String.Format("Stars: {0}\nStars/s: {1}\nMode: {2}", fStars.Count, fStarsPerSecond,fMode);
        }

        public override string GetAbout()
        {
            return @"Render a 2D view of stars";
        }

        private void HandleInput(GameTime gameTime)
        {
            //HandleMouse(gameTime, this.Manager.GetMouseState());

            if (!fHalted)
            {
                if (fKeyTime == TimeSpan.MinValue || (gameTime.TotalGameTime - fKeyTime).TotalMilliseconds > 10.0f)
                {
                    fKeyTime = gameTime.TotalGameTime;
                    
                    if (this.Manager.IsKeyDown(Keys.A))
                    {
                        fStarsPerSecond += 10;
                    }
                    else if (this.Manager.IsKeyDown(Keys.Z))
                    {
                        fStarsPerSecond -= 10;
                        if (fStarsPerSecond < 0)
                            fStarsPerSecond = 0;
                    }
                }
            }
            if (this.Manager.KeyPressed(Keys.H))
            {
                fHalted = !fHalted;
            }
            else if (this.Manager.KeyPressed(Keys.M))
            {
                switch(fMode)
                {
                    case MyMode.FixedStars: fMode = MyMode.ScaledStars; break;
                    case MyMode.ScaledStars: fMode = MyMode.FixedStars; break;
                }
            }

        }

        public override void DisplayChanged()
        {
            UpdateScale();
        }

        #endregion

        #region Private methods: INIT

        /// <summary>
        /// Initializes the transforms used by the game.
        /// </summary>
        private void InitializeTransform()
        {

            fWorldMatrix = Matrix.Identity;

            fViewMatrix = Matrix.CreateLookAt(
               new Vector3(0.0f, 0.0f, 1.0f),
               Vector3.Zero,
               Vector3.Up
               );

            fProjectionMatrix = Matrix.CreateOrthographicOffCenter(
                0,
                (float)GraphicsDevice.Viewport.Width,
                (float)GraphicsDevice.Viewport.Height,
                0,
                1.0f, 1000.0f);

            fTargetWorldMatrix = fWorldMatrix;
            fTargetViewMatrix = fViewMatrix;
            fTargetProjectionMatrix = fProjectionMatrix;

        }

        /// <summary>
        /// Initialize the app
        /// </summary>
        private void InitApp()
        {
            this.BackColor = Color.Black;

            InitializeTransform();
            fStars = new List<Star>();

            fHalted = false;

        }


        #endregion

        #region Private methods: MOVE

        private void UpdateStars(GameTime gameTime)
        {
            List<Star> removeList = new List<Star>();
            foreach (Star star in fStars)
            {
                //double m2 = gameTime.ElapsedGameTime.TotalMilliseconds * gameTime.ElapsedGameTime.TotalMilliseconds;

                star.Position = new Vector2(
                        (float)(star.Position.X + gameTime.ElapsedGameTime.TotalMilliseconds * star.Velocity.X),
                        (float)(star.Position.Y + gameTime.ElapsedGameTime.TotalMilliseconds * star.Velocity.Y));
                star.Velocity = new Vector2(
                    (float)(star.Velocity.X * (1.000f + gameTime.ElapsedGameTime.TotalSeconds )),
                    (float)(star.Velocity.Y * (1.000f + gameTime.ElapsedGameTime.TotalSeconds )));
                star.Scale = (float)(star.Scale * (1.0f + gameTime.ElapsedGameTime.TotalSeconds*0.3f));
                 
                if ((star.Position.X < -50) ||
                    (star.Position.X >= (fScreenWidth + 2 * 50)) ||
                    (star.Position.Y < -50) ||
                    (star.Position.Y >= (fScreenHeight + 2 * 50)))
                {
                    removeList.Add(star);
                }
            }
            foreach (Star star in removeList)
            {
                fStars.Remove(star);
            }
            TryAddNewStars(gameTime);
        }

        private void TryAddNewStars(GameTime gameTime)
        {
            //if (fStars.Count >= fMaxStars)
            //    return;

            int count = 0;
            if (fStarTime == TimeSpan.MinValue)
                count = 1;
            else
            {
                double ms = (gameTime.TotalGameTime - fStarTime).TotalMilliseconds;
                count = Convert.ToInt32(fStarsPerSecond * ms / 1000.0f);
            }
            if (count > 0)
            {
                fStarTime = gameTime.TotalGameTime;
                for (int i = 0; i < count; i++)
                {
                    AddNewStars(gameTime);
                }
            }
        }

        private void AddNewStars(GameTime gameTime)
        {
            Star star = new Star();
            const int SIZE = 0;
            const int SIZE2 = SIZE / 2;

            int x = fScreenWidth / 2 + fRandom.Next(SIZE) - SIZE2;
            int y = fScreenHeight / 2 + fRandom.Next(SIZE) - SIZE2;
            int r = fRandom.Next(255);
            int g = fRandom.Next(255);
            int b = fRandom.Next(255);
            double vx = (fRandom.NextDouble() - 0.5f) / 100.0f;
            double vy = (fRandom.NextDouble() - 0.5f) / 100.0f;
            star.Color = new Color(r, g, b);
            star.Position = new Vector2(x, y);
            star.Velocity = new Vector2((float)vx, (float)vy);
            star.Scale = 0.5f;

            fStars.Add(star);
        }

        #endregion

        #region Private methods: DRAW

        private void RenderStars()
        {
            switch(fMode)
            {
                case MyMode.FixedStars:
                    fSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
                    foreach (Star star in fStars)
                    {
                        fSpriteBatch.Draw(fStarTexture, star.Position, star.Color);
                    }
                    fSpriteBatch.End();
                    break;
                case MyMode.ScaledStars:
                    fSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    foreach (Star star in fStars)
                    {
                        fSpriteBatch.Draw(fStarTexture, star.Position, null, star.Color, 0, new Vector2(16, 16), star.Scale, SpriteEffects.None, 0);
                    }
                    fSpriteBatch.End();
                    break;
            }
        }
        
        #endregion

  
    }
}
