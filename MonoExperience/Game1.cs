using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGUI.Controls;
using MonoGUI.Engine;
using System.Collections.Generic;
using MonoGUI.Graphics;
using System;
using MonoGUI.GameComponents;

// TODOS

// Label - TextAligned - center i fönster
// Sizeable - dra i border
// Window - SizeToContent
// Label - hoover
// Borde göra refactor ändå på resize och move. Alltid border?

namespace MonoExperience
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {


        #region Private members

        // XNA
        private GraphicsDeviceManager fGraphics;
        private SpriteBatch fSpriteBatch;

        // Engines
        private EngineContainer fContainer;
        private List<BaseEngine> fEngines;
        private int fCurrentEngineIndex = -1;

        private FpsEngine fFpsEngine;
        private InputManager fInputManager;

        // MonoGUI
        private GuiEngine fEngine;
        private GuiWindow fWindow1;
        private GuiWindow fWindow2;
        private GuiExpandablePanel fExPanel;

        private GuiEngineOptions fOptions = new GuiEngineOptions()
        {
            PreferredSize = new Rectangle(0, 0, 800, 640)
        };

        #endregion

        #region Constructor

        public Game1()
        {
            this.Window.Title = "MonoGUI Experience";

            fGraphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            fGraphics.HardwareModeSwitch = false;

            IsMouseVisible = true;
            IsFixedTimeStep = false;

            this.Window.AllowAltF4 = true;
            this.Window.AllowUserResizing = true;
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
            fEngine.OnResize += FEngine_OnResize;

            fFpsEngine = new FpsEngine(this);

            fInputManager = fEngine.InputManager;// new InputManager(this);

            fWindow1 = new MainWindow(fEngine);
            //fWindow2 = new GuiWindow()
            //{
            //    BackgroundColor = Color.Yellow,
            //    Content = new GuiExpandablePanel()
            //    {
            //        Content = new GuiLabel() { Text = "Now you see me" },
            //        PanelState = GuiExpandablePanelState.Expanded,
            //        Title = new GuiDockChild()
            //        {
            //            Dock = GuiDock.Top,
            //            Control = new GuiLabel() { Text = "Click to expand" }
            //        },
            //    }
            //};

            GuiStackPanel menuPanel = new GuiStackPanel() { BackgroundColor = new Color(Color.DarkSlateBlue, 0.7f) };
            fExPanel = new GuiExpandablePanel()
            {
                Content = menuPanel,
                PanelState = GuiExpandablePanelState.Collapsed,
                Title = new GuiDockChild()
                {
                    Dock = GuiDock.Top,
                    Control = new GuiLabel() { Text = "Click", BackgroundColor = new Color(Color.Black, 0.7f), ForegroundColor = Color.White },
                },
            };

            fWindow2 = new GuiWindow()
            {
                Title = new GuiDockChild()
                {
                    Dock = GuiDock.Bottom,
                    Control = fExPanel,
                },
                Content = new GuiPanel() { },  // Fill upp window
                //Content = new GuiPanel() { Width=0, Height=0},
                Border = new GuiBorder() { Border=new GuiThickness(1), BorderColor=Color.Red}
            };

            fEngine.AddWindow(fWindow1);
            fEngine.AddWindow(fWindow2);

            //var w = new BasicLines1EngineControlWindow(fEngine);
            //fEngine.AddWindow(w);
            //fEngine.AddWindow(new MainWindow(fEngine));

            this.Components.Add(fEngine);
            this.Components.Add(fFpsEngine);
            //this.Components.Add(fInputManager);

            fContainer = new EngineContainer(this, fGraphics, fInputManager, fEngine, fFpsEngine);
            fContainer.BackColor = Color.Transparent;

            fEngines = EngineFactory.GetEngines(fContainer);

            foreach (var engine in fEngines)
                engine.DoInitialize();

            var index = 0;
            foreach (var engine in fEngines)
            {
                var label = new GuiLabel()
                {
                    Text = engine.GetName(),
                    BackgroundColor = new Color(Color.Gray, 0.5f),
                    ForegroundColor = Color.White,
                };
                label.OnClick += Label_OnClick;
                label.Tag = index++;
                menuPanel.Childs.Add(new GuiStackChild() { Control = label });
            }

            base.Initialize();
        }

        private void Label_OnClick(object sender, EventArgs e)
        {
            fCurrentEngineIndex = (int)(sender as GuiElement).Tag;
            SetEngine();
            fExPanel.PanelState = GuiExpandablePanelState.Collapsed;
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

            fGraphics.PreferMultiSampling = false;
            fGraphics.SynchronizeWithVerticalRetrace = false;
            fGraphics.HardwareModeSwitch = false;
            fGraphics.ApplyChanges();


            fInputManager.InitMouse();
            fInputManager.CaptureMouse = false;


            //Window.ClientSizeChanged += (s, e) => UpdateScale();

            fContainer.LoadContent();
            foreach (var engine in fEngines)
                engine.DoLoadContent();

            base.LoadContent();
        }


        protected override void UnloadContent()
        {
            fContainer.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            fContainer.Update(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (fEngine.InputManager.KeyPressed(Keys.P))
                fEngine.UpdatePaused = !fEngine.UpdatePaused;

            //if (fEngine.InputManager.KeyPressed(Keys.M))
            //    fEngine.ToggleFullscreen();

            if (fCurrentEngineIndex < 0)
            {
                fCurrentEngineIndex = 0;
                SetEngine();
            }

            HandleInput(gameTime);


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {            
            fContainer.Draw(gameTime);

            fContainer.ApplyViewPort(ViewPortType.Window);
            base.Draw(gameTime);
        }

        #endregion

        #region Private methods

        private void FEngine_OnResize(object sender, EventArgs e)
        {
            fContainer.UpdateScale();
        }


        #endregion

        #region XNA Experience

        /// <summary>
        /// Handle own and engine input
        /// </summary>
        /// <param name="gameTime"></param>
        private void HandleInput(GameTime gameTime)
        {
            if (!this.IsActive)
                return;

            if (fInputManager.KeyPressed(Keys.Escape))
                Exit();
            else if (fInputManager.KeyPressed(Keys.F1))
            {
                fContainer.ShowInfo = !fContainer.ShowInfo;
            }
            else if (fInputManager.IsKeyDown(Keys.LeftAlt) && fInputManager.KeyPressed(Keys.Enter))
            {
                fEngine.ToggleFullscreen();
            }
            else if (fInputManager.KeyPressed(Keys.F3))
            {
                fCurrentEngineIndex--;
                if (fCurrentEngineIndex < 0)
                    fCurrentEngineIndex = fEngines.Count - 1;
                SetEngine();
            }
            else if (fInputManager.KeyPressed(Keys.F4))
            {
                fCurrentEngineIndex++;
                if (fCurrentEngineIndex >= fEngines.Count)
                    fCurrentEngineIndex = 0;
                SetEngine();
            }
            else if (fInputManager.KeyPressed(Keys.F5))
            {
                fContainer.ClearScreen = !fContainer.ClearScreen;
            }
            else if (fInputManager.KeyPressed(Keys.F6))
            {
                fInputManager.CaptureMouse = !fInputManager.CaptureMouse;
            }
        }



        /// <summary>
        /// Select a new engine
        /// </summary>
        private void SetEngine()
        {
            if (fCurrentEngineIndex < 0 && fCurrentEngineIndex >= fEngines.Count)
                return;

            var engine = fEngines[fCurrentEngineIndex];
            fContainer.CurrentEngine = engine;
            fContainer.UpdateScale();
            //if (fCurrentEngine != null)
            //{
            //    this.Components.Remove(fCurrentEngine);
            //    fCurrentEngine = null;
            //}
            //fCurrentEngine = fEngines[fCurrentEngineIndex];

            //this.Components.Add(fCurrentEngine);
        }


        #endregion


    }




}
