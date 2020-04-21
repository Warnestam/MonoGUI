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
 * File:		GuiBox
 * Purpose:		A simple box
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2018-11-14  RW  Created
 * 
 */
namespace MonoGUI.Controls
{

    /// <summary>
    /// A simple box
    /// </summary>
    public class GuiBox : GuiElement
    {

        #region Private members

        private Texture2D fTexture;

        #endregion

        #region Constructor

        public GuiBox()
        {
            //Width = 0;
            //Height = 0;
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
            Rectangle r = new Rectangle(
               point.X + Offset.X,
               point.Y + Offset.Y,
               RenderSize.Width,
               RenderSize.Height);
            if (BackgroundColor!=Color.Transparent)
            {
                GuiPainter.DrawRectangle(spriteBatch, r, BackgroundColor, fTexture);
            }
            base.DoDraw(spriteBatch,point, clipRect);

        }

        #endregion

    }

}
