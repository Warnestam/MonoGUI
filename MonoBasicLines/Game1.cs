using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGUI.Controls;
using MonoGUI.Engine;
using System.Collections.Generic;
using MonoGUI.Graphics;
using System;

// TODOS

// Label - TextAligned - center i fönster
// Sizeable - dra i border
// Window - SizeToContent
// Label - hoover
// Borde göra refactor ändå på resize och move. Alltid border?

namespace MonoBasicLines
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {

        #region Private members

        private GuiEngine fEngine;

        private GraphicsDeviceManager fGraphics;
        private SpriteBatch fSpriteBatch;

        private GuiWindow fWindow;
        private GuiLabel fCurrentElement;
        private GuiLabel fLabelDisplayMode;
        private GuiLabel fAdapterSize;
        private GuiLabel fWindowSize;
        private GuiLabel fAspectRatio;
        private GuiLabel fFPS;
        private GuiLabel fInfo;

        private Random fRandom = new Random();
        private DynamicPrimitiveLine fLines;
        private Matrix fWorldMatrix;
        private Matrix fViewMatrix;
        private Matrix fProjectionMatrix;
        private BasicEffect fBasicEffect;
        private float fAngle;

        private GuiEngineOptions fOptions = new GuiEngineOptions()
        {
            PreferredSize = new Rectangle(0, 0, 1024, 768)
        };

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
            fEngine.OnResize += FEngine_OnResize;
            fLabelDisplayMode = new GuiLabel();
            fAdapterSize = new GuiLabel() { HorizontalAlignment = GuiHorizontalAlignment.Right, BackgroundColor = Color.Orange };
            fWindowSize = new GuiLabel();
            fAspectRatio = new GuiLabel();
            fFPS = new GuiLabel() { HorizontalAlignment = GuiHorizontalAlignment.Right };
            fInfo = new GuiLabel();
            fCurrentElement = new GuiLabel();

            fWindow = new GuiWindow()
            {
                Dragable = true,
                Clickable = true,
                Title = new GuiDockChild()
                {
                    Dock = GuiDock.Top,
                    Control = new GuiLabel()
                    {
                        Text = "Lines! (Click, Size or Drag)",
                        //VerticalAlignment = GuiVerticalAlignment.Stretch,
                        //HorizontalAlignment = GuiHorizontalAlignment.Stretch,
                        BackgroundColor = Color.Black,
                        ForegroundColor = Color.Yellow,

                    }
                },
                Border = new GuiBorder()
                {
                    Border = new GuiThickness(15),
                    BorderColor = new Color(Color.Black, 0.3f),
                },
                //BackgroundColor = new Color(Color.Red, 0.9f),
                HorizontalAlignment = GuiHorizontalAlignment.Left,
                //VerticalAlignment = GuiVerticalAlignment.Top,
                WindowState = GuiWindowState.Normal,
                X = 100,
                Y = 100,
                Width = 600,
                Height = 500,
        
                Content = new GuiPanel()
                {
                    Margin = new GuiThickness(20),
                    Content = new GuiStackPanel()
                    {
                        Childs = new List<GuiStackChild>()
                        {
                            new GuiStackChild(){Control = fFPS },
                            new GuiStackChild(){Control = fLabelDisplayMode },
                            new GuiStackChild(){Control =fAdapterSize},
                            new GuiStackChild(){Control =fWindowSize},
                            new GuiStackChild(){Control =fAspectRatio},
                            new GuiStackChild(){Control =fInfo },
                            new GuiStackChild(){Control =fCurrentElement }
                        },
                        HorizontalAlignment = GuiHorizontalAlignment.Left,
                        VerticalAlignment = GuiVerticalAlignment.Top,
                        BackgroundColor = new Color(Color.White, 0.2f),
                    },
                    BackgroundColor = new Color(Color.Red, 0.7f),
                }

            };

            fEngine.AddWindow(fWindow);

            if (fWindow.Title != null)
                fWindow.Title.Control.OnClick += WindowTitle_OnClick;

            this.Components.Add(fEngine);

            UpdateScale();

            base.Initialize();
        }

        private void FEngine_OnResize(object sender, EventArgs e)
        {
            UpdateScale();
        }

        private void WindowTitle_OnClick(object sender, System.EventArgs e)
        {

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
            fGraphics.HardwareModeSwitch = false;
            fGraphics.ApplyChanges();

            UpdateScale();

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

            if (fEngine.InputManager.KeyPressed(Keys.M))
                fEngine.ToggleFullscreen();

            fLabelDisplayMode.Text = $"Mode={fEngine.DisplayMode}";
            fAdapterSize.Text = $"Adapter={fEngine.AdapterSize.Width}x{fEngine.AdapterSize.Height}";
            fWindowSize.Text = $"Window={fEngine.WindowSize.Width}x{fEngine.WindowSize.Height}";
            fAspectRatio.Text = $"AspectRatio={fEngine.AspectRatio}";
            fFPS.Text = $"FPS={fEngine.FPS.FrameRate}";
            fInfo.Text = $"Lines={fLines.Lines}";
            if (fEngine.MouseOverElement == null)
            {
                fCurrentElement.Text = "...";
            }
            else
            {
                GuiElement element = fEngine.MouseOverElement;
                string name = String.Empty;
                bool isFirst = true;
                
                while (element != null)
                {
                    if (!isFirst)
                        name = name + "/";
                    isFirst = false;
                    string n = element.GetType().Name;
                    if (n.StartsWith("Gui"))
                        name = name + n.Substring(3);
                    else
                        name = name + n;
                    element = element.Parent;
                }
                fCurrentElement.Text = name;
            }

            //fAngle = Convert.ToSingle(PI2 * gameTime.TotalGameTime.TotalSeconds * 0.1f);
            fAngle += Convert.ToSingle(gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

            base.Update(gameTime);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            RasterizerState rState = new RasterizerState();
            rState.ScissorTestEnable = false;
            fSpriteBatch.GraphicsDevice.RasterizerState = rState;

            GraphicsDevice.Clear(Color.Black);

            fWorldMatrix = Matrix.CreateRotationZ(fAngle) *
                Matrix.CreateTranslation(
                GraphicsDevice.Viewport.Width / 2f,
                GraphicsDevice.Viewport.Height / 2f, 0);
            fBasicEffect.World = fWorldMatrix;
            fLines.UseVertexBuffer = true;
            foreach (EffectPass pass in fBasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                fLines.Render();
            }

            base.Draw(gameTime);

        }

        #endregion



        private void UpdateScale()
        {
            InitializeTransform();
            InitializeEffect();
            if (fSpriteBatch == null)
                return;

            int maxLines = 1000000;
            fLines = new DynamicPrimitiveLine(fSpriteBatch.GraphicsDevice);

            CircleData circleTo = CreateRandomCircle();
            CircleData circleFrom = null;
            int currentStep = 0;
            int maxSteps = 0;

            while (fLines.Lines < maxLines)
            {
                CircleData currentCircle;
                currentStep++;
                if (currentStep >= maxSteps)
                {
                    circleFrom = circleTo;
                    circleTo = CreateRandomCircle();
                    currentStep = 0;
                    maxSteps = fRandom.Next(200) + 1;
                    currentCircle = circleFrom;
                }
                else
                {
                    float f = Convert.ToSingle(currentStep) / maxSteps;
                    currentCircle = CalculateAverageCircle(circleFrom, circleTo, f);
                }
                Color c1 = new Color(currentCircle.R1, currentCircle.G1, currentCircle.B1, (byte)255);
                Color c2 = new Color(currentCircle.R2, currentCircle.G2, currentCircle.B2, (byte)255);
                fLines.AddCircle(currentCircle.X, currentCircle.Y, 0,
                    currentCircle.Radius, currentCircle.Radius,
                    currentCircle.Segments, c1, c2);
            }
        }

        /// <summary>
        /// Initializes the transforms used by the game.
        /// </summary>
        private void InitializeTransform()
        {

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
        }

        /// <summary>
        /// Initializes the effect (loading, parameter setting, and technique selection)
        /// used by the game.
        /// </summary>
        private void InitializeEffect()
        {
            fBasicEffect = new BasicEffect(GraphicsDevice);
            fBasicEffect.VertexColorEnabled = true;

            fBasicEffect.View = fViewMatrix;
            fBasicEffect.Projection = fProjectionMatrix;
        }

        public CircleData CalculateAverageCircle(CircleData cFrom, CircleData cTo, float factor)
        {
            return new CircleData()
            {
                X = cFrom.X + (cTo.X - cFrom.X) * factor,
                Y = cFrom.Y + (cTo.Y - cFrom.Y) * factor,
                Radius = Convert.ToInt32(cFrom.Radius + (cTo.Radius - cFrom.Radius) * factor),
                Segments = Convert.ToInt32(cFrom.Segments + (cTo.Segments - cFrom.Segments) * factor),
                R1 = Convert.ToByte(cFrom.R1 + (cTo.R1 - cFrom.R1) * factor),
                G1 = Convert.ToByte(cFrom.G1 + (cTo.G1 - cFrom.G1) * factor),
                B1 = Convert.ToByte(cFrom.B1 + (cTo.B1 - cFrom.B1) * factor),
                R2 = Convert.ToByte(cFrom.R2 + (cTo.R2 - cFrom.R2) * factor),
                G2 = Convert.ToByte(cFrom.G2 + (cTo.G2 - cFrom.G2) * factor),
                B2 = Convert.ToByte(cFrom.B2 + (cTo.B2 - cFrom.B2) * factor),
            };
        }

        public CircleData CreateRandomCircle()
        {
            return new CircleData()
            {
                X = (float)(1500.0f * (fRandom.NextDouble() - 0.5f)),
                Y = (float)(1500.0f * (fRandom.NextDouble() - 0.5f)),
                Radius = fRandom.Next(150) + 10,
                Segments = fRandom.Next(100) + 10,
                R1 = (byte)fRandom.Next(256),
                G1 = (byte)fRandom.Next(256),
                B1 = (byte)fRandom.Next(256),
                R2 = (byte)fRandom.Next(256),
                G2 = (byte)fRandom.Next(256),
                B2 = (byte)fRandom.Next(256),
            };
        }

        public class CircleData
        {
            public float X { get; set; }
            public float Y { get; set; }
            public byte R1 { get; set; }
            public byte G1 { get; set; }
            public byte B1 { get; set; }
            public byte R2 { get; set; }
            public byte G2 { get; set; }
            public byte B2 { get; set; }
            public int Radius { get; set; }
            public int Segments { get; set; }

        }


    }
}
