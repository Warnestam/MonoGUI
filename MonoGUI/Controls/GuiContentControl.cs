using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;

using MonoGUI.Graphics;
using MonoGUI.Engine;

/*
 * File:		GuiContentControl
 * Purpose:		A control to hold another control
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2018-11-14  RW  Created
 * 
 */
namespace MonoGUI.Controls
{

    /// <summary>
    /// A control to hold another control
    /// </summary>
    public class GuiContentControl : GuiElement
    {

        #region Private memebrs

        private GuiElement fContent;

        #endregion

        #region Properties

        public GuiElement Content
        {
            get => fContent;
            set
            {
                if (fContent!=value)
                {
                    fContent = value;
                    if (fContent != null)
                        fContent.Parent = this;
                    InvalidateMeasure();
                }
            }
        }

        #endregion

        #region Overrrides

        public override void Initialize(GraphicsDevice device)
        {
            base.Initialize(device);
            if (Content != null)
            {
                Content.Initialize(device);
            }
        }

        public override void LoadContent(GuiEngine engine)
        {
            base.LoadContent(engine);
            if (Content != null)
            {
                Content.LoadContent(engine);
            }
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            if (Content != null)
            {
                Content.UnloadContent();
            }
        }
        
        protected override GuiSize DoMeasure(GuiSize availableSize)
        {
            GuiSize frameworkAvailableSize = new GuiSize(availableSize.Width, availableSize.Height);

            GuiMinMax mm = new GuiMinMax(this);

            frameworkAvailableSize.Width = Math.Max(mm.minWidth, Math.Min(frameworkAvailableSize.Width, mm.maxWidth));
            frameworkAvailableSize.Height = Math.Max(mm.minHeight, Math.Min(frameworkAvailableSize.Height, mm.maxHeight));

            GuiSize desiredSize = frameworkAvailableSize;

            if (Content != null)
            {
                Content.Measure(frameworkAvailableSize);
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

            if (Content != null)
            {
                Content.Arrange(arrangeSize);
            }

            return arrangeSize;
        }

        protected override void DoDraw(SpriteBatch spriteBatch, GuiPoint point, Rectangle clipRect)
        {
            if (Content != null)
            {
                Content.Draw(spriteBatch, new GuiPoint(point.X + Offset.X, point.Y + Offset.Y), clipRect);
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

        protected override void DoInvalidateMeasure()
        {
            if (Content != null)
            {
                Content.InvalidateMeasure();
            }
        }

        #endregion

    }



}
