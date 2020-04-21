using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGUISampleShared;
using MonoGUI.GameComponents;
using MonoGUI.Engine;
using MonoGUI.Controls;
using System.Collections.Generic;

namespace MonoGUISampleWindows
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {

        #region Private memers

        private GuiEngine fEngine;

        private GraphicsDeviceManager fGraphics;
        private SpriteBatch fSpriteBatch;
       
        private GuiEngineOptions fOptions = new GuiEngineOptions()
        {
            PreferredSize = new Rectangle(0, 0, 1024, 768)
        };

        private GuiWindow fWindow1;
        private GuiWindow fWindow2;
        private GuiWindow fWindow3;

        #endregion

        #region Constructor

        public Game1()
        {
            fGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

        }

        #endregion

        #region Game
                
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            fEngine = new GuiEngine(this, fOptions, fGraphics);
            //fEngine.Theme = new MonoGUI.Themes.TestTheme(); 

            fWindow1 = new MainWindow(fEngine);
            fWindow2 = new MainWindow2(fEngine);
            fWindow3 = new MainWindow3(fEngine);

            fEngine.AddWindow(fWindow1);
            fEngine.AddWindow(fWindow2);
            fEngine.AddWindow(fWindow3);          

            this.Components.Add(fEngine);

            base.Initialize();
        }        

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            fSpriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            IsMouseVisible = true;
            IsFixedTimeStep = false;
            this.Window.AllowAltF4 = true;
            this.Window.AllowUserResizing = true;
            fGraphics.PreferMultiSampling = false;
            fGraphics.SynchronizeWithVerticalRetrace = false;
            fGraphics.HardwareModeSwitch = true;
            fGraphics.ApplyChanges();
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (fEngine.InputManager.KeyPressed(Keys.P))
                fEngine.UpdatePaused = !fEngine.UpdatePaused;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }

        #endregion

        #region Private memers

        #endregion

    }
}
