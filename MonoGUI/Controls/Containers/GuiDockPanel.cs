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
 * File:		GuiDockPanel
 * Purpose:		A container where the content in docked
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2018-11-14  RW  Created
 * 
 */
namespace MonoGUI.Controls
{

    #region The child

    public class GuiDockChild : IGuiChild
    {
        public GuiPoint ContainerPosition { get; set; }
        public GuiDock Dock { get; set; }
        public GuiElement Control { get; set; }
    }

    #endregion

    /// <summary>
    /// A container where the content in docked
    /// </summary>
    public class GuiDockPanel : GuiContainerControl<GuiDockChild>
    {

        #region Private members

        private Texture2D fTexture;

        #endregion

        #region Properties

        public bool LastChildFill { get; set; }

        #endregion

        #region overrides

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
            int parentWidth = 0;
            int parentHeight = 0;
            int accumulatedWidth = 0;
            int accumulatedHeight = 0;

            if (Childs != null)
            {
                foreach (var child in Childs)
                {
                    GuiSize childConstraint = new GuiSize(
                        Math.Max(0, constraint.Width - accumulatedWidth),
                        Math.Max(0, constraint.Height - accumulatedHeight));

                    child.Control.Measure(childConstraint);
                    GuiSize childDesiredSize = child.Control.DesiredSize;

                    switch (child.Dock)
                    {
                        case GuiDock.Left:
                        case GuiDock.Right:
                            parentHeight = Math.Max(parentHeight, accumulatedHeight + childDesiredSize.Height);
                            accumulatedWidth += childDesiredSize.Width;
                            break;
                        case GuiDock.Top:
                        case GuiDock.Bottom:
                            parentWidth = Math.Max(parentWidth, accumulatedWidth + childDesiredSize.Width);
                            accumulatedHeight += childDesiredSize.Height;
                            break;
                    }
                }
            }
            parentWidth = Math.Max(parentWidth, accumulatedWidth);
            parentHeight = Math.Max(parentHeight, accumulatedHeight);

            return (new GuiSize(parentWidth, parentHeight));
        }

        protected override GuiSize DoArrange(GuiSize arrangeSize)
        {
            int totalChildrenCount = Childs.Count;
            int nonFillChildrenCount = totalChildrenCount - (LastChildFill ? 1 : 0);

            int accumulatedLeft = 0;
            int accumulatedTop = 0;
            int accumulatedRight = 0;
            int accumulatedBottom = 0;

            for (int i = 0; i < totalChildrenCount; ++i)
            {
                GuiDockChild child = Childs[i];
                GuiSize childDesiredSize = child.Control.DesiredSize;
                GuiRect rcChild = new GuiRect(
                    accumulatedLeft,
                    accumulatedTop,
                    Math.Max(0, arrangeSize.Width - (accumulatedLeft + accumulatedRight)),
                    Math.Max(0, arrangeSize.Height - (accumulatedTop + accumulatedBottom)));

                if (i < nonFillChildrenCount)
                {
                    switch (child.Dock)
                    {
                        case GuiDock.Left:
                            accumulatedLeft += childDesiredSize.Width;
                            rcChild.Width = childDesiredSize.Width;
                            break;

                        case GuiDock.Right:
                            accumulatedRight += childDesiredSize.Width;
                            rcChild.X = Math.Max(0, arrangeSize.Width - accumulatedRight);
                            rcChild.Width = childDesiredSize.Width;
                            break;

                        case GuiDock.Top:
                            accumulatedTop += childDesiredSize.Height;
                            rcChild.Height = childDesiredSize.Height;
                            break;

                        case GuiDock.Bottom:
                            accumulatedBottom += childDesiredSize.Height;
                            rcChild.Y = Math.Max(0, arrangeSize.Height - accumulatedBottom);
                            rcChild.Height = childDesiredSize.Height;
                            break;
                    }
                }
                child.Control.Arrange(new GuiSize(rcChild.Width, rcChild.Height));
                child.ContainerPosition = new GuiPoint(rcChild.Left, rcChild.Top);
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
                    // Ska inte Margin bort?
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
