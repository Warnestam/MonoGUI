using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGUI.Graphics;
using MonoGUI.GameComponents;
using MonoGUI.Engine;



/*
 * File:		GuiWindow
 * Purpose:		A Window that can hold another GuiElement
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2018-11-14  RW  Created
 * 
 */
namespace MonoGUI.Controls
{

    /// <summary>
    /// A Window that can hold another GuiElement
    /// </summary>
    public class GuiWindow : GuiContentControl, IGuiDraggable, IGuiSizeable
    {

        #region Private members

        private Texture2D fTexture;
        private GuiDockPanel fDockPanel;
        private int? fNormalWidth;
        private int? fNormalHeight;
        private int fNormalX;
        private int fNormalY;
        private GuiHorizontalAlignment fNormalHorizontalAlignment;
        private GuiVerticalAlignment fNormalVerticalAlignment;

        private GuiWindowState fWindowState = GuiWindowState.Normal;
        private GuiElement fRenderElement;

        public bool Dragable { get; set; } = true;
        public bool Sizeable { get; set; } = true;
        public bool Clickable { get; set; } = true;

        #endregion

        #region Constructor

        public GuiWindow()
        {
            //Width = 500;
            //Height = 400;
        }

        #endregion

        #region Properties

        public GuiBorder Border { get; set; } = null;
        public GuiDockChild Title { get; set; } = null;

        public int X { get; set; }
        public int Y { get; set; }

        public GuiWindowState WindowState
        {
            get => fWindowState;
            set
            {
                if (fWindowState != value)
                {
                    fWindowState = value;
                    if (fWindowState == GuiWindowState.Normal)
                    {
                        Width = fNormalWidth;
                        Height = fNormalHeight;
                        X = fNormalX;
                        Y = fNormalY;
                        HorizontalAlignment = fNormalHorizontalAlignment;
                        VerticalAlignment = fNormalVerticalAlignment;
                    }
                    else
                    {
                        fNormalWidth = Width;
                        fNormalHeight = Height;
                        fNormalX = X;
                        fNormalY = Y;
                        fNormalHorizontalAlignment = HorizontalAlignment;
                        fNormalVerticalAlignment = VerticalAlignment;
                        Width = null;
                        Height = null;
                        X = 0;
                        Y = 0;
                        HorizontalAlignment = GuiHorizontalAlignment.Stretch;
                        VerticalAlignment = GuiVerticalAlignment.Stretch;
                    }
                    InvalidateMeasure();
                }
            }
        }

        #endregion

        #region Overrides

        public override void Initialize(GraphicsDevice device)
        {
            EnsureControls();
            base.Initialize(device);

            if (fRenderElement != null)
                fRenderElement.Initialize(device);
        }

        public override void LoadContent(GuiEngine engine)
        {
            EnsureControls();

            fTexture = GuiPainter.GetTexture1x1(Device);
            base.LoadContent(engine);
            if (fRenderElement != null)
                fRenderElement.LoadContent(engine);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            if (fRenderElement != null)
                fRenderElement.UnloadContent();
        }

        protected override GuiSize DoMeasure(GuiSize availableSize)
        {
            /* 
            GuiSize frameworkAvailableSize = new GuiSize(availableSize.Width, availableSize.Height);
            GuiMinMax mm = new GuiMinMax(this);
            frameworkAvailableSize.Width = Math.Max(mm.minWidth, Math.Min(frameworkAvailableSize.Width, mm.maxWidth));
            frameworkAvailableSize.Height = Math.Max(mm.minHeight, Math.Min(frameworkAvailableSize.Height, mm.maxHeight));
            GuiSize desiredSize = frameworkAvailableSize;
            if (fRenderElement != null)
            {
                fRenderElement.Measure(frameworkAvailableSize);
            }
            desiredSize = new GuiSize(
                Math.Max(desiredSize.Width, mm.minWidth),
                Math.Max(desiredSize.Height, mm.minHeight));

            return desiredSize;
            */


            // TEST 20200331
            GuiSize frameworkAvailableSize = new GuiSize(availableSize.Width, availableSize.Height);
            GuiMinMax mm = new GuiMinMax(this);
            frameworkAvailableSize.Width = Math.Max(mm.minWidth, Math.Min(frameworkAvailableSize.Width, mm.maxWidth));
            frameworkAvailableSize.Height = Math.Max(mm.minHeight, Math.Min(frameworkAvailableSize.Height, mm.maxHeight));
            GuiSize desiredSize = frameworkAvailableSize;
            if (fRenderElement != null)
            {
                fRenderElement.Measure(frameworkAvailableSize);
                desiredSize = fRenderElement.DesiredSize;
            }
            //  maximize desiredSize with user provided min size
            desiredSize = new GuiSize(
                Math.Max(desiredSize.Width, mm.minWidth),
                Math.Max(desiredSize.Height, mm.minHeight));

            return desiredSize;
        }

        protected override GuiSize DoArrange(GuiSize arrangeSize)
        {
            GuiMinMax mm = new GuiMinMax(this);

            arrangeSize.Width = Math.Max(mm.minWidth, Math.Min(arrangeSize.Width, mm.maxWidth));
            arrangeSize.Height = Math.Max(mm.minHeight, Math.Min(arrangeSize.Height, mm.maxHeight));

            if (fRenderElement != null)
                fRenderElement.Arrange(arrangeSize);

            return arrangeSize;
        }

        protected override void DoDraw(SpriteBatch spriteBatch, GuiPoint point, Rectangle clipRect)
        {
            Rectangle r = new Rectangle(
               point.X + Offset.X,
               point.Y + Offset.Y,
               RenderSize.Width,
               RenderSize.Height);
            if (BackgroundColor != Color.Transparent)
            {
                GuiPainter.DrawRectangle(spriteBatch, r, BackgroundColor, fTexture);
            }
            if (fRenderElement != null)
            {
                fRenderElement.Draw(spriteBatch, new GuiPoint(point.X + Offset.X, point.Y + Offset.Y), clipRect);
            }
            //base.DoDraw(spriteBatch, point, clipRect);
        }

        protected override void DoInvalidateMeasure()
        {
            if (fRenderElement != null)
                fRenderElement.InvalidateMeasure();
            //base.DoInvalidateMeasure();
        }

        #endregion

        #region Private methods

        private void EnsureControls()
        {
            // We "the window" is always drawn first
            // Then the border, title (if so also the dockpanel) and last the borde and then the content

            if (fRenderElement == null)
            {
                if (Border != null)
                {
                    Border.ResizeElement = this;
                }

                if (Title != null)
                {
                    fDockPanel = new GuiDockPanel()
                    {
                        LastChildFill = true,
                        Childs = new List<GuiDockChild>()
                        {
                            Title,
                            new GuiDockChild()
                            {
                                Control=this.Content,
                            }
                        }
                    };
                    Title.Control.DragElement = this;
                    Title.Control.OnClick += Control_OnClick;
                }
                if (Border == null)
                {
                    if (fDockPanel == null)
                    {
                        fRenderElement = Content;
                    }
                    else
                    {
                        this.Content = fDockPanel;
                        fRenderElement = fDockPanel;
                    }
                }
                else
                {
                    if (fDockPanel == null)
                    {
                        Border.Content = Content;
                        this.Content = Border;
                        fRenderElement = Border;
                    }
                    else
                    {
                        Border.Content = fDockPanel;
                        this.Content = Border;
                        fRenderElement = Border;
                    }
                }
            }
        }

        private void Control_OnClick(object sender, EventArgs e)
        {
            if (Clickable && Width != null && Height != null)
            {
                if (this.WindowState == GuiWindowState.Maximized)
                {
                    this.WindowState = GuiWindowState.Normal;
                }
                else
                {
                    this.WindowState = GuiWindowState.Maximized;
                }
            }
        }

        #endregion

        #region IGuiDraggable/IGuiSizeable

        Point IGuiDraggable.GetPosition()
        {
            return new Point(X, Y);
        }

        void IGuiDraggable.SetPosition(Point point)
        {
            X = point.X;
            Y = point.Y;
            InvalidateMeasure();
        }

        void IGuiDraggable.StartDrag()
        {
        }

        void IGuiDraggable.EndDrag()
        {
        }

        bool IGuiDraggable.IsDragEnabled()
        {
            return this.Dragable;
        }

        #endregion

        #region IGuiSizeable

        public GuiRect GetSizeRectangle()
        {
            return new GuiRect(X, Y, Width.GetValueOrDefault(0), Height.GetValueOrDefault(0));
        }

        public void SetSizeRectangle(GuiRect rect)
        {
            if (rect.X >= 0 && rect.Y >= 0 && rect.Width > 0 && rect.Height > 0)
            {
                X = rect.X;
                Y = rect.Y;
                Width = rect.Width;
                Height = rect.Height;
                InvalidateMeasure();
            }
        }

        public void StartResize()
        {
        }

        public void EndResize()
        {
        }

        public bool IsSizingEnabled()
        {
            return this.Sizeable && this.WindowState == GuiWindowState.Normal;
        }

        public GuiSizeElements GetElements(Point point)
        {
            GuiSizeElements elements = GuiSizeElements.None;
            if (this.Border != null)
            {
                elements = this.Border.GetElements(point);
            }
            return elements;
        }

        #endregion

    }

}
