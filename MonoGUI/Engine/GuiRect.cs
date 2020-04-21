using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGUI.Graphics;
using MonoGUI.Controls;
using MonoGUI.Engine;
using MonoGUI.GameComponents;
/*
 * File:		GuiRect
 * Purpose:		Describes the width, height, and location of a rectangle. 
 * 
 * Author(s):	RW: Robert Warnestam
 * 
 * History:		2018-05-25  RW  Created
 * 
 */
namespace MonoGUI.Engine
{

    /// <summary>
    /// Describes the width, height, and location of a rectangle. 
    /// </summary>
    public struct GuiRect
    {

        private readonly static GuiRect EMPTY_RECT = CreateEmptyRect();

        /// <summary>
        /// Gets or sets the height of the rectangle. 
        /// </summary>
        public int Height;

        /// <summary>
        /// Gets or sets the width of the rectangle. 
        /// </summary>
        public int Width;

        /// <summary>
        /// Gets or sets the x-axis value of the left side of the rectangle. 
        /// </summary>
        public int X;

        /// <summary>
        /// Gets or sets the y-axis value of the top side of the rectangle. 
        /// </summary>
        public int Y;

        /// <summary>
        /// Gets the y-axis value of the bottom of the rectangle. 
        /// </summary>
        public int Bottom { get => Y + Height; }

        /// <summary>
        /// Gets the position of the bottom-left corner of the rectangle 
        /// </summary>
        public GuiPoint BottomLeft { get => new GuiPoint(Bottom, Left); }

        /// <summary>
        /// Gets the position of the bottom-right corner of the rectangle. 
        /// </summary>
        public GuiPoint BottomRight { get => new GuiPoint(Bottom, Right); }

        
        /// <summary>
        /// Gets the x-axis value of the left side of the rectangle. 
        /// </summary>
        public int Left { get => X; }

        /// <summary>
        /// Gets or sets the position of the top-left corner of the rectangle.
        /// </summary>
        public GuiPoint Location
        {
            get => new GuiPoint(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }            
        }

        /// <summary>
        /// Gets the x-axis value of the right side of the rectangle. 
        /// </summary>
        public int Right { get => X + Width; }

        /// <summary>
        /// Gets the y-axis position of the top of the rectangle. 
        /// </summary>
        public int Top { get => Y; }

        /// <summary>
        /// Gets the position of the top-left corner of the rectangle. 
        /// </summary>
        public GuiPoint TopLeft { get => new GuiPoint(Top, Left); }

        /// <summary>
        /// Gets the position of the top-right corner of the rectangle. 
        /// </summary>
        public GuiPoint TopRight { get => new GuiPoint(Top, Right); }

        public GuiRect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public GuiRect(GuiSize size)
        {
            X = 0;
            Y = 0;
            Width = size.Width;
            Height = size.Height;
        }

        /// <summary>
        /// IsEmpty - this returns true if this rect is the Empty rectangle.
        /// Note: If width or height are 0 this Rectangle still contains a 0 or 1 dimensional set
        /// of points, so this method should not be used to check for 0 area.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
               return Width < 0;
            }
        }

        /// <summary>
        /// Gets a special value that represents a rectangle with no position or area. 
        /// </summary>
        public static GuiRect Empty
        {
            get
            {
                return EMPTY_RECT;
            }
        }

        private static GuiRect CreateEmptyRect()
        {
            GuiRect rect = new GuiRect();
            rect.X = -1;
            rect.Y = -1;
            rect.Width = -1;
            rect.Height = -1;
            return rect;
        }

        /// <summary>
        /// Size - The Size representing the area of the Rectangle
        /// </summary>
        public GuiSize Size
        {
            get
            {
                if (IsEmpty)
                    return GuiSize.Empty;
                return new GuiSize(Width, Height);
            }
            set
            {
                if (value.IsEmpty)
                {
                    this = EMPTY_RECT;
                }
                else
                {
                    Width = value.Width;
                    Height = value.Height;
                }
            }
        }

        public bool IntersectsWith(GuiRect rect)
        {
            if (IsEmpty || rect.IsEmpty)
            {
                return false;
            }

            return (rect.Left <= Right) &&
                   (rect.Right >= Left) &&
                   (rect.Top <= Bottom) &&
                   (rect.Bottom >= Top);
        }

        public void Intersect(GuiRect rect)
        {
            if (!this.IntersectsWith(rect))
            {
                this = Empty;
            }
            else
            {
                int left = Math.Max(Left, rect.Left);
                int top = Math.Max(Top, rect.Top);

                //  Max with 0 to prevent double weirdness from causing us to be (-epsilon..0)
                Width = Math.Max(Math.Min(Right, rect.Right) - left, 0);
                Height = Math.Max(Math.Min(Bottom, rect.Bottom) - top, 0);

                X = left;
                Y = top;
            }
        }

        public override string ToString()
        {
            return $"{Left},{Top},{Width},{Height}";
        }


    }


}



