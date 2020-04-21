using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGUI.Graphics;
using MonoGUI.GameComponents;
using MonoGUI.Engine;
using MonoGUI.Themes;


/*
 * File:		GuiWindow
 * Purpose:		GuiWindow is the top most container for all GuiControls
 * 
 * Author(s):	RW: Robert Warnestam
 * Created:		2017-11-09  RW	
 * 
 * History:		2017-11-09  RW  Created
 * 
 */
namespace MonoGUI.Controls
{

    public class GuiWindow : GuiContentControl
    {

        private Texture2D fTexture;

        public override void Initialize(GraphicsDevice device)
        {
            base.Initialize(device);

            fTexture = GuiPainter.CreateTexture1x1(device);
        }

        public override void LoadContent(GuiEngine engine)
        {
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


        protected override void DoDraw(SpriteBatch spriteBatch)
        {
            Rectangle r = new Rectangle(
                FinalRect.X + Offset.X,
                FinalRect.Y + Offset.Y,
                RenderSize.Width,
                RenderSize.Height);
            if (BackgroundColor != Color.Transparent)
            {
                GuiPainter.DrawRectangle(spriteBatch, r, BackgroundColor, fTexture);
            }

            base.DoDraw(spriteBatch);


        }




    }

}
