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
 * File:		GuiExpandablePanel
 * Purpose:		A panel that can be expanded or collapsed
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2018-11-28  RW  Created
 * 
 */
namespace MonoGUI.Controls
{

    /// <summary>
    /// A panel that can be expanded or collapsed
    /// </summary>
    public class GuiExpandablePanel : GuiContentControl
    {

        #region Private members

        private Texture2D fTexture;
        private GuiDockPanel fDockPanel;

        private GuiExpandablePanelState fPanelState = GuiExpandablePanelState.Expanded;
        private GuiElement fRenderElement;
        private GuiElement fOriginalContent;

        #endregion

        #region Constructor

        public GuiExpandablePanel()
        {
        }

        #endregion

        #region Properties

        public GuiBorder Border { get; set; } = null;
        public GuiDockChild Title { get; set; } = null;

        public GuiExpandablePanelState PanelState
        {
            get => fPanelState;
            set
            {
                if (fPanelState != value)
                {
                    fPanelState = value;
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
            
            SetCollapsedOnContent();

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
            SetCollapsedOnContent();
            if (fRenderElement != null)
                fRenderElement.InvalidateMeasure();
            //base.DoInvalidateMeasure();
        }

        #endregion

        #region Private methods

        private void SetCollapsedOnContent()
        {
            if (fOriginalContent != null)
            {
                if (PanelState == GuiExpandablePanelState.Expanded)
                {
                    fOriginalContent.Visibility = GuiVisibility.Visible;
                }
                else
                {
                    fOriginalContent.Visibility = GuiVisibility.Collapsed;
                }
            }
        }

        private void EnsureControls()
        {
            // We "the window" is always drawn first
            // Then the border, title (if so also the dockpanel) and last the borde and then the content

            if (fRenderElement == null)
            {
                fOriginalContent = this.Content;
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
                        this.Content = fDockPanel;
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
            if (this.PanelState == GuiExpandablePanelState.Collapsed)
            {
                this.PanelState = GuiExpandablePanelState.Expanded;
            }
            else
            {
                this.PanelState = GuiExpandablePanelState.Collapsed;
            }
        }

        #endregion

    }

}
