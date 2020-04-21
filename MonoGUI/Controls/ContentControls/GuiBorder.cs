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
 * File:		GuiBorder
 * Purpose:		Border around a control
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2018-11-17  RW  Created
 * 
 */
namespace MonoGUI.Controls
{

    /// <summary>
    /// Border around a control
    /// </summary>
    public class GuiBorder : GuiContentControl
    {

        #region Private members

        private Texture2D fTexture;

        #endregion

        #region Constructor

        public GuiBorder()
        {
        }

        #endregion

        #region Properties

        public bool Sizeable { get; set; }

        public GuiThickness Border { get; set; }

        public Color BorderColor { get; set; }

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

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override GuiSize DoMeasure(GuiSize availableSize)
        {
            GuiSize frameworkAvailableSize = new GuiSize(availableSize.Width, availableSize.Height);

            GuiMinMax mm = new GuiMinMax(this);

            frameworkAvailableSize.Width = Math.Max(mm.minWidth, Math.Min(frameworkAvailableSize.Width, mm.maxWidth));
            frameworkAvailableSize.Height = Math.Max(mm.minHeight, Math.Min(frameworkAvailableSize.Height, mm.maxHeight));

            GuiSize desiredSize = frameworkAvailableSize;// MeasureOverrideHelper(frameworkAvailableSize);


            if (Content != null)
            {
                GuiSize childAvailableSize = new GuiSize(frameworkAvailableSize.Width - Border.Width, frameworkAvailableSize.Height - Border.Height);
                Content.Measure(childAvailableSize);
                desiredSize = Content.DesiredSize;
                desiredSize.Width += Border.Width;
                desiredSize.Height += Border.Height;
            }
            else
            {
                //desiredSize = new GuiSize(0, 0);
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

            GuiSize childSize = arrangeSize;
            childSize.Width -= Border.Width;
            childSize.Height -= Border.Height;

            if (Content != null)
            {
                Content.Arrange(childSize);
            }

            return arrangeSize;
        }

        protected override void DoDraw(SpriteBatch spriteBatch, GuiPoint point, Rectangle clipRect)
        {
            int outerX0 = point.X + Offset.X;
            int outerY0 = point.Y + Offset.Y;
            int innerX0 = outerX0 + Border.Left;
            int innerY0 = outerY0 + Border.Top;
            int outerWidth = RenderSize.Width;
            int outerHeight = RenderSize.Height;
            int innerWidth = outerWidth - Border.Width;
            int innerHeigth = outerHeight - Border.Height;

            if (BorderColor != Color.Transparent)
            {
                Rectangle topRect = new Rectangle(outerX0, outerY0, outerWidth - Border.Right, Border.Top);
                Rectangle bottomRect = new Rectangle(innerX0, outerY0 + innerHeigth + Border.Top, outerWidth, Border.Bottom);
                Rectangle leftRect = new Rectangle(outerX0, innerY0, Border.Left, outerHeight);
                Rectangle rightRect = new Rectangle(outerX0 + innerWidth + Border.Left, outerY0, Border.Right, outerHeight - Border.Bottom);
                if (Border.Top > 0)
                {
                    GuiPainter.DrawRectangle(spriteBatch, topRect, BorderColor, fTexture);
                }
                if (Border.Bottom > 0)
                {
                    GuiPainter.DrawRectangle(spriteBatch, bottomRect, BorderColor, fTexture);
                }
                if (Border.Left > 0)
                {
                    GuiPainter.DrawRectangle(spriteBatch, leftRect, BorderColor, fTexture);
                }
                if (Border.Right > 0)
                {
                    GuiPainter.DrawRectangle(spriteBatch, rightRect, BorderColor, fTexture);
                }
            }
            if (BackgroundColor != Color.Transparent)
            {
                Rectangle innerRect = new Rectangle(innerX0, innerY0, innerWidth, innerHeigth);
                GuiPainter.DrawRectangle(spriteBatch, innerRect, BackgroundColor, fTexture);
            }
            if (Content != null)
            {
                Rectangle childRect = new Rectangle(clipRect.Left + Border.Left, clipRect.Top + Border.Top, innerWidth, innerHeigth);

                Content.Draw(spriteBatch, new GuiPoint(point.X + Offset.X + Border.Left, point.Y + Offset.Y + Border.Top), childRect);
            }
            //base.DoDraw(spriteBatch, point, clipRect);
        }

        protected override GuiElement DoFindElement(Point point)
        {
            GuiElement result = null;
            int outerX0 = point.X + Offset.X;
            int outerY0 = point.Y + Offset.Y;
            int innerX0 = outerX0 + Border.Left;
            int innerY0 = outerY0 + Border.Top;
            int outerWidth = RenderSize.Width;
            int outerHeight = RenderSize.Height;
            int innerWidth = outerWidth - Border.Width;
            int innerHeigth = outerHeight - Border.Height;

            Rectangle topRect = new Rectangle(outerX0, outerY0, outerWidth - Border.Right, Border.Top);
            Rectangle bottomRect = new Rectangle(innerX0, outerY0 + innerHeigth + Border.Top, outerWidth, Border.Bottom);
            Rectangle leftRect = new Rectangle(outerX0, innerY0, Border.Left, outerHeight);
            Rectangle rightRect = new Rectangle(outerX0 + innerWidth + Border.Left, outerY0, Border.Right, outerHeight - Border.Bottom);

            if (point.X >= (DrawPosition.X + Offset.X + Border.Left) &&
                point.X < (DrawPosition.X + Offset.X + RenderSize.Width - Border.Right) &&
                point.Y >= (DrawPosition.Y + Offset.Y + Border.Top) &&
                point.Y < (DrawPosition.Y + Offset.Y + RenderSize.Height - Border.Bottom))
            {
                result = Content;
                if (result != null)
                {
                    Point subPoint = new Point(point.X, point.Y);
                    GuiElement subElement = Content.FindElement(subPoint);
                    if (subElement != null)
                        result = subElement;
                }
            }

            return result;
        }

        #endregion


        public GuiSizeElements GetElements(Point point)
        {
            GuiSizeElements elements = GuiSizeElements.None;
            int x = point.X - DrawPosition.X - Offset.X;
            int y = point.Y - DrawPosition.Y - Offset.Y;
            if (x >= 0 && x <= Border.Left)
                elements |= GuiSizeElements.Left;
            if (y >= 0 && y <= Border.Top)
                elements |= GuiSizeElements.Top;
            x -= RenderSize.Width;
            y -= RenderSize.Height;
            if (x <= 0 && x >= -Border.Right)
                elements |= GuiSizeElements.Width;
            if (y <= 0 && y >= -Border.Bottom)
                elements |= GuiSizeElements.Height;

            return elements;
        }

    }
}
