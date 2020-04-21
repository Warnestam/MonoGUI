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
 * File:		GuiStackPanel
 * Purpose:		A panel where the items is stacked
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2018-11-14  RW  Created
 * 
 * 
 */
namespace MonoGUI.Controls
{

    #region The Child 

    public class GuiStackChild : IGuiChild
    {
        public GuiPoint ContainerPosition { get; set; }
        public GuiElement Control { get; set; }
    }

    #endregion

    /// <summary>
    /// A panel where the items is stacked
    /// </summary>
    public class GuiStackPanel : GuiContainerControl<GuiStackChild>
    {

        #region Private members

        private Texture2D fTexture;

        #endregion

        #region Properties

        public GuiStackPanelOrientation Orientation { get; set; } = GuiStackPanelOrientation.Vertical;

        #endregion

        #region Overrides

        public override void Initialize(GraphicsDevice device)
        {
            base.Initialize(device);
        }

        public override void LoadContent(GuiEngine engine)
        {
            fTexture = GuiPainter.GetTexture1x1(Device);
            base.LoadContent(engine);
        }

        protected override GuiSize DoMeasure(GuiSize constraint)
        {
            GuiSize desiredSize = MeasureStack(constraint);
            return desiredSize;
        }

        private GuiSize MeasureStack(GuiSize constraint)
        {
            GuiSize stackDesiredSize = new GuiSize();

            if (Childs != null)
            {
                GuiSize layoutSlotSize = constraint;
                foreach (var child in Childs)
                {
                    child.Control.Measure(layoutSlotSize);
                    GuiSize childDesiredSize = child.Control.DesiredSize;
                    if (Orientation == GuiStackPanelOrientation.Horizontal)
                    {
                        stackDesiredSize.Width += childDesiredSize.Width;
                        stackDesiredSize.Height = Math.Max(stackDesiredSize.Height, childDesiredSize.Height);
                        layoutSlotSize.Width -= childDesiredSize.Width;
                    }
                    else
                    {
                        stackDesiredSize.Width = Math.Max(stackDesiredSize.Width, childDesiredSize.Width);
                        stackDesiredSize.Height += childDesiredSize.Height;
                        layoutSlotSize.Height -= childDesiredSize.Height;
                    }
                }
            }
            return stackDesiredSize;
        }

        protected override GuiSize DoArrange(GuiSize arrangeSize)
        {
            GuiSize childSize = new GuiSize(arrangeSize.Width, arrangeSize.Height);
            GuiSize childSizeAvailable = new GuiSize(arrangeSize.Width, arrangeSize.Height);
            GuiPoint childPosition = new GuiPoint();
            int previousChildSize = 0;
            if (Childs != null)
            {
                foreach (var child in Childs)
                {
                    if (Orientation == GuiStackPanelOrientation.Horizontal)
                    {
                        childPosition.X += previousChildSize;
                        previousChildSize = child.Control.DesiredSize.Width;
                        childSize.Width = previousChildSize;
                        childSize.Height = Math.Max(arrangeSize.Height, child.Control.DesiredSize.Height);

                        if (childSize.Width > childSizeAvailable.Width)
                            childSize.Width = childSizeAvailable.Width;
                        if (childSize.Height > childSizeAvailable.Height)
                            childSize.Height = childSizeAvailable.Height;
                        childSizeAvailable.Width -= childSize.Width;
                    }
                    else
                    {
                        childPosition.Y += previousChildSize;
                        previousChildSize = child.Control.DesiredSize.Height;
                        childSize.Height = previousChildSize;
                        childSize.Width = Math.Max(arrangeSize.Width, child.Control.DesiredSize.Width);

                        if (childSize.Width > childSizeAvailable.Width)
                            childSize.Width = childSizeAvailable.Width;
                        if (childSize.Height > childSizeAvailable.Height)
                            childSize.Height = childSizeAvailable.Height;
                        childSizeAvailable.Height -= childSize.Height;
                    }
                    child.Control.Arrange(childSize);
                    child.ContainerPosition = childPosition;
                }
            }
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
            if (Childs != null)
            {
                foreach (var child in Childs)
                {
                    GuiPoint childPoint = new GuiPoint(
                        point.X + Offset.X + child.ContainerPosition.X,
                       point.Y + Offset.Y + child.ContainerPosition.Y);
                    child.Control.Draw(spriteBatch, childPoint, clipRect);
                }
            }

        }

        protected override GuiElement DoFindElement(Point point)
        {
            GuiElement result = null;
            if (point.X >= (DrawPosition.X + Offset.X) &&
                    point.X < (DrawPosition.X + Offset.X + RenderSize.Width) &&
                    point.Y >= (DrawPosition.Y + Offset.Y) &&
                    point.Y < (DrawPosition.Y + Offset.Y + RenderSize.Height))
            {
                foreach (var child in Childs)
                {
                    GuiElement subElement = child.Control.FindElement(point);
                    if (subElement != null)
                    {
                        result = subElement;
                        break;
                    }
                }
                
            }
            return result;
        }

        #endregion

    }

}
