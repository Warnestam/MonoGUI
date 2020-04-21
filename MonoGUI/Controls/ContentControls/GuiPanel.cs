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
 * File:		GuiPanel
 * Purpose:		A simple panel
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2018-11-14  RW  Created
 * 
 */
namespace MonoGUI.Controls
{

    /// <summary>
    /// A simple panel
    /// </summary>
    public class GuiPanel : GuiContentControl
    {

        #region Private members

        private Texture2D fTexture;

        #endregion

        #region Constructor

        public GuiPanel()
        {
        }

        #endregion

        #region Overrides

        public override void Initialize(GraphicsDevice device)
        {
            base.Initialize(device);

        }

        public override void LoadContent(GuiEngine engine)
        {

            fTexture = GuiPainter.GetTexture1x1(Device);
            /*
            int w = 100;
            int h = 2;
            int pixels = w * h;
            Color[] colors = new Color[pixels];
            Texture2D texture = new Texture2D(device, w,h);
            int index = 0;
            for (int y = 0; y < h; y++) 
            {
                for (int x = 0; x < w; x++)
                {
                    Color c = new Color(1.0f*x / w, 1.0f * y / h, 1.0f);
                    colors[index++] = c;
                }
            }
            texture.SetData<Color>(colors);
            fTexture = texture;
            */
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
                Content.Measure(frameworkAvailableSize);
                desiredSize = Content.DesiredSize;
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

            if (Content != null)
            {
                Content.Arrange(arrangeSize);
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
            base.DoDraw(spriteBatch,point, clipRect);
        }

        #endregion

    }
}
