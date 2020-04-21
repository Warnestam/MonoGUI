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
 * File:		GuiCanvasChild
 * Purpose:		A container where children could be placed arbitrary
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2018-11-14  RW  Created
 * 
 */
namespace MonoGUI.Controls
{

    #region The Child 

    public class GuiCanvasChild : IGuiChild
    {
        public int X { get; set; }
        public int Y { get; set; }
        public GuiElement Control { get; set; }
    }

    #endregion
    
    /// <summary>
    /// A container where children could be placed arbitrary
    /// </summary>
    public class GuiCanvasPanel : GuiContainerControl<GuiCanvasChild>
    {

        #region Private members

        private Texture2D fTexture;

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
            int maxWidth = 0;
            int maxHeight = 0;

            if (Childs != null)
            {
                foreach (var child in Childs)
                {
                    GuiSize childConstraint = new GuiSize(
                        Math.Max(0, constraint.Width),
                        Math.Max(0, constraint.Height));

                    child.Control.Measure(childConstraint);
                    GuiSize childDesiredSize = child.Control.DesiredSize;

                    if ((childDesiredSize.Width + child.X) > maxWidth)
                        maxWidth = childDesiredSize.Width + child.X;
                    if ((childDesiredSize.Height + child.Y) > maxHeight)
                        maxHeight = childDesiredSize.Height + child.Y;
                }
            }
            return (new GuiSize(maxWidth, maxHeight));
        }

        protected override GuiSize DoArrange(GuiSize arrangeSize)
        {
            foreach (var child in Childs)
            {
                GuiSize childDesiredSize = child.Control.DesiredSize;
                child.Control.Arrange(childDesiredSize);
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
                        point.X + Offset.X + child.X,
                        point.Y + Offset.Y + child.Y);
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
