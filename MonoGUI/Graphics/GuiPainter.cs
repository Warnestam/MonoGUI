using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Text;
/*
 * File:		GuiPainter
 * Purpose:		A utility class for drawing
 * 
 * Author(s):	RW: Robert Warnestam
 * History:		2017-11-09  RW  Created
 * 
 * 
 */
namespace MonoGUI.Graphics
{

    /// <summary>
    /// A utility class for drawing
    /// </summary>
    public static class GuiPainter
    {

        #region Private members

        private static Texture2D fTexture1x1;

        #endregion


        public static Texture2D GetTexture1x1(GraphicsDevice device)
        {
            if (fTexture1x1 == null)
                fTexture1x1 = CreateTexture1x1(device);
            return fTexture1x1;
        }

        public static Texture2D CreateTexture1x1(GraphicsDevice device)
        {
            Texture2D texture = new Texture2D(device, 1, 1);
            texture.SetData<Color>(new Color[] { Color.White });
            return texture;
        }

        public static Texture2D CreateTexture1x1(GraphicsDevice device, Color color)
        {
            Texture2D texture = new Texture2D(device, 1, 1);
            texture.SetData<Color>(new Color[] { color });
            return texture;
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 begin, Vector2 end, Color color, Texture2D texture)
        {
            Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + 1, 1);
            Vector2 v = Vector2.Normalize(begin - end);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;
            spriteBatch.Draw(texture, r, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }

        public static void DrawHorizontalLine(SpriteBatch spriteBatch, int x, int y, int length, Color color, Texture2D texture)
        {
            Rectangle r = new Rectangle(x, y, length, 1);
            spriteBatch.Draw(texture, r, null, color, 0, Vector2.Zero, SpriteEffects.None, 0);
        }

        public static void DrawVerticalLine(SpriteBatch spriteBatch, int x, int y, int length, Color color, Texture2D texture)
        {
            Rectangle r = new Rectangle(x, y, 1, length);
            spriteBatch.Draw(texture, r, null, color, 0, Vector2.Zero, SpriteEffects.None, 0);
        }

        public static void DrawRectangle(SpriteBatch spriteBatch, Rectangle r, Color color, Texture2D texture)
        {
            spriteBatch.Draw(texture, r, null, color, 0, Vector2.Zero, SpriteEffects.None, 0);
        }

        public static Color ReplaceAlpha(Color color, byte alpha)
        {
            Color result = color;
            color.A = alpha;
            color.G = 0;
            return color;
        }
    }

}
