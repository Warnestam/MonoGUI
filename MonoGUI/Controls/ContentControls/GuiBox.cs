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


namespace MonoGUI.Controls
{

    public class GuiBox : GuiControl
    {

        private Texture2D fTexture;

        public GuiBox()
        {
            Width = 0;
            Height = 0;
        }

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
            if (BackgroundColor!=Color.Transparent)
            {
                GuiPainter.DrawRectangle(spriteBatch, r, BackgroundColor, fTexture);
            }
            base.DoDraw(spriteBatch);

        }




    }

}
