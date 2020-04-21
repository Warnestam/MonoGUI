using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGUI.Graphics;
using MonoGUI.Controls;
using MonoGUI.Engine;
using MonoGUI.GameComponents;

/*
 * File:		GuiEngine
 * Purpose:		A GUI Engine for Monogame
 * 
 * Author(s):	RW: Robert Warnestam
 * History:		2017-11-09  RW  Created
 *              2020-03-29   RW  GuiDisplayMode
 *              
 * 
 */
namespace MonoGUI.Engine
{



    /// <summary>
    /// A GUI Engine for Monogame
    /// </summary>
    public class GuiEngine : Microsoft.Xna.Framework.DrawableGameComponent
    {

        #region Private members

        private GraphicsDeviceManager fGraphics;
        private List<GuiWindow> fWindows = new List<GuiWindow>();
        private GuiEngineOptions fOptions;
        private GuiDisplayMode fDisplayMode;

        public Rectangle AdapterSize { get; private set; }
        public Rectangle WindowSize { get; private set; }

        public float AspectRatio { get; private set; }
        private bool fIsReady;

        private Vector2 fScale;

        private Point fClickStartMouse;
        private Point fDragStartPosition;

        private GuiRect fResizeStartRect;
        private GuiSizeElements fResizeElements;

        private RenderTarget2D fRenderTarget;
        private Effect fEffect;
        private SpriteBatch fSpriteBatch;
        private SpriteFont fStandardFont;

        private Texture2D fTexture;
        private GuiElement fTestElement;

        private string fOldTitle;

        #endregion

        #region OnResize

        public event EventHandler<EventArgs> OnResize;

        protected internal void DoOnResize()
        {
            if (OnResize != null)
            {
                OnResize(this, new EventArgs());
            }
        }

        #endregion

        #region Properties

        public GuiDisplayMode DisplayMode
        {
            get => fDisplayMode;
            set
            {
                if (fDisplayMode != value)
                {
                    fDisplayMode = value;
                    SetResolution();
                }
            }
        }


        #endregion

        #region Automatic properties

        public InputManager InputManager { get; private set; }
        public FpsEngine FPS { get; private set; }

        public GuiElement MouseHoverControl { get; private set; }
        public IGuiDraggable DragControl { get; private set; }
        public IGuiSizeable SizeControl { get; private set; }

        public GuiElement PressedControl { get; private set; }

        public GuiMouseState MouseState { get; private set; }
        public Point MousePosition { get; private set; }
        public Point ScaledPosition { get; private set; }

        public GuiElement MouseOverElement { get; private set; }

        #endregion

        #region Properties


        public int Width
        {
            get
            {
                return WindowSize.Width;
            }
        }
        public int Height
        {
            get
            {
                return WindowSize.Height;
            }
        }

        #endregion

        #region Constructor

        public GuiEngine(Game game, GuiEngineOptions options, GraphicsDeviceManager device)
                : base(game)
        {
            fOptions = options;
            fGraphics = device;

            FPS = new FpsEngine(game);
            InputManager = new InputManager(game);

            this.Game.Components.Add(FPS);
            this.Game.Components.Add(InputManager);

        }


        #endregion

        #region DrawableGameComponent

        public override void Initialize()
        {
            InitScreen();
            Game.Window.ClientSizeChanged += (s, e) => ResolutionChanged();
            Game.Window.TextInput += Window_TextInput;

            fSpriteBatch = new SpriteBatch(this.GraphicsDevice);

            InputManager.InitMouse();
            InputManager.CaptureMouse = false;

            foreach (GuiWindow window in fWindows)
                window.Initialize(this.GraphicsDevice);
            MouseState = GuiMouseState.Moving;

            base.Initialize();

        }

        private void Window_TextInput(object sender, TextInputEventArgs e)
        {
            // TODO: Only focused window
            foreach (var window in fWindows)
            {
                //window.OnTextInput(e);
            }
        }

        protected override void LoadContent()
        {
            fStandardFont = Game.Content.Load<SpriteFont>(@"Fonts\LabelFont");

            foreach (GuiWindow window in fWindows)
                window.LoadContent(this);

            fIsReady = true;

            fEffect = this.Game.Content.Load<Effect>(@"Effects\SimpleAlpha");
            ResolutionChanged();
            InitScreen();

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();

            fIsReady = false;
            foreach (GuiWindow window in fWindows)
                window.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var mouse = InputManager.GetMouseState();
            MousePosition = mouse.Position;

            ScaledPosition = new Point(
                Convert.ToInt32(fScale.X * mouse.Position.X),
                 Convert.ToInt32(fScale.Y * mouse.Position.Y));

           
            GuiWindow topWindow = null;
            if (fWindows.Count > 0)
                topWindow = fWindows[fWindows.Count - 1];

            switch (MouseState)
            {
                case GuiMouseState.Moving:
                    var ctrlTop = topWindow.FindElement(ScaledPosition);
                    if (mouse.LeftButton == ButtonState.Released)
                    {
                        if (ctrlTop != MouseHoverControl)
                        {
                            if (MouseHoverControl != null)
                            {
                                MouseHoverControl = null;
                            }
                            if (ctrlTop != null)
                            {
                                MouseHoverControl = ctrlTop;
                            }
                        }
                    }
                    else
                    {
                        if (MouseHoverControl != null)
                        {
                            MouseHoverControl = null;
                        }
                        // If there is any window that has a clickable element under us => select that window now
                        fClickStartMouse = ScaledPosition;
                        GuiWindow checkWindow = CheckWindow(ScaledPosition, true);
                        if (checkWindow != null && topWindow != checkWindow)
                        {
                            BringWindowToFront(checkWindow);
                            topWindow = checkWindow;
                        }
                        ctrlTop = topWindow.FindElement(ScaledPosition);
                        if (ctrlTop != null)
                        {
                            PressedControl = ctrlTop;
                            // In case we will start dragging or resizing
                            if (ctrlTop.DragElement != null)
                            {
                                fDragStartPosition = ctrlTop.DragElement.GetPosition();
                            }
                            if (ctrlTop.ResizeElement != null)
                            {
                                fResizeStartRect = ctrlTop.ResizeElement.GetSizeRectangle();
                            }
                        }
                        MouseState = GuiMouseState.Pressed;
                    }
                    break;
                case GuiMouseState.Dragging:
                    if (mouse.LeftButton == ButtonState.Released)
                    {
                        DragControl.EndDrag();
                        DragControl = null;
                        MouseState = GuiMouseState.Moving;
                    }
                    else
                    {
                        // Continue dragging
                        Point delta = ScaledPosition - fClickStartMouse;
                        Point newPosition = fDragStartPosition + delta;
                        DragControl.SetPosition(newPosition);
                    }
                    break;
                case GuiMouseState.Sizing:
                    if (mouse.LeftButton == ButtonState.Released)
                    {
                        SizeControl.EndResize();
                        SizeControl = null;
                        MouseState = GuiMouseState.Moving;
                    }
                    else
                    {
                        // Continue resizing
                        Point delta = ScaledPosition - fClickStartMouse;
                        GuiRect newRect = fResizeStartRect;
                        if ((fResizeElements & GuiSizeElements.Height) != 0)
                        {
                            newRect.Height += delta.Y;
                        }
                        if ((fResizeElements & GuiSizeElements.Width) != 0)
                        {
                            newRect.Width += delta.X;
                        }
                        if ((fResizeElements & GuiSizeElements.Left) != 0)
                        {
                            newRect.X += delta.X;
                            newRect.Width -= delta.X;
                        }
                        if ((fResizeElements & GuiSizeElements.Top) != 0)
                        {
                            newRect.Y += delta.Y;
                            newRect.Height -= delta.Y;
                        }
                        SizeControl.SetSizeRectangle(newRect);
                    }
                    break;
                case GuiMouseState.Pressed:
                    //BringWindowToFront(window);
                    if (mouse.LeftButton == ButtonState.Released)
                    {
                        if (PressedControl != null)
                        {
                            PressedControl.DoOnClick();//TODO?
                            //XYZPressedControl.SetMousePressed(false);
                            PressedControl = null;
                        }
                        MouseState = GuiMouseState.Moving;
                    }
                    else
                    {
                        // Pressed can change to dragging
                        Point delta = ScaledPosition - fClickStartMouse;
                        if (delta.X > 2 || delta.Y > 2 || delta.X < -2 || delta.Y < -2)
                        {
                            if (PressedControl != null)
                            {
                                //XYZPressedControl.SetMousePressed(false);
                                IGuiDraggable dragControl = PressedControl.DragElement;
                                if (dragControl != null && dragControl.IsDragEnabled())
                                {
                                    DragControl = dragControl;
                                    PressedControl = null;
                                    DragControl.StartDrag();
                                    MouseState = GuiMouseState.Dragging;
                                }
                                else
                                {
                                    IGuiSizeable sizeControl = PressedControl.ResizeElement;
                                    if (sizeControl != null && sizeControl.IsSizingEnabled())
                                    {
                                        SizeControl = sizeControl;
                                        PressedControl = null;
                                        SizeControl.StartResize();
                                        fResizeElements = SizeControl.GetElements(fClickStartMouse);
                                        MouseState = GuiMouseState.Sizing;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Still pressed...
                        }

                    }
                    break;
            }
            if (!UpdatePaused)
            {
                foreach (GuiWindow wnd in fWindows)
                    wnd.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            bool USE_RENDER_TARGET = false;

            if (USE_RENDER_TARGET)
            {
                this.GraphicsDevice.SetRenderTarget(fRenderTarget);
                GraphicsDevice.Clear(Color.TransparentBlack);

                if (fWindows.Count > 0)
                {
                    for (int i = 0; i < fWindows.Count - 1; i++)
                    {
                        GuiWindow window = fWindows[i];
                        DrawWindow(window, BlendState.AlphaBlend);
                    }
                    this.GraphicsDevice.SetRenderTarget(null);//Removes the background. Could change pp
                    GraphicsDevice.Clear(Color.CornflowerBlue);
                    fSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
                    fSpriteBatch.Draw(fRenderTarget, new Vector2(0, 0), new Color(255, 255, 255, 64));
                    fSpriteBatch.End();

                    DrawWindow(fWindows[fWindows.Count - 1], BlendState.NonPremultiplied);
                }
            }
            else
            {
                foreach (GuiWindow window in fWindows)
                    DrawWindow(window, BlendState.NonPremultiplied);
            }
        }

        private void DrawOutlineElement(GameTime gameTime)
        {
            if (fTestElement != null)
            {
                fSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);

                if (fTexture == null)
                    fTexture = GuiPainter.GetTexture1x1(fGraphics.GraphicsDevice);
                Color color1 = Color.Red;
                Color color2 = Color.Red;
                if (gameTime.TotalGameTime.TotalMilliseconds % 1000 < 500)
                {
                    color1 = Color.Blue;
                    color2 = Color.Blue;
                }
                int x0 = fTestElement.DrawPosition.X;
                int y0 = fTestElement.DrawPosition.Y;
                int w = fTestElement.FinalSize.Width;
                int h = fTestElement.FinalSize.Height;
                int x1 = x0 + w - 1;
                int y1 = y0 + h - 1;
                GuiPainter.DrawHorizontalLine(fSpriteBatch, x0, y0, w, color1, fTexture);
                GuiPainter.DrawHorizontalLine(fSpriteBatch, x0, y1, w, color1, fTexture);
                GuiPainter.DrawVerticalLine(fSpriteBatch, x0, y0, h, color1, fTexture);
                GuiPainter.DrawVerticalLine(fSpriteBatch, x1, y0, h, color1, fTexture);
                x0 = fTestElement.DrawPosition.X + fTestElement.Offset.X;
                y0 = fTestElement.DrawPosition.Y + fTestElement.Offset.Y;
                w = fTestElement.RenderSize.Width;
                h = fTestElement.RenderSize.Height;
                x1 = x0 + w - 1;
                y1 = y0 + h - 1;
                GuiPainter.DrawHorizontalLine(fSpriteBatch, x0, y0, w, color2, fTexture);
                GuiPainter.DrawHorizontalLine(fSpriteBatch, x0, y1, w, color2, fTexture);
                GuiPainter.DrawVerticalLine(fSpriteBatch, x0, y0, h, color2, fTexture);
                GuiPainter.DrawVerticalLine(fSpriteBatch, x1, y0, h, color2, fTexture);
                fSpriteBatch.End();
            }

        }

        #endregion

        #region Public methods

        public void AddWindow(GuiWindow window)
        {
            fWindows.Add(window);
        }

        public void RemoveWindow(GuiWindow window)
        {
            fWindows.Remove(window);
        }

        public void ToggleFullscreen()
        {
            switch (DisplayMode)
            {
                case GuiDisplayMode.Window:
                    DisplayMode = GuiDisplayMode.WindowKeepAspectRatio;
                    break;
                case GuiDisplayMode.WindowKeepAspectRatio:
                    DisplayMode = GuiDisplayMode.Fullscreen;
                    break;
                case GuiDisplayMode.Fullscreen:
                    DisplayMode = GuiDisplayMode.Window;
                    break;
            }
        }

        public void WindowResized()
        {
            foreach (var window in fWindows)
            {
                window.InvalidateMeasure();
            }
            DoOnResize();
        }

        #endregion

        #region Private methods


        private GuiWindow CheckWindow(Point position, bool checkForClickableElement)
        {
            GuiWindow result = null;

            for (int i = fWindows.Count - 1; i >= 0; i--)
            {
                var cursorElement = fWindows[i].FindElement(position);
                if (cursorElement != null)
                {
                    if (checkForClickableElement)
                    {
                        if (cursorElement.DoHasClickableElement())
                        {
                            result = fWindows[i];
                            break;
                        }
                    }
                    else
                    {
                        result = fWindows[i];
                        break;
                    }
                }
            }
            return result;
        }

        private void BringWindowToFront(GuiWindow window)
        {
            if (window != null)
            {
                fWindows.Remove(window);
                fWindows.Add(window);
            }
        }

        private void BuildRenderTarget()
        {
            PresentationParameters pp = this.GraphicsDevice.PresentationParameters;
            fRenderTarget = new RenderTarget2D(this.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.None);
        }

        private void InitScreen()
        {
            fGraphics.PreferMultiSampling = false;
            fGraphics.IsFullScreen = false;
            fGraphics.SynchronizeWithVerticalRetrace = false;

            AspectRatio = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.AspectRatio;
            DisplayMode = GuiDisplayMode.Window;
        }

        private void SetResolution()
        {
            switch (DisplayMode)
            {
                case GuiDisplayMode.Window:
                case GuiDisplayMode.WindowKeepAspectRatio:
                    fGraphics.PreferredBackBufferWidth = fOptions.PreferredSize.Width;
                    fGraphics.PreferredBackBufferHeight = fOptions.PreferredSize.Height;
                    fGraphics.IsFullScreen = false;
                    fGraphics.ApplyChanges();
                    break;
                //case GuiDisplayMode.FullscreenStretch:
                //    fGraphics.PreferredBackBufferWidth = fOptions.PreferredSize.Width;
                //    fGraphics.PreferredBackBufferHeight = fOptions.PreferredSize.Height;
                //    fGraphics.IsFullScreen = true;
                //    fGraphics.ApplyChanges();
                //    break;
                case GuiDisplayMode.Fullscreen:
                    fGraphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
                    fGraphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
                    fGraphics.IsFullScreen = true;
                    fGraphics.ApplyChanges();
                    break;
            }
            ResolutionChanged();
        }


        private void ResolutionChanged()
        {
            if (fIsReady)
            {
                CalculateSizes();
                BuildRenderTarget();
                //...
            }
            WindowResized();

        }

        private void CalculateSizes()
        {
            AdapterSize = new Rectangle(0, 0, GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height);
            //WindowSize = new Rectangle(0, 0, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight);
            WindowSize = Game.Window.ClientBounds;

            if (GraphicsDevice.PresentationParameters.IsFullScreen)
            {
                fScale = new Vector2(
                    (float)(GraphicsDevice.PresentationParameters.BackBufferWidth) / GraphicsDevice.DisplayMode.Width,
                    (float)(GraphicsDevice.PresentationParameters.BackBufferHeight) / GraphicsDevice.DisplayMode.Height);
            }
            else
            {
                fScale = new Vector2(1, 1);
            }


        }

        private void DrawWindow(GuiWindow window, BlendState state)
        {
            if (!window.MeasureValid)
            {
                GuiSize availableSize = new GuiSize(Width, Height);
                window.Measure(availableSize);
            }
            if (!window.ArrangeValid)
            {
                window.Arrange(window.DesiredSize);
            }

            //RasterizerState oldState = fSpriteBatch.GraphicsDevice.RasterizerState;
            RasterizerState rState = new RasterizerState();
            rState.ScissorTestEnable = true;
            fSpriteBatch.GraphicsDevice.RasterizerState = rState;
            fSpriteBatch.Begin(SpriteSortMode.Immediate, state, null, null, rState);
            Rectangle clipRect = new Rectangle(window.X, window.Y, Width, Height);
            window.Draw(fSpriteBatch, new GuiPoint(window.X, window.Y), clipRect);
            fSpriteBatch.End();
            //fSpriteBatch.GraphicsDevice.RasterizerState = oldState;
        }

        #endregion

        public bool UpdatePaused { get; set; }

        public SpriteFont StandardFont { get => fStandardFont; }

        //        ResolutionChanged=UpdateScale

    }
}



