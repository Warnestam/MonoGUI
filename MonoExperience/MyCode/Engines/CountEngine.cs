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
using MonoExperience.Engines.CountEngine;
using TexturePackerLoader;


/*
 * File:		CountEngine
 * Purpose:		Counter with some kind of particles
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2020-04-16  RW	
 * 
 * History:		2020-04-16  RW  Created
 * 
 */
namespace MonoExperience
{

    /// <summary>
    /// Counter with some kind of particles
    /// </summary>
    public class CountEngine : BaseEngine
    {

        #region Private members
        private readonly DateTime COUNTDOWN = new DateTime(2021, 1, 1, 00, 00, 00);

        SideEffect fParticlesSideEffect;
        MyParticles fParticlesMouse;
        DigitEffect fParticlesDigits;
        SecondEffect fParticlesSeconds;
        DownCounterEffect fParticlesCounter;
        EvaEffect fParticlesEva;

        private int fEffectParticles = 100;
        private int fMouseParticles = 50;
        private int fMouseIndex = 0;

        SpriteRender fSpriteRender;
        SpriteSheet fSpriteSheet;
        private Texture2D fTexture;

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

        private bool fHalted;

        #endregion

        #region Constructor

        /// <summary>
        /// Create the engine
        /// </summary>
        public CountEngine(EngineContainer cnt) : base(cnt)
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

            fTexture = Game.Content.Load<Texture2D>("count/particle_32_2");                  
                       

            var spriteSheetLoader = new SpriteSheetLoader(Game.Content, GraphicsDevice);
            fSpriteSheet = spriteSheetLoader.Load("count/flowers.png");
            fSpriteRender = new SpriteRender(fSpriteBatch);

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


         
            fParticlesSideEffect = new SideEffect(fScreenWidth, fScreenHeight, fEffectParticles);
            fParticlesMouse = new MyParticles(fScreenWidth, fScreenHeight, fMouseParticles);
            fParticlesDigits = new DigitEffect(fScreenWidth, fScreenHeight, 50);
            fParticlesSeconds = new SecondEffect(fScreenWidth, fScreenHeight, 80);
            fParticlesCounter = new DownCounterEffect(COUNTDOWN, fScreenWidth, fScreenHeight, 50);
            fParticlesEva = new EvaEffect(fScreenWidth, fScreenHeight, 150);
            fParticlesSideEffect.Particles.ParticleColor = Microsoft.Xna.Framework.Color.Yellow;
            fParticlesMouse.ParticleColor = Microsoft.Xna.Framework.Color.Red;
            fParticlesDigits.Particles.ParticleColor = Microsoft.Xna.Framework.Color.Green;
            fParticlesSeconds.Particles1.ParticleColor = Microsoft.Xna.Framework.Color.Cyan;
            fParticlesSeconds.Particles2.ParticleColor = Microsoft.Xna.Framework.Color.Cyan;
            fParticlesEva.Particles[0].ParticleColor = Microsoft.Xna.Framework.Color.LightSkyBlue;
            fParticlesEva.Particles[1].ParticleColor = Microsoft.Xna.Framework.Color.LightCoral;
            fParticlesEva.Particles[2].ParticleColor = Microsoft.Xna.Framework.Color.LightGreen;
            fParticlesEva.Particles[3].ParticleColor = Microsoft.Xna.Framework.Color.LightPink;
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
                fParticlesSideEffect.Update(gameTime);
                fParticlesMouse.Update(gameTime);
                fParticlesDigits.Update(gameTime);
                fParticlesSeconds.Update(gameTime);
                fParticlesCounter.Update(gameTime);
                fParticlesEva.Update(gameTime);
            }


        }

        /// <summary>
        /// Allows the game component to draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.Black);

            //fSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            //fSpriteBatch.Draw(fBackground, new Rectangle(0, 0, fGraphics.PreferredBackBufferWidth, fGraphics.PreferredBackBufferHeight),Color.FromNonPremultiplied(255, 255, 255, 160));
            //fSpriteBatch.End();

            fParticlesSideEffect.Draw(fSpriteBatch, fTexture);
            fParticlesMouse.Draw(fSpriteBatch, fTexture);
            fParticlesSeconds.Draw(fSpriteBatch, fTexture);
            fParticlesCounter.Draw(fSpriteBatch, fTexture);
            fParticlesEva.Draw(fSpriteBatch, fTexture);

            fSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

            fSpriteRender.Draw(
                fSpriteSheet.Sprite(TexturePackerMonoGameDefinitions.Flowers.Flowers_10), new Vector2(Mouse.GetState().X, Mouse.GetState().Y));
            fSpriteBatch.End();


            base.Draw(gameTime);
        }

        #endregion

        #region BaseEngine

        public override string GetName()
        {
            return "Count Engine";
        }

        public override string GetHelp()
        {
            return @"
H - Halt";
        }

        public override string GetInfo()
        {
            return String.Format("...");
        }

        public override string GetAbout()
        {
            return @"Render a counter with particles";
        }

        private void HandleInput(GameTime gameTime)
        {
            if (this.Manager.KeyPressed(Keys.H))
            {
                fHalted = !fHalted;
            }

            MouseState state = this.Manager.GetMouseState();

            fParticlesMouse.SetDestination(fMouseIndex, new Vector2(state.X, state.Y));
            fMouseIndex++;
            if (fMouseIndex >= fMouseParticles)
                fMouseIndex = 0;

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
            
            fHalted = false;

        }


        #endregion

        #region Private methods: MOVE


           
        #endregion

        #region Private methods: DRAW

  
        #endregion

  
    }
}
