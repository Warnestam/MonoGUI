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
 * File:		GuiTabControl
 * Purpose:		A control with tabbed items
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2018-11-14  RW  Created
 * 
 */
namespace MonoGUI.Controls
{

    #region The Child

    public class GuiTabControlChild : IGuiChild
    {
        public GuiElement Header { get; set; }
        public GuiElement Control { get; set; }
    }

    #endregion

    /// <summary>
    /// A control with tabbed items
    /// </summary>
    public class GuiTabControl : GuiContainerControl<GuiTabControlChild>
    {

        #region Private members

        private Texture2D fTexture;
        private int fSelectedIndex;
        private GuiDockPanel fDockPanel;

        #endregion

        #region Overrides

        public override void Initialize(GraphicsDevice device)
        {
            base.Initialize(device);
            if (Childs != null)
            {
                foreach (var child in Childs)
                {
                    child.Header.Initialize(device);
                }
            }

            BuildDockPanelWithHeaders();
            SetSelectedIndex(0);
        }

        public override void LoadContent(GuiEngine engine)
        {
            fTexture = GuiPainter.GetTexture1x1(Device);

            base.LoadContent(engine);
            if (Childs != null)
            {
                foreach (var child in Childs)
                {
                    child.Header.LoadContent(engine);
                }
            }
        }

        protected override void DoInvalidateMeasure()
        {
            base.DoInvalidateMeasure();
            if (Childs != null)
            {
                foreach (var child in Childs)
                {
                    child.Header.InvalidateMeasure();
                }
            }
        }

        

        public override void UnloadContent()
        {
            base.UnloadContent();
            if (Childs != null)
            {
                foreach (var child in Childs)
                {
                    child.Header.UnloadContent();
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Childs != null)
            {
                foreach (var child in Childs)
                {
                    child.Header.Update(gameTime);
                }
            }
        }

        protected override GuiSize DoMeasure(GuiSize availableSize)
        {
            GuiSize frameworkAvailableSize = new GuiSize(availableSize.Width, availableSize.Height);

            GuiMinMax mm = new GuiMinMax(this);

            frameworkAvailableSize.Width = Math.Max(mm.minWidth, Math.Min(frameworkAvailableSize.Width, mm.maxWidth));
            frameworkAvailableSize.Height = Math.Max(mm.minHeight, Math.Min(frameworkAvailableSize.Height, mm.maxHeight));

            GuiSize desiredSize = frameworkAvailableSize;// MeasureOverrideHelper(frameworkAvailableSize);


            fDockPanel.Measure(frameworkAvailableSize);

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

            fDockPanel.Arrange(arrangeSize);

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

            GuiPoint childPoint = new GuiPoint(
                   point.X + Offset.X,
                   point.Y + Offset.Y);
            fDockPanel.Draw(spriteBatch, childPoint, clipRect);

        }

        protected override GuiElement DoFindElement(Point point)
        {
            GuiElement result = null;
            if (point.X >= (DrawPosition.X + Offset.X) &&
                    point.X < (DrawPosition.X + Offset.X + RenderSize.Width) &&
                    point.Y >= (DrawPosition.Y + Offset.Y) &&
                    point.Y < (DrawPosition.Y + Offset.Y + RenderSize.Height))
            {
                GuiElement subElement = fDockPanel.FindElement(point);
                if (subElement != null)
                {
                    result = subElement;
                }

            }
            return result;
        }

        #endregion

        #region Private methods

        private void BuildDockPanelWithHeaders()
        {
            GuiElement selectedControl = null;
            List<GuiStackChild> headers = new List<GuiStackChild>();
            if (Childs != null && Childs.Count > 0)
            {
                selectedControl = Childs[fSelectedIndex].Control;
                foreach (var child in Childs)
                {
                    headers.Add(new GuiStackChild() { Control = child.Header });
                    child.Header.OnClick += Header_OnClick;// TODO remove later...
                }
            }
            fDockPanel = new GuiDockPanel()
            {
                LastChildFill = true,
                Childs = new List<GuiDockChild>()
                {
                    new GuiDockChild()
                    {
                        Control=new GuiStackPanel()
                        {
                            Orientation=GuiStackPanelOrientation.Horizontal,
                            Childs = headers
                        },
                        Dock=GuiDock.Top
                    }, new GuiDockChild()
                    {
                        Control=selectedControl
                    }
                }
            };
            fDockPanel.Parent = this;
        }

        private void Header_OnClick(object sender, EventArgs e)
        {
            if (Childs != null && Childs.Count > 0)
            {
                int index = 0;
                foreach (var child in Childs)
                {
                    if (child.Header == sender)
                    {
                        SetSelectedIndex(index);
                        break;
                    }
                    index++;
                }
            }  
        }

        private void SetSelectedIndex(int index)
        {
            fSelectedIndex = index;
            if (Childs != null && Childs.Count > 0)
            {
                fDockPanel.Childs[1].Control = Childs[fSelectedIndex].Control;
                //fDockPanel.Childs[1].Control.Parent = fDockPanel;
                fDockPanel.InvalidateMeasure();
            }
        }

        #endregion

    }

}
