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
 * File:		GuiLabel
 * Purpose:		Control for a line of text
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2018-11-14  RW  Created
 * 
 */
namespace MonoGUI.Controls
{

    /// <summary>
    /// Control for a line of text
    /// </summary>
    public class GuiLabel : GuiElement
    {

        #region Private members

        private string fText;

        private Texture2D fTexture;

        #endregion

        #region Properties

        public Color ForegroundColor { get; set; } = Color.Black;

        public string Text
        {
            get => fText;
            set
            {
                if (fText!=value)
                {
                    fText = value;
                    InvalidateMeasure();
                }
            }
        }

        public SpriteFont Font { get; set; }

        public GuiThickness Padding = new GuiThickness(0);

        #endregion

        #region Constructor

        public GuiLabel()
        {
            this.Text = String.Empty;
        }

        #endregion

        #region Overrides

        protected override GuiSize DoMeasure(GuiSize availableSize)
        {
            Vector2 size = Font.MeasureString(Text);
            GuiSize desiredSize = new GuiSize(Convert.ToInt32(size.X), Convert.ToInt32(size.Y));
            desiredSize.Width += Padding.Width;
            desiredSize.Height += Padding.Height;
            return desiredSize;
        }

        protected override GuiSize DoArrange(GuiSize arrangeSize)
        {
            //GuiMinMax mm = new GuiMinMax(this);

            //arrangeSize.Width = Math.Max(mm.minWidth, Math.Min(arrangeSize.Width, mm.maxWidth));
            //arrangeSize.Height = Math.Max(mm.minHeight, Math.Min(arrangeSize.Height, mm.maxHeight));

            return arrangeSize;
        }

        public override void Initialize(GraphicsDevice device)
        {
            base.Initialize(device);
        }

        public override void LoadContent(GuiEngine engine)
        {
            fTexture = GuiPainter.GetTexture1x1(Device);
            Font = engine.StandardFont;

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
        
        protected override void DoDraw(SpriteBatch spriteBatch, GuiPoint point, Rectangle clipRect)
        {

            if (BackgroundColor != Color.Transparent)
            {
                Rectangle r = new Rectangle(
                   point.X + Offset.X,
                   point.Y + Offset.Y,
                   RenderSize.Width,
                   RenderSize.Height);
                GuiPainter.DrawRectangle(spriteBatch, r, BackgroundColor, fTexture);
            }

            Vector2 position = new Vector2(
                point.X + Offset.X + Padding.Left,
                point.Y + Offset.Y + Padding.Top);

            spriteBatch.DrawString(Font, Text, position, ForegroundColor);
        }

        #endregion

    }

}
