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

    public class GuiWindow : GuiControl
    {

        private Texture2D fTexture;
        public override void DrawCore(SpriteBatch spriteBatch, GuiRect finalRect)
        {
            base.DrawCore(spriteBatch,finalRect);
            Rectangle r = new Rectangle( 
                Convert.ToInt32(finalRect.X),
                Convert.ToInt32(finalRect.Y),
                Convert.ToInt32(finalRect.Width),
                Convert.ToInt32(finalRect.Height));
            GuiPainter.DrawRectangle(spriteBatch, r, Color.White, fTexture);
        }

        public override void Initialize(GraphicsDevice device)
        {
            base.Initialize(device);

            fTexture = GuiPainter.CreateTexture1x1(device);
        }

        public override void LoadContent()
        {
            base.LoadContent();


        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

    }

}
