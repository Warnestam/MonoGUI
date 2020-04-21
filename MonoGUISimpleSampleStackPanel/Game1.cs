using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGUI.GameComponents;
using MonoGUI.Engine;
using MonoGUI.Controls;
using System.Collections.Generic;

namespace MonoGUISimpleSampleStackPanel
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

        private List<GuiElement> fContent;
        private int fContentIndex;

        private GuiWindow fWindow1;
        private GuiWindow fWindow2;
        private GuiLabel fContentInformation;

        private GuiPanel fContentPanel;

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
            //fEngine.Theme = new MonoGUI.Themes.TestTheme(); 
            fContent = new List<GuiElement>();
            fContent.Add(new GuiPanel()
            {
                Name = "Panel",
                BackgroundColor = Color.Orange,
                Width = 200,
                Height = 200,
                HorizontalAlignment = GuiHorizontalAlignment.Right,
                Content = new GuiLabel() { Text = "Label in GuiPanel'" }
            });
            fContent.Add(new GuiStackPanel()
            {
                Name = "Stack Panel",
                BackgroundColor = Color.Yellow,
                Width = 200,
                Height = 200,
                HorizontalAlignment = GuiHorizontalAlignment.Right,
                Childs = new List<GuiStackChild>()
                {
                    new GuiStackChild(){Control=new GuiLabel() { Text="Label in GuiStackPanel'"}},
                    new GuiStackChild(){Control=new GuiLabel() { Text="Label in GuiStackPanel'"}}
                }
            });
            fContent.Add(new GuiDockPanel()
            {
                Name = "Dock Panel",
                BackgroundColor = Color.YellowGreen,
                Width = 200,
                Height = 200,
                HorizontalAlignment = GuiHorizontalAlignment.Right,
                Childs = new List<GuiDockChild>()
                {
                    new GuiDockChild(){Control=new GuiLabel() { Text="Bottom in GuiDockChild'"}, Dock=GuiDock.Bottom},
                     new GuiDockChild(){Control=new GuiLabel() { Text="Top in GuiDockChild'"}, Dock=GuiDock.Top}
                }
            });
            fContent.Add(new GuiExpandablePanel()
            {
                Name = "Expandable Panel",
                BackgroundColor = Color.DodgerBlue,
                Width = 200,
                Height = 200,
                HorizontalAlignment = GuiHorizontalAlignment.Right,
                Title = new GuiDockChild() { Dock = GuiDock.Top, Control = new GuiLabel() { Text = "Expand" } },
                Content = new GuiLabel() { Text = "Label in GuiExpandablePanel'" },
                Border = new GuiBorder() { Border = new GuiThickness(10), BorderColor = Color.Red }
            });
            fContent.Add(new GuiBox()
            {
                Name = "Box",
                BackgroundColor = Color.Violet,
                Width = 200,
                Height = 200,
                HorizontalAlignment = GuiHorizontalAlignment.Right,
            });
            fContent.Add(new GuiLabel()
            {
                Name = "Label",
                BackgroundColor = Color.Turquoise,
                Text = "Label",
                Width = 200,
                Height = 200,
                HorizontalAlignment = GuiHorizontalAlignment.Right
            });
            fContent.Add(new GuiBorder()
            {
                Name = "Border",
                BackgroundColor = Color.Transparent,
                Width = 200,
                Height = 200,
                Border = new GuiThickness(2),
                BorderColor = Color.Black,
                HorizontalAlignment = GuiHorizontalAlignment.Right,
                Content = new GuiLabel() { Text = "Label in GuiBorder'" }
            });
            fContentPanel = new GuiPanel();

            fWindow1 = new GuiWindow()
            {
                Title = new GuiDockChild()
                {
                    Dock = GuiDock.Top,
                    Control = new GuiLabel()
                    {
                        Text = "Click to change content"
                    }
                },
                Border = new GuiBorder()
                {
                    Border = new GuiThickness(5),
                    BorderColor = Color.White
                },
                BackgroundColor = new Color(Color.Black, 0.5f),
                Content = fContentPanel
            };
            fContentInformation = new GuiLabel() { BackgroundColor = Color.Black, ForegroundColor = Color.Yellow };
            fWindow2 = new GuiWindow()
            {
                Title = new GuiDockChild()
                {
                    Dock = GuiDock.Top,
                    Control = fContentInformation
                },
                Border = new GuiBorder()
                {
                    Border = new GuiThickness(5),
                    BorderColor = Color.White
                },
                BackgroundColor = new Color(Color.Black, 0.5f),
                //Content = new GuiPanel() { BackgroundColor = Color.Red, Content = new GuiElement() { } } // collapsed
                Content = new GuiPanel() { BackgroundColor = Color.Red, Content = null } // Whole window
            };

            fWindow2.X = 400;
            fWindow2.Y = 200;
            fEngine.AddWindow(fWindow1);
            fEngine.AddWindow(fWindow2);

            fWindow1.Title.Control.OnClick += WindowTitle_OnClick;

            this.Components.Add(fEngine);
            SetContent(fContentIndex);

            foreach (var content in fContent)
                content.Initialize(GraphicsDevice);

            base.Initialize();
        }

        private void SetContent(int index)
        {
            fContentPanel.Content = fContent[index];
            fContentPanel.InvalidateMeasure();
            var info = fContentPanel.Content.GetType();

            fContentInformation.Text = $"Content in other information is of type '{info}'";
        }

        private void WindowTitle_OnClick(object sender, System.EventArgs e)
        {
            fContentIndex++;
            if (fContentIndex >= fContent.Count)
                fContentIndex = 0;
            SetContent(fContentIndex);
            /*
            GuiElement element = sender as GuiElement;
            GuiElement topParent = element.GetTopParent();
            GuiWindow wnd = topParent as GuiWindow;
            if (wnd != null)
            {
                if (wnd.WindowState == GuiWindowState.Maximized)
                {
                    wnd.WindowState = GuiWindowState.Normal;
                }
                else
                {
                    wnd.WindowState = GuiWindowState.Maximized;
                }
            }*/
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

            foreach (var content in fContent)
                content.LoadContent(fEngine);
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
