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
using MonoGUI.Engine;
using MonoGUI.GameComponents;
using MonoGUI.Graphics;



/*
 * File:		EngineContainer
 * Purpose:		Render a Engine
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2010-02-28  RW	
 * 
 * History:		2010-02-28  RW  Created
 *              2020-03-30  RW  Code moved to this class from Game.cs
 * 
 */
namespace MonoExperience
{

    public enum ViewPortType { Engine, Window, EngineInWindow };


    /// <summary>
    /// Base class for all engines in the "XNA Experience"
    /// </summary>
    public class EngineContainer
    {

        #region Private members

        private Game fGame;
        private InputManager fManager;
        private Color fBackColor;
        private SpriteBatch fSpriteBatch;
        private RenderTarget2D fCurrentTarget;
        private int fClearBackCount;
        private Texture2D fBackground;

        // Window and sizes
        private Rectangle fAdapterSize;
        private Rectangle fWindowSize;
        private Rectangle fEngineSize;
        private Rectangle fEngineOutputSize;
        private Rectangle fEngineRelativePosition;

        // Various
        GraphicsDeviceManager fGraphics;

        private SpriteFont fHelpFont;
        private Texture2D fLogo;
        private TextBox fBoxInfo;
        private TextBox fBoxHelp;
        private TextBox fBoxAbout;
        private TextBox fBoxFps;

        private bool fIsReady = false;

        private BaseEngine fEngine;
        private GuiEngine fGuiEngine;
        private FpsEngine fFpsEngine;


        #endregion

        #region Properties

        /// <summary>
        /// Get the input manager
        /// </summary>
        public InputManager Manager
        {
            get
            {
                return fManager;
            }
        }

        public Color BackColor
        {
            get
            {
                return fBackColor;
            }
            set
            {
                fBackColor = value;
            }
        }

        public bool ClearScreen { get; set; } = true;
        public bool ShowInfo { get; set; } = true;

        public BaseEngine CurrentEngine
        {
            get
            {
                return fEngine;
            }
            set
            {
                if (fEngine != value)
                {
                    if (fEngine != null)
                    {
                        var controlWindow = fEngine.GetControlWindow(fGuiEngine);
                        if (controlWindow != null)
                        {
                            fGuiEngine.RemoveWindow(controlWindow);
                        }
                    }
                    fEngine = value;
                    if (fEngine != null)
                    {
                        //fEngine.DoInitialize();
                        //fEngine.DoLoadContent();
                        var controlWindow = fEngine.GetControlWindow(fGuiEngine);
                        if (controlWindow != null)
                        {
                            // TODO. CHANGE
                            controlWindow.Initialize(fGraphics.GraphicsDevice);
                            controlWindow.LoadContent(fGuiEngine);
                            fGuiEngine.AddWindow(controlWindow);
                        }
                    }
                }
            }
        }

        public Game Game { get => fGame; }

        #endregion

        #region Constructor

        /// <summary>
        /// Create the engine
        /// </summary>
        /// <param name="game"></param>
        public EngineContainer(Game game, GraphicsDeviceManager graphics, InputManager manager, GuiEngine guiEngine, FpsEngine fpsEngine)
        {
            fGame = game;
            fManager = manager;
            fBackColor = Color.DarkBlue;
            fGuiEngine = guiEngine;
            fFpsEngine = fpsEngine;
            fGraphics = graphics;
        }

        #endregion

        public void LoadContent()
        {
            fSpriteBatch = new SpriteBatch(fGame.GraphicsDevice);
            fBackground = fGame.Content.Load<Texture2D>(@"Background2");
            fHelpFont = fGame.Content.Load<SpriteFont>(@"Fonts\HelpFont");
            fLogo = fGame.Content.Load<Texture2D>(@"monogame");

            fIsReady = true;

            UpdateScale();
            
        }

        public void UnloadContent()
        {
            fIsReady = false;
        }

        public void Update(GameTime gameTime)
        {
            if (CurrentEngine != null)
            {
                CurrentEngine.DoUpdate(gameTime);
            }
        }

        const bool USE_RENDER_TARGET = true;

        public void Draw(GameTime gameTime)
        {
            //var oldViewport = fGame.GraphicsDevice.Viewport;
            //RasterizerState rState = new RasterizerState();
            //fSpriteBatch.GraphicsDevice.RasterizerState = rState;
            var backColor = BackColor;
            if (CurrentEngine != null)
            {
                backColor = CurrentEngine.BackColor;
            }

            if (USE_RENDER_TARGET)
            {
                fGame.GraphicsDevice.SetRenderTarget(fCurrentTarget);
            }
            ApplyViewPort(ViewPortType.Engine);
            if (ClearScreen || fClearBackCount < 10)
            {
                fGame.GraphicsDevice.Clear(BackColor);
            }
            else
            {
                fGame.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Black, 10000, 0);
                fSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                Color c = BackColor;
                c.A = 4;
                fSpriteBatch.Draw(fBackground, fEngineSize, c);
                fSpriteBatch.End();
            }
            fClearBackCount++;
            if (ClearScreen)
                fClearBackCount = 0;

            if (CurrentEngine != null)
            {
                CurrentEngine.DoDraw(gameTime);
            }

            fGame.GraphicsDevice.SetRenderTarget(null);
            ApplyViewPort(ViewPortType.EngineInWindow);
            //fGame.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);
            if (USE_RENDER_TARGET)
            {
                fSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
                fSpriteBatch.Draw(fCurrentTarget, fEngineOutputSize, Color.White);
                fSpriteBatch.End();
            }
            if (ShowInfo)
            {
                ApplyViewPort(ViewPortType.EngineInWindow);
                fSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                RenderHelp();
                RenderInfo();
                RenderFps();
                RenderLogo();
                fSpriteBatch.End();
            }

            //fGame.GraphicsDevice.Viewport = oldViewport;
        }

        public void UpdateScale()
        {
            if (fIsReady)
            {
                CalculateSizes();

                fCurrentTarget = new RenderTarget2D(fGame.GraphicsDevice,
                    fEngineSize.Width, fEngineSize.Height, false,
                    fGame.GraphicsDevice.PresentationParameters.BackBufferFormat,
                    fGame.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                    0, RenderTargetUsage.PreserveContents);

                fBoxHelp = TextBox.CreateTextBox(fGame.GraphicsDevice, fHelpFont,
                    TextBox.TextBoxLocation.TopLeft, 3, 3, 10, 10,
                    Color.DarkRed, new Color(0, 0, 0, 192), new Color(64, 128, 192, 255));
                fBoxAbout = TextBox.CreateWrappedTextBox(fGame.GraphicsDevice, fHelpFont,
                    TextBox.TextBoxLocation.TopRight, 3, 3, 10, 10,
                    Color.DarkRed, new Color(0, 0, 0, 192), new Color(64, 128, 192, 255),
                    320);
                fBoxInfo = TextBox.CreateTextBox(fGame.GraphicsDevice, fHelpFont,
                    TextBox.TextBoxLocation.BotttomLeft, 3, 3, 10, 10,
                    Color.DarkRed, new Color(0, 0, 0, 192), new Color(255, 192, 128, 255));

                fBoxFps = TextBox.CreateTextBox(fGame.GraphicsDevice, fHelpFont,
                    TextBox.TextBoxLocation.BottomRight, 0, 3, 10, 10,
                    Color.Transparent, new Color(128, 128, 0, 32), new Color(0, 0, 0, 255));

                if (CurrentEngine != null)
                {
                    ApplyViewPort(ViewPortType.Engine);
                    CurrentEngine.DisplayChanged();
                }

            }

        }

        private void CalculateSizes()
        {
            fAdapterSize = new Rectangle(0, 0, fGame.GraphicsDevice.DisplayMode.Width, fGame.GraphicsDevice.DisplayMode.Height);
            fWindowSize = new Rectangle(0, 0, fGame.GraphicsDevice.PresentationParameters.BackBufferWidth, fGame.GraphicsDevice.PresentationParameters.BackBufferHeight);
            switch (fGuiEngine.DisplayMode)
            {
                case GuiDisplayMode.Window:
                    fEngineSize = fWindowSize;
                    fEngineRelativePosition = fWindowSize;
                    fEngineOutputSize = fWindowSize;
                    break;
                case GuiDisplayMode.WindowKeepAspectRatio:
                    int width = fWindowSize.Width;
                    int height = fWindowSize.Height;
                    int x = 0;
                    int y = 0;
                    height = (int)(width / fGuiEngine.AspectRatio + 0.5f);
                    if (height > fWindowSize.Height)
                    {
                        height = fWindowSize.Height;
                        width = (int)(height * fGuiEngine.AspectRatio + 0.5f);
                    }
                    x = (fWindowSize.Width / 2) - (width / 2);
                    y = (fWindowSize.Height / 2) - (height / 2);
                    fEngineSize = new Rectangle(0, 0, width, height);
                    fEngineRelativePosition = new Rectangle(x, y, width, height);
                    fEngineOutputSize = fEngineSize;
                    break;
                //case GuiDisplayMode.FullscreenStretch:
                //    fEngineSize = fOptions.PreferredSize;
                //    fEngineRelativePosition = fOptions.PreferredSize;
                //    fEngineOutputSize = fWindowSize;
                //    break;
                case GuiDisplayMode.Fullscreen:
                    fEngineSize = fWindowSize;
                    fEngineRelativePosition = fWindowSize;
                    fEngineOutputSize = fWindowSize;
                    break;
            }
        }

        public void ApplyViewPort(ViewPortType portType)
        {
            Viewport viewport;
            switch (portType)
            {
                case ViewPortType.EngineInWindow:
                    //if (fEngine.DisplayMode == GuiDisplayMode.Fullscreen)
                    //{
                    //    int w = fGraphics.PreferredBackBufferWidth;
                    //    int h = fGraphics.PreferredBackBufferHeight;
                    //    viewport = new Viewport(0, 0, fWindowSize.Width, fWindowSize.Height);
                    //}
                    //else
                    {
                        viewport = new Viewport(fEngineRelativePosition.X, fEngineRelativePosition.Y, fEngineSize.Width, fEngineSize.Height);
                    }
                    break;
                case ViewPortType.Engine:
                    viewport = new Viewport(0, 0, fEngineSize.Width, fEngineSize.Height);
                    break;
                case ViewPortType.Window:
                    viewport = new Viewport(0, 0, fWindowSize.Width, fWindowSize.Height);
                    break;
                default:
                    throw new NotImplementedException();
            }
            fGame.GraphicsDevice.Viewport = viewport;
        }

        /// <summary>
        /// Show own and engine help text
        /// </summary>
        private void RenderHelp()
        {
            List<string> lines = new List<string>();
            lines.Add("ESC Exit");
            lines.Add("F1 Toggle info");
            lines.Add("F3/F4 Change engine");
            lines.Add("F5 Toggle clear");
            lines.Add("F6 Toggle mouse capture");
            lines.Add("ALT/ENTER Toggle fullscreen");
            if (CurrentEngine != null)
            {
                string[] help = CurrentEngine.GetHelp().Split('\n');
                foreach (string text in help)
                {
                    lines.Add(text);
                }
            }
            fBoxHelp.Text = lines;
            fBoxHelp.Render(fSpriteBatch);

            List<string> aboutLines = new List<String>();
            aboutLines.Add("Monogame Experience 5.0\nRobert Warnestam 2010/2016/2020");
            if (CurrentEngine != null)
            {
                aboutLines.Add(new String('-', 60));
                aboutLines.Add(String.Format("Engine: {0}", CurrentEngine.GetName()));
                aboutLines.Add("");
                aboutLines.Add(CurrentEngine.GetAbout());
                aboutLines.Add("");
            }
            fBoxAbout.Text = aboutLines;
            fBoxAbout.Render(fSpriteBatch);
        }

        /// <summary>
        /// Show own and engine info text
        /// </summary>
        private void RenderInfo()
        {
            if (CurrentEngine != null)
            {
                string engineName = engineName = CurrentEngine.GetName();

                List<string> lines = new List<string>();
                // lines.Add(String.Format("Current engine: {0} ({1} of {2})", engineName, fCurrentEngineIndex + 1, fEngines.Count));
                string[] info = CurrentEngine.GetInfo().Split('\n');
                foreach (string text in info)
                {
                    lines.Add(text);
                }
                fBoxInfo.Text = lines;
                fBoxInfo.Render(fSpriteBatch);
            }
        }

        /// <summary>
        /// Show FPS
        /// </summary>
        private void RenderFps()
        {
            //MouseState mouse = fInputManager.GetMouseState();
            MouseState mouse = Mouse.GetState();

            List<string> lines = new List<string>();
            lines.Add(String.Format("FPS: {0}", fFpsEngine.FrameRate));
            lines.Add(String.Format("Mode: {0}", fGuiEngine.DisplayMode));
            lines.Add(String.Format("Mouse: {0} x {1}", mouse.X, mouse.Y));
            lines.Add(String.Format("Adapter: {0} x {1}", fAdapterSize.Width, fAdapterSize.Height));
            lines.Add(String.Format("Window: {0} x {1}", fWindowSize.Width, fWindowSize.Height));
            lines.Add(String.Format("Engine: {0} x {1} ({2},{3})", fEngineSize.Width, fEngineSize.Height, fEngineRelativePosition.X, fEngineRelativePosition.Y));

            lines.Add(String.Format("Back: {0} x {1}", fGame.GraphicsDevice.PresentationParameters.BackBufferWidth, fGame.GraphicsDevice.PresentationParameters.BackBufferHeight));
            lines.Add(String.Format("PrefBack: {0} x {1}", fGraphics.PreferredBackBufferWidth, fGraphics.PreferredBackBufferHeight));
            lines.Add(String.Format("AspectRatio: {0}", fGuiEngine.AspectRatio));

            fBoxFps.Text = lines;
            fBoxFps.Render(fSpriteBatch);
        }

        private void RenderLogo()
        {
            fSpriteBatch.Draw(fLogo, new Rectangle(
                (fGame.GraphicsDevice.Viewport.Width - fLogo.Width) / 2,
                fGame.GraphicsDevice.Viewport.Height - fLogo.Height,
                fLogo.Width, fLogo.Height), Color.White);
        }




    }

}
